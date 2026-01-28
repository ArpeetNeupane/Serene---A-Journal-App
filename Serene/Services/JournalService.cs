using Microsoft.EntityFrameworkCore;
using Serene.Data;
using Serene.Entities;


/// <summary>
/// Provides CRUD operations and querying capabilities for journal entries
/// within the Serene application.
/// </summary>
/// <remarks>
/// This service handles retrieval, creation, updating, and deletion of
/// journal entries. It supports filtered and paginated queries, date-based
/// retrieval (e.g., today's entry), tag management, and ensures that only
/// one entry per day is maintained. All database operations are performed
/// asynchronously using Entity Framework Core.
/// </remarks>
namespace Serene.Services
{
    public class JournalService : IJournalService
    {
        public async Task<(List<JournalEntry> Entries, int TotalCount)> GetPaginatedEntriesAsync(
            string search, string mood, string tag,
            DateTime? startDate, DateTime? endDate,
            int page, int pageSize)
        {
            var query = _context.JournalEntries.AsQueryable();

            //applying text search
            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(e => e.Title.Contains(search) || e.ContentHtml.Contains(search));

            //applying mood filter
            if (!string.IsNullOrWhiteSpace(mood) && mood != "All")
                query = query.Where(e => e.PrimaryMood == mood);

            //applying tag filter
            if (!string.IsNullOrWhiteSpace(tag) && tag != "All")
                query = query.Where(e => e.Tags.Contains(tag));

            //Filtering by Date Range
            if (startDate.HasValue)
                query = query.Where(e => e.EntryDate >= startDate.Value.Date);

            if (endDate.HasValue)
                query = query.Where(e => e.EntryDate <= endDate.Value.Date);

            int totalCount = await query.CountAsync();

            var entries = await query
                .OrderByDescending(e => e.EntryDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (entries, totalCount);
        }

        private readonly AppDbContext _context;

        public JournalService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<JournalEntry?> GetTodayEntryAsync()
        {
            return await GetEntryByDateAsync(DateTime.Today);
        }

        public async Task<JournalEntry?> GetEntryByDateAsync(DateTime date)
        {
            return await _context.JournalEntries
                .FirstOrDefaultAsync(e => e.EntryDate.Date == date.Date);
        }

        public async Task DeleteEntryAsync(Guid id)
        {
            var entry = await _context.JournalEntries.FindAsync(id);
            if (entry != null)
            {
                _context.JournalEntries.Remove(entry);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpsertEntryAsync(JournalEntry entry)
        {
            //stripping the time portion to ensure "2025-12-20 10:30" becomes "2025-12-20 00:00"
            var targetDate = entry.EntryDate.Date;
            entry.EntryDate = targetDate;

            //checking if an entry already exists for this specific day
            var existing = await _context.JournalEntries
                .FirstOrDefaultAsync(e => e.EntryDate == targetDate);

            if (existing == null)
            {
                //making sure creating 1 a day is implemented
                entry.CreatedAt = DateTime.Now; //system Timestamp
                entry.UpdatedAt = DateTime.Now;
                _context.JournalEntries.Add(entry);
            }
            else
            {
                //doing this to update the existing entry instead of creating a duplicate
                //updating the content but keep the original CreatedAt timestamp
                entry.Id = existing.Id;
                entry.CreatedAt = existing.CreatedAt;
                entry.UpdatedAt = DateTime.Now;

                _context.Entry(existing).CurrentValues.SetValues(entry);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<JournalEntry>> GetFilteredEntriesAsync(string search, string mood, string tag)
        {
            var query = _context.JournalEntries.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(e => e.Title.Contains(search) || e.ContentHtml.Contains(search));

            if (!string.IsNullOrWhiteSpace(mood) && mood != "All")
                query = query.Where(e => e.PrimaryMood == mood);

            if (!string.IsNullOrWhiteSpace(tag) && tag != "All")
                query = query.Where(e => e.Tags.Contains(tag));

            return await query.OrderByDescending(e => e.EntryDate).ToListAsync();
        }

        public async Task<List<DateTime>> GetEntryDatesAsync(int month, int year)
        {
            return await _context.JournalEntries
                .Where(e => e.EntryDate.Month == month && e.EntryDate.Year == year)
                .Select(e => e.EntryDate.Date)
                .ToListAsync();
        }

        public async Task<List<string>> GetAllUniqueTagsAsync()
        {
            var allTagsStrings = await _context.JournalEntries
                .Where(e => !string.IsNullOrEmpty(e.Tags))
                .Select(e => e.Tags)
                .ToListAsync();

            //splitting comma-separated strings and getting unique values
            return allTagsStrings
                .SelectMany(t => t.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Select(t => t.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(t => t)
                .ToList();
        }
    }

}

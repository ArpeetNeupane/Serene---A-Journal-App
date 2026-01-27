using Microsoft.EntityFrameworkCore;
using Serene.Data;
using Serene.Entities;
using Serene.Services;

namespace Serene.Services
{
    public class JournalService : IJournalService
    {
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
                //doing this if it's the first time saving today
                _context.JournalEntries.Add(entry);
            }
            else
            {
                //doing this to update the existing entry instead of creating a duplicate
                _context.Entry(existing).CurrentValues.SetValues(entry);
                existing.UpdatedAt = DateTime.Now;
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
    }

}

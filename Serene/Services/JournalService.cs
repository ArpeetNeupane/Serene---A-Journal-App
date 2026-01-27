using Microsoft.EntityFrameworkCore;
using Serene.Data;
using Serene.Entities;
using System;
using System.Threading.Tasks;

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

        public async Task UpsertEntryAsync(JournalEntry entry)
        {
            var existing = await _context.JournalEntries
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == entry.Id || e.EntryDate.Date == entry.EntryDate.Date);

            entry.UpdatedAt = DateTime.Now;

            if (existing == null)
                _context.JournalEntries.Add(entry);
            else
            {
                entry.Id = existing.Id;
                _context.JournalEntries.Update(entry);
            }

            await _context.SaveChangesAsync();
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
    }
}

using Serene.Entities;
using System;
using System.Threading.Tasks;

namespace Serene.Services
{
    public interface IJournalService
    {
        Task<JournalEntry?> GetTodayEntryAsync();
        Task<JournalEntry?> GetEntryByDateAsync(DateTime date);
        Task UpsertEntryAsync(JournalEntry entry);
        Task DeleteEntryAsync(Guid id);
    }
}
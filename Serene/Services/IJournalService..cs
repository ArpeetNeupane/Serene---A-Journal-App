using Serene.Entities;


//interface for JournalService
namespace Serene.Services
{
    public interface IJournalService
    {
        Task<JournalEntry?> GetTodayEntryAsync();
        Task<List<string>> GetAllUniqueTagsAsync();
        Task<JournalEntry?> GetEntryByDateAsync(DateTime date);
        Task<List<JournalEntry>> GetFilteredEntriesAsync(string search, string mood, string tag);
        Task UpsertEntryAsync(JournalEntry entry);
        Task<List<DateTime>> GetEntryDatesAsync(int month, int year);
        Task DeleteEntryAsync(Guid id);
        Task<(List<JournalEntry> Entries, int TotalCount)> GetPaginatedEntriesAsync(
            string search, string mood, string tag,
            DateTime? startDate, DateTime? endDate,
            int page, int pageSize
        );
    }
}
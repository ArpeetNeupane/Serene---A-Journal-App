using Serene.Services;
using Serene.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;


/// <summary>
/// Provides analytical insights for journal entries in the Serene application.
/// </summary>
/// <remarks>
/// This service calculates various statistics such as average word count,
/// mood distribution, daily word count trends, most used tags, and top
/// frequently used words. It interacts with the database using EF Core
/// queries and performs lightweight text processing, including stripping
/// HTML tags and filtering common stop words, to generate meaningful analytics.
/// </remarks>
public class AnalyticsService : IAnalyticsService
{
    private readonly AppDbContext _context;

    public AnalyticsService(AppDbContext context) => _context = context;
    public async Task<int> GetAverageWordCountAsync()
    {
        var hasEntries = await _context.JournalEntries.AnyAsync();
        if (!hasEntries) return 0;

        //using EF's Average method directly for performance
        return (int)await _context.JournalEntries.AverageAsync(e => e.WordCount);
    }

    public async Task<Dictionary<string, int>> GetMoodDistributionAsync()
    {
        return await _context.JournalEntries
            .GroupBy(e => e.PrimaryMood)
            .Select(g => new { Mood = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Mood, x => x.Count);
    }

    public async Task<List<(string Date, int Count)>> GetDailyWordCountTrendsAsync()
    {
        var startDate = DateTime.Today.AddDays(-14);
        var entries = await _context.JournalEntries
            .Where(e => e.EntryDate >= startDate)
            .OrderBy(e => e.EntryDate)
            .Select(e => new { e.EntryDate, e.WordCount })
            .ToListAsync();

        return entries.Select(x => (x.EntryDate.ToString("MM/dd"), x.WordCount)).ToList();
    }

    public async Task<List<(string Tag, int Count)>> GetMostUsedTagsAsync(int limit = 6)
    {
        var allTagsStrings = await _context.JournalEntries
            .Where(e => !string.IsNullOrEmpty(e.Tags))
            .Select(e => e.Tags)
            .ToListAsync();

        return allTagsStrings
            .SelectMany(t => t.Split(',', StringSplitOptions.RemoveEmptyEntries))
            .Select(t => t.Trim())
            .GroupBy(t => t.ToLower()) //grouping by lowercase to avoid duplicates
            .OrderByDescending(g => g.Count())
            .Take(limit)
            .Select(g => (Tag: g.Key, Count: g.Count()))
            .ToList();
    }

    public async Task<List<(string Word, int Count)>> GetTopUsedWordsAsync(int limit = 10)
    {
        var allEntries = await _context.JournalEntries.Select(e => e.ContentHtml).ToListAsync();
        if (!allEntries.Any()) return new();

        var stopWords = new HashSet<string> { "the", "and", "a", "to", "of", "i", "is", "in", "it", "that", "you", "for", "with", "was", "on", "at" };

        return allEntries
            .Select(html => Regex.Replace(html, "<.*?>", " "))
            .SelectMany(text => text.ToLower().Split(new[] { ' ', '.', ',', '!', '?', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
            .Where(word => word.Length > 2 && !stopWords.Contains(word))
            .GroupBy(word => word)
            .OrderByDescending(g => g.Count())
            .Take(limit)
            .Select(g => (Word: g.Key, Count: g.Count()))
            .ToList();
    }
}
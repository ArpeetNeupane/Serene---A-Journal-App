namespace Serene.Services;


//interface for AnalyticsService
public interface IAnalyticsService
{
    Task<int> GetAverageWordCountAsync();
    Task<Dictionary<string, int>> GetMoodDistributionAsync();
    Task<List<(string Date, int Count)>> GetDailyWordCountTrendsAsync();
    Task<List<(string Word, int Count)>> GetTopUsedWordsAsync(int limit);
    Task<List<(string Tag, int Count)>> GetMostUsedTagsAsync(int limit);
}
namespace Serene.Services;


//interface for StreakService
public interface IStreakService
{
    Task<(int Current, int Longest, int Missed)> GetStreakStatsAsync();
}
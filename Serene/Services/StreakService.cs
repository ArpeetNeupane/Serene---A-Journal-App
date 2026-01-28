using Microsoft.EntityFrameworkCore;
using Serene.Data;
using Serene.Services;


/// <summary>
/// Provides functionality to calculate user journal streak statistics.
/// </summary>
/// <remarks>
/// This service computes the current streak, longest streak, and number of
/// missed days based on journal entry dates stored in the database. 
/// It considers consecutive daily entries, accounts for gaps, and ensures
/// the current streak is calculated correctly even if no entry exists today.
/// All operations are performed asynchronously using Entity Framework Core.
/// </remarks>
public class StreakService : IStreakService
{
    private readonly AppDbContext _context;

    public StreakService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(int Current, int Longest, int Missed)> GetStreakStatsAsync()
    {
        //fetching all entry dates sorted
        var dates = await _context.JournalEntries
        .OrderByDescending(e => e.EntryDate)
        .Select(e => e.EntryDate)
        .ToListAsync();

        var distinctDates = dates
            .Select(d => d.Date)
            .Distinct()
            .ToList();

        var dateSet = distinctDates.ToHashSet();

        if (!distinctDates.Any()) return (0, 0, 0);

        //calculating Current Streak
        int currentStreak = 0;
        DateTime checkDate = DateTime.Today;

        //if no entry today, check if streak is still alive from yesterday
        if (!dateSet.Contains(checkDate)) checkDate = checkDate.AddDays(-1);

        while (dateSet.Contains(checkDate))
        {
            currentStreak++;
            checkDate = checkDate.AddDays(-1);
        }

        //calculating Longest Streak
        int longestStreak = 0;
        int tempStreak = 0;
        var ascendingDates = distinctDates.OrderBy(d => d).ToList();

        for (int i = 0; i < ascendingDates.Count; i++)
        {
            tempStreak++;
            if (i == ascendingDates.Count - 1 || ascendingDates[i + 1] != ascendingDates[i].AddDays(1))
            {
                if (tempStreak > longestStreak) longestStreak = tempStreak;
                tempStreak = 0;
            }
        }

        //calculating Missed Days (since the very first entry)
        var firstEntry = ascendingDates.First();
        int totalDaysPossible = (DateTime.Today - firstEntry).Days + 1;
        int missedDays = totalDaysPossible - distinctDates.Count;

        return (currentStreak, longestStreak, Math.Max(0, missedDays));
    }
}
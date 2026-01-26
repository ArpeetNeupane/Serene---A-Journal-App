using Serene.Models;
using Serene.Common;

namespace Serene.Services;

public interface IStreakService
{
    public Task<ServiceResult<StreakUpdateResponse>> IncrementStreak(Guid userId);
    public Task<ServiceResult<StreakUpdateResponse>> DecrementStreak(Guid userId);
}

using Serene.Common;
using Serene.Data;
using Serene.Entities;
using Microsoft.EntityFrameworkCore;

namespace Serene.Services;

public class MoodsService : IMoodsService
{
    private readonly AppDbContext _context;

    public MoodsService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceResult<List<Moods>>> GetMoods()
    {
        try
        {
            List<Moods> Moods = await _context.Moods.ToListAsync();
            return ServiceResult<List<Moods>>.SuccessResult(Moods);
        }
        catch(Exception ex)
        {
            return ServiceResult<List<Moods>>.FailureResult($"Failed to get moods. {ex.Message}");
        }
    }
}

using Serene.Common;
using Serene.Data;
using Serene.Entities;
using Microsoft.EntityFrameworkCore;

namespace Serene.Services;

public class TagsService: ITagsService
{
    private readonly AppDbContext _context;
    
    public TagsService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get all the tags from the database
    /// </summary>
    /// <returns>List of all the tags in the database</returns>
    public async Task<ServiceResult<List<Tags>>> GetTags()
    {
        try
        {
            List<Tags> tags = await _context.Tags.ToListAsync();
            return ServiceResult<List<Tags>>.SuccessResult(tags);
        }
        catch (Exception ex)
        {
            return ServiceResult<List<Tags>>.FailureResult($"Failed to get tags {ex.Message}");
        }
    }
}

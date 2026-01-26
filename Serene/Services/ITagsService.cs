namespace Serene.Services;

using Serene.Common;
using Serene.Entities;

public interface ITagsService
{
    Task<ServiceResult<List<Tags>>> GetTags();
}

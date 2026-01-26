using Serene.Common;
using Serene.Entities;

namespace Serene.Services;

public interface IMoodsService
{
    Task<ServiceResult<List<Moods>>> GetMoods();
}

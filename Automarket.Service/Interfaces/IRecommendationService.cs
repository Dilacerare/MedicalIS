using Automarket.Domain.Entity;
using Automarket.Domain.Response;
using Automarket.Domain.ViewModels.Recommendation;

namespace Automarket.Service.Interfaces;

public interface IRecommendationService
{
    IBaseResponse<List<Recommendation>> GetRecommendations();
        
    Task<IBaseResponse<RecommendationViewModel>> GetRecommendation(long id);

    Task<IBaseResponse<Recommendation>> Create(RecommendationViewModel model);

    Task<IBaseResponse<bool>> DeleteRecommendation(long id);

    Task<IBaseResponse<Recommendation>> Edit(long id, Recommendation model);
}
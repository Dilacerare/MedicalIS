using Automarket.DAL.Interfaces;
using Automarket.Domain.Entity;
using Automarket.Domain.Enum;
using Automarket.Domain.Response;
using Automarket.Domain.ViewModels.Recommendation;
using Automarket.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Automarket.Service.Implementations;

public class RecommendationService : IRecommendationService
{
    private readonly IBaseRepository<Recommendation> _recommendationRepository;
    private readonly IBaseRepository<Profile> _proFileRepository;
    private readonly IBaseRepository<User> _userRepository;
    private readonly IProfileService _profileService;

    public RecommendationService(IBaseRepository<Recommendation> recommendationRepository, 
        IBaseRepository<Profile> proFileRepository, IBaseRepository<User> userRepository, IProfileService profileService)
    {
        _recommendationRepository = recommendationRepository;
        _proFileRepository = proFileRepository;
        _userRepository = userRepository;
        _profileService = profileService;
    }

    public IBaseResponse<List<Recommendation>> GetRecommendations()
    {
        try
        {
            var recommendations = _recommendationRepository.GetAll().ToList();
            if (!recommendations.Any())
            {
                return new BaseResponse<List<Recommendation>>()
                {
                    Description = "Найдено 0 элементов",
                    StatusCode = StatusCode.OK
                };
            }
                
            return new BaseResponse<List<Recommendation>>()
            {
                Data = recommendations,
                StatusCode = StatusCode.OK
            };
        }
        catch (Exception ex)
        {
            return new BaseResponse<List<Recommendation>>()
            {
                Description = $"[GetRecommendations] : {ex.Message}",
                StatusCode = StatusCode.InternalServerError
            };
        }
    }

    public async Task<IBaseResponse<RecommendationViewModel>> GetRecommendation(long id)
    {
        try
        {
            var recommendation = await _recommendationRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);
            var profile = await _profileService.GetProfile(recommendation.ProfileId);
            if (recommendation == null)
            {
                return new BaseResponse<RecommendationViewModel>()
                {
                    Description = "Пользователь не найден",
                    StatusCode = StatusCode.UserNotFound
                };
            }

            var data = new RecommendationViewModel()
            {
                Id = recommendation.Id,
                AuthorName = recommendation.Author,
                PatientName = profile.Data.UserName,
                Description = recommendation.Description,
                DateCreate = recommendation.DateCreate.ToString("dd/MM/yyyy")
            };

            return new BaseResponse<RecommendationViewModel>()
            {
                StatusCode = StatusCode.OK,
                Data = data
            };
        }
        catch (Exception ex)
        {
            return new BaseResponse<RecommendationViewModel>()
            {
                Description = $"[GetRecommendation] : {ex.Message}",
                StatusCode = StatusCode.InternalServerError
            };
        }
    }

    public async Task<IBaseResponse<Recommendation>> Create(RecommendationViewModel model)
    {
        try
        {
            // var author = _userRepository.GetAll().FirstOrDefaultAsync(x => x.Name == model.AuthorName).Result;
            var patient = _proFileRepository.GetAll().FirstOrDefaultAsync(x => x.User.Name == model.PatientName).Result;
            
            var health = new Recommendation()
            {    
                Author = model.AuthorName,
                Patient = patient,
                Description = model.Description,
                DateCreate = DateTime.Today
            }; 
            await _recommendationRepository.Create(health);

            return new BaseResponse<Recommendation>()
            {
                StatusCode = StatusCode.OK,
                Data = health
            };
        }
        catch (Exception ex)
        {
            return new BaseResponse<Recommendation>()
            {
                Description = $"[Create] : {ex.Message}",
                StatusCode = StatusCode.InternalServerError
            };
        }
    }

    public async Task<IBaseResponse<bool>> DeleteRecommendation(long id)
    {
        try
        {
            var recommendation = await _recommendationRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);
            if (recommendation == null)
            {
                return new BaseResponse<bool>()
                {
                    Description = "User not found",
                    StatusCode = StatusCode.UserNotFound,
                    Data = false
                };
            }

            await _recommendationRepository.Delete(recommendation);

            return new BaseResponse<bool>()
            {
                Data = true,
                StatusCode = StatusCode.OK
            };
        }
        catch (Exception ex)
        {
            return new BaseResponse<bool>()
            {
                Description = $"[DeleteRecommendation] : {ex.Message}",
                StatusCode = StatusCode.InternalServerError
            };
        }
    }

    public async Task<IBaseResponse<Recommendation>> Edit(long id, RecommendationViewModel model)
    {
        try
        {
            var recommendation = await _recommendationRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);
            if (recommendation == null)
            {
                return new BaseResponse<Recommendation>()
                {
                    Description = "Recommendation not found",
                    StatusCode = StatusCode.CarNotFound
                };
            }

            recommendation.Description = model.Description;

            await _recommendationRepository.Update(recommendation);


            return new BaseResponse<Recommendation>()
            {
                Data = recommendation,
                StatusCode = StatusCode.OK,
            };
        }
        catch (Exception ex)
        {
            return new BaseResponse<Recommendation>()
            {
                Description = $"[Edit] : {ex.Message}",
                StatusCode = StatusCode.InternalServerError
            };
        }
    }
}
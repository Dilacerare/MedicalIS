using Automarket.DAL.Interfaces;
using Automarket.Domain.Entity;
using Automarket.Domain.Enum;
using Automarket.Domain.Response;
using Automarket.Domain.ViewModels.Car;
using Automarket.Domain.ViewModels.PerfectHealth;
using Automarket.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Automarket.Service.Implementations;

public class PerfectHealthService : IPerfectHealthService
{
    private readonly IBaseRepository<PerfectHealth> _healthRepository;

    public PerfectHealthService(IBaseRepository<PerfectHealth> healthRepository)
    {
        _healthRepository = healthRepository;
    }

    public IBaseResponse<List<PerfectHealth>> GetHealths()
    {
        try
        {
            var healths = _healthRepository.GetAll().ToList();
            if (!healths.Any())
            {
                return new BaseResponse<List<PerfectHealth>>()
                {
                    Description = "Найдено 0 элементов",
                    StatusCode = StatusCode.OK
                };
            }
                
            return new BaseResponse<List<PerfectHealth>>()
            {
                Data = healths,
                StatusCode = StatusCode.OK
            };
        }
        catch (Exception ex)
        {
            return new BaseResponse<List<PerfectHealth>>()
            {
                Description = $"[GetHealths] : {ex.Message}",
                StatusCode = StatusCode.InternalServerError
            };
        }
    }

    public async Task<IBaseResponse<PerfectHealthViewModel>> GetHealth(long id)
    {
        try
        {
            var health = await _healthRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);
            if (health == null)
            {
                return new BaseResponse<PerfectHealthViewModel>()
                {
                    Description = "Пользователь не найден",
                    StatusCode = StatusCode.UserNotFound
                };
            }

            var data = new PerfectHealthViewModel()
            {
                Temperature = health.Temperature,
                BloodPressure = health.BloodPressure,
                GUrineAnalysis = health.GUrineAnalysis,
                GBloodTest = health.BloodPressure,
                Cholesterol = health.Cholesterol
            };

            return new BaseResponse<PerfectHealthViewModel>()
            {
                StatusCode = StatusCode.OK,
                Data = data
            };
        }
        catch (Exception ex)
        {
            return new BaseResponse<PerfectHealthViewModel>()
            {
                Description = $"[GetHealth] : {ex.Message}",
                StatusCode = StatusCode.InternalServerError
            };
        }
    }

    public async Task<IBaseResponse<PerfectHealth>> Create(PerfectHealthViewModel model)
    {
        try
        {
            var health = new PerfectHealth()
            {    
                Temperature = model.Temperature,
                BloodPressure = model.BloodPressure,
                GUrineAnalysis = model.GUrineAnalysis,
                GBloodTest = model.GBloodTest,
                Cholesterol = model.Cholesterol
            }; 
            await _healthRepository.Create(health);

            return new BaseResponse<PerfectHealth>()
            {
                StatusCode = StatusCode.OK,
                Data = health
            };
        }
        catch (Exception ex)
        {
            return new BaseResponse<PerfectHealth>()
            {
                Description = $"[Create] : {ex.Message}",
                StatusCode = StatusCode.InternalServerError
            };
        }
    }

    public async Task<IBaseResponse<bool>> DeleteHealth(long id)
    {
        try
        {
            var health = await _healthRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);
            if (health == null)
            {
                return new BaseResponse<bool>()
                {
                    Description = "User not found",
                    StatusCode = StatusCode.UserNotFound,
                    Data = false
                };
            }

            await _healthRepository.Delete(health);

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
                Description = $"[DeleteHealth] : {ex.Message}",
                StatusCode = StatusCode.InternalServerError
            };
        }
    }

    public async Task<IBaseResponse<PerfectHealth>> Edit(long id, PerfectHealthViewModel model)
    {
        try
        {
            var health = await _healthRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);
            if (health == null)
            {
                return new BaseResponse<PerfectHealth>()
                {
                    Description = "Health not found",
                    StatusCode = StatusCode.CarNotFound
                };
            }

            health.Temperature = model.Temperature;
            health.BloodPressure = model.BloodPressure;
            health.GUrineAnalysis = model.GUrineAnalysis;
            health.GBloodTest = model.GBloodTest;
            health.Cholesterol = model.Cholesterol;

            await _healthRepository.Update(health);


            return new BaseResponse<PerfectHealth>()
            {
                Data = health,
                StatusCode = StatusCode.OK,
            };
        }
        catch (Exception ex)
        {
            return new BaseResponse<PerfectHealth>()
            {
                Description = $"[Edit] : {ex.Message}",
                StatusCode = StatusCode.InternalServerError
            };
        }
    }
}
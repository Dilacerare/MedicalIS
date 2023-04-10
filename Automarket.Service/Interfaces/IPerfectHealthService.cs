using Automarket.Domain.Entity;
using Automarket.Domain.Response;
using Automarket.Domain.ViewModels.PerfectHealth;

namespace Automarket.Service.Interfaces;

public interface IPerfectHealthService
{
    IBaseResponse<List<PerfectHealth>> GetHealths();
        
    Task<IBaseResponse<PerfectHealthViewModel>> GetHealth(long id);

    Task<IBaseResponse<PerfectHealth>> Create(PerfectHealthViewModel model);

    Task<IBaseResponse<bool>> DeleteHealth(long id);

    Task<IBaseResponse<PerfectHealth>> Edit(long id, PerfectHealthViewModel model);
}
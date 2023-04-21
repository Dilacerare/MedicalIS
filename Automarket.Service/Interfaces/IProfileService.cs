using System.Collections.Generic;
using System.Threading.Tasks;
using Automarket.Domain.Entity;
using Automarket.Domain.Response;
using Automarket.Domain.ViewModels.Profile;
using Automarket.Domain.ViewModels.User;

namespace Automarket.Service.Interfaces
{
    public interface IProfileService
    {
        IBaseResponse<List<ProfileViewModel>> GetProfiles();
        
        Task<BaseResponse<ProfileViewModel>> GetProfile(string userName);

        Task<BaseResponse<Profile>> Save(ProfileViewModel model);
        
        Task<BaseResponse<Profile>> SaveAnalyzes(ProfileViewModel model);
    }
}
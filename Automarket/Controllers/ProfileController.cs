using System;
using System.Linq;
using System.Threading.Tasks;
using Automarket.Domain.ViewModels.Profile;
using Automarket.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Automarket.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpPost]
        public async Task<IActionResult> Save(ProfileViewModel model)
        {
            ModelState.Remove("Id");
            ModelState.Remove("UserName");
            ModelState.Remove("Recommendations");
            ModelState.Remove("Temperature");
            ModelState.Remove("BloodPressure");
            ModelState.Remove("GUrineAnalysis");
            ModelState.Remove("GBloodTest");
            ModelState.Remove("Cholesterol");
            if (ModelState.IsValid)
            {
                var response = await _profileService.Save(model);
                if (response.StatusCode == Domain.Enum.StatusCode.OK)
                {
                    return Json(new { description = response.Description });
                }
            }
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
        
        [HttpPost]
        public async Task<IActionResult> SaveAnalyzes(ProfileViewModel model)
        {
            ModelState.Remove("Id");
            ModelState.Remove("UserName");
            ModelState.Remove("Age");
            ModelState.Remove("Address");
            ModelState.Remove("Recommendations");
            if (ModelState.IsValid)
            {
                var response = await _profileService.SaveAnalyzes(model);
                if (response.StatusCode == Domain.Enum.StatusCode.OK)
                {
                    return Json(new { description = response.Description });
                }
            }
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
        
        public async Task<IActionResult> Detail()
        {
            var userName = User.Identity.Name;
            var response = await _profileService.GetProfile(userName);
            if (response.StatusCode == Domain.Enum.StatusCode.OK)
            {
                return View(response.Data);   
            }

            return View();
        }
    }
}
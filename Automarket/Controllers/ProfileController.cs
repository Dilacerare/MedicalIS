using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Automarket.DAL.Interfaces;
using Automarket.Domain.Entity;
using Automarket.Domain.Extensions;
using Automarket.Domain.Response;
using Automarket.Domain.ViewModels.Profile;
using Automarket.Domain.ViewModels.Recommendation;
using Automarket.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace Automarket.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IProfileService _profileService;
        
        private readonly IRecommendationService _recommendation;
        
        private readonly IBaseRepository<Profile> _profileRepository;

        public ProfileController(IProfileService profileService, IRecommendationService recommendation, IBaseRepository<Profile> profileRepository)
        {
            _profileService = profileService;
            _recommendation = recommendation;
            _profileRepository = profileRepository;
        }
        
        [Authorize(Roles = "Admin,Doctor")]
        [HttpGet]
        public IActionResult GetProfiles()
        {
            var response = _profileService.GetProfiles();
            if (response.StatusCode == Domain.Enum.StatusCode.OK)
            {
                return View(response.Data);   
            }
            return View("Error", $"{response.Description}");
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
        
        public async Task<IActionResult> Detail(string userName)
        {
            // var userName = User.Identity.Name;
            var response = await _profileService.GetProfile(userName);
            if (response.StatusCode == Domain.Enum.StatusCode.OK)
            {
                return View(response.Data);   
            }

            return View();
        }

        public IActionResult SaveRecommendation(string userName)
        {
            // ViewBag.Message = "loh";
            ViewData["UserName"] = userName;
            return PartialView();
        }


        [HttpPost]
        public async Task<IActionResult> SaveRecommendation(RecommendationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var response = await _recommendation.Create(model);
                if (response.StatusCode == Domain.Enum.StatusCode.OK)
                {
                    return Json(new { description = response.Description });
                }
        
                return BadRequest(new { errorMessage = response.Description });
            }
        
            var errorMessage = ModelState.Values
                .SelectMany(v => v.Errors.Select(x => x.ErrorMessage)).ToList().Join();
            return StatusCode(StatusCodes.Status500InternalServerError, new { errorMessage });
        }
    }
}
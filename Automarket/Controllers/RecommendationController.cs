using System;
using System.Linq;
using System.Threading.Tasks;
using Automarket.Domain.Entity;
using Automarket.Domain.Extensions;
using Automarket.Domain.ViewModels.Recommendation;
using Automarket.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Automarket.Controllers;

public class RecommendationController : Controller
{
    private readonly IRecommendationService _recommendation;

    private readonly IProfileService _profile;

    private static int _lastRecommendationId;

    public RecommendationController(IRecommendationService recommendation, IProfileService profile)
    {
        _recommendation = recommendation;
        _profile = profile;
    }

    [HttpGet]
    public async Task<ActionResult> GetRecommendation(int id)
    {
        if (id == 0)
        {
            id = _lastRecommendationId;
        }
        var response = await _recommendation.GetRecommendation(id);
            if (response.StatusCode == Domain.Enum.StatusCode.OK)
            {
                return View(response.Data);   
            }
            return View("Error", $"{response.Description}");
    }

    [HttpGet]
    public IActionResult GetRecommendations()
    {
        var response = _recommendation.GetRecommendations();
        if (response.StatusCode == Domain.Enum.StatusCode.OK)
        {
            return View(response.Data);   
        }
        return View("Error", $"{response.Description}");
    }
    
    
    [HttpGet]
    public async Task<ActionResult> Edit(long id)
    {
        var response = await _recommendation.GetRecommendation(id);

        return View(response.Data);

    }

    [HttpPost]
    public async Task<IActionResult> Edit(long id, RecommendationViewModel model)
    {
        if (ModelState.IsValid)
        {
            var response = await _recommendation.Edit(id, model);
            if (response.StatusCode == Domain.Enum.StatusCode.OK)
            {
                _lastRecommendationId = (int)id;
                return RedirectToAction("GetRecommendation", "Recommendation",  id);
            }

            return BadRequest(new { errorMessage = response.Description });
        }

        var errorMessage = ModelState.Values
            .SelectMany(v => v.Errors.Select(x => x.ErrorMessage)).ToList().Join();
        return StatusCode(StatusCodes.Status500InternalServerError, new { errorMessage });
    }
}
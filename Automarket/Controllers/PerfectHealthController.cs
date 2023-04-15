using System.Linq;
using System.Threading.Tasks;
using Automarket.Domain.Extensions;
using Automarket.Domain.ViewModels.PerfectHealth;
using Automarket.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Automarket.Controllers;

public class PerfectHealthController : Controller
{
    private readonly IPerfectHealthService _health;

    public PerfectHealthController(IPerfectHealthService healthService)
    {
        _health = healthService;
    }
    
    [HttpGet]
    public IActionResult GetHealths()
    {
        var response = _health.GetHealths();
        if (response.StatusCode == Domain.Enum.StatusCode.OK)
        {
            return View(response.Data);   
        }
        return View("Error", $"{response.Description}");
    }
    
    public async Task<IActionResult> DeleteHealth(long id)
    {
        var response = await _health.DeleteHealth(id);
        if (response.StatusCode == Domain.Enum.StatusCode.OK)
        {
            return RedirectToAction("GetHealths");
        }
        return RedirectToAction("Index", "Home");
    }
    
    public IActionResult Save() => PartialView();

    [HttpPost]
    public async Task<IActionResult> Save(PerfectHealthViewModel model)
    {
        if (ModelState.IsValid)
        {
            var response = await _health.Create(model);
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
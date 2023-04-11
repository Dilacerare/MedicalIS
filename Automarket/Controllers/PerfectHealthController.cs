using Automarket.Service.Interfaces;
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
}
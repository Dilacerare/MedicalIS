using System.Numerics;
using Microsoft.AspNetCore.Mvc;

namespace Automarket.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();
        
        public IActionResult Services() => View();

        public IActionResult WorkOnIt() => View();
    }
}
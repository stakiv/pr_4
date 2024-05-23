using _4praktika.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace _4praktika.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult MainPage()
        {
            return View("MainPage");
        }
        public IActionResult Error()
        {
            return View("Error");
        }
    }
}

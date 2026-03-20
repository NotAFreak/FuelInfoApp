using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FuelInfoApp.Models;
using Microsoft.AspNetCore.Authorization;

namespace FuelInfoApp.Controllers;

public class HomeController : Controller {
    [AllowAnonymous]
    public IActionResult Index() {
        return View();
    }
    [AllowAnonymous]
    public IActionResult Privacy() {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

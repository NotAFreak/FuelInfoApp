using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FuelInfoApp.Models;
using FuelInfoApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace FuelInfoApp.Controllers;

public class HomeController : Controller {
    const uint maxViewableStations = 20;
    FuelDbContext db;

    public HomeController(FuelDbContext db) {
        this.db = db;
    }
    [AllowAnonymous]
    public async Task<IActionResult> Index() {
        MapViewModel vm = new();
        //vm.stations = await db.Stations
        //    .Include(o => o.Brand).ToListAsync();
        return View(vm);
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> UpdateMap(double minLatitude, double maxLatitude, double minLongitude, double maxLongitude, int zoom) {
        MapViewModel vm = new();
        List<Station> stations = await db.Stations.Where( o => 
            o.Latitude > (decimal)minLatitude && o.Latitude < (decimal)maxLatitude &&
            o.Longitude > (decimal)minLongitude && o.Longitude < (decimal)maxLongitude
        ).ToListAsync();
        
        if (stations.Count > maxViewableStations) {
            // stations.
        }
        return Json(vm);
    }
    [AllowAnonymous]
    public async Task<IActionResult> Privacy() {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

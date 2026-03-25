using System.Collections.Specialized;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using FuelInfoApp.Models;
using FuelInfoApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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
    public async Task<IActionResult> UpdateMap(double minLat, double maxLat, double minLon, double maxLon, int z) {
        MapViewModel vm = new();
        List<Station> stations = await db.Stations.Where( o => 
            o.Latitude > (decimal)minLat && o.Latitude < (decimal)maxLat &&
            o.Longitude > (decimal)minLon && o.Longitude < (decimal)maxLon
        ).ToListAsync();
        
        if (stations.Count > maxViewableStations) {
            Marker m = new() {
                latitude = (double)stations.Average(o=>o.Latitude),
                longitude = (double)stations.Average(o=>o.Longitude),
                stationCount = stations.Count
            };
            vm.markers.Add(m);
        }
        else {
            vm.stations = stations;
        }
        //Newton
        string jt = JsonConvert.SerializeObject(vm);
        return Json(jt);
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

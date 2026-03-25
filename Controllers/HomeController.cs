using System.Diagnostics;
using System.Numerics;
using Microsoft.AspNetCore.Mvc;
using FuelInfoApp.Models;
using FuelInfoApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FuelInfoApp.Controllers;

public class HomeController : Controller {
    const uint gridSize = 4;
    const uint maxViewableStations = 10;
    
    FuelDbContext db;

    public HomeController(FuelDbContext db) {
        this.db = db;
    }
    [AllowAnonymous]
    public async Task<IActionResult> Index() {
        MapViewModel vm = new();
        return View(vm);
    }
    
    // Requested through AJAX
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> UpdateMap(decimal minLat, decimal maxLat, decimal minLon, decimal maxLon, int z) {
        MapViewModel vm = new();
        List<Station> stations = await db.Stations.Where( o => 
            o.Latitude > (decimal)minLat && o.Latitude < (decimal)maxLat &&
            o.Longitude > (decimal)minLon && o.Longitude < (decimal)maxLon
        ).ToListAsync();
        
        // Divide the viewport into an X*X grid of cells, group based on those cells, and
        // the quantity of stations in each cell
        List<StationGroup> markers = new();
        decimal latSegment = (maxLat - minLat) / gridSize;
        decimal lonSegment = (maxLon - minLon) / gridSize;
        for (int x = 0; x < gridSize; x++) {
            for (int y = 0; y < gridSize; y++) {
                // Cell-specific coordinates
                decimal latStart = (minLat + (latSegment * x));
                decimal latEnd = (latStart + latSegment);
                decimal lonStart = (minLon + (lonSegment * y));
                decimal lonEnd = (lonStart + lonSegment);

                List<Station> s = stations.Where(o =>
                    o.Latitude >= latStart && o.Latitude < latEnd &&
                    o.Longitude >= lonStart && o.Longitude < lonEnd
                ).ToList();
                
                // If there are too many stations in that cell, group them
                int sCount = s.Count;
                if (sCount > maxViewableStations) {
                    StationGroup m = new() {
                        latitude = (double)s.Average(o => o.Latitude),
                        longitude = (double)s.Average(o => o.Longitude),
                        stationCount = sCount
                    };
                    markers.Add(m);
                } else {
                    // There aren't too many in that cell, so just add them straight to the map
                    vm.stations.AddRange(s);
                }
            }
        }

        vm.markers = markers;
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

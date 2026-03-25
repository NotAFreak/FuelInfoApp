using System.Collections.Specialized;
using System.Diagnostics;
using System.Numerics;
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
    const uint gridSize = 4;
    const uint maxViewableStations = 10;
    
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
    
    // Note this is squared
    public float dist(decimal latA, decimal lonA, decimal latB, decimal lonB) {
        Vector2 a = new((float)latA, (float)lonA);
        Vector2 b = new((float)latB, (float)lonB);
        return Vector2.DistanceSquared(a, b);
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> UpdateMap(double minLat, double maxLat, double minLon, double maxLon, int z) {
        MapViewModel vm = new();
        List<Station> stations = await db.Stations.Where( o => 
            o.Latitude > (decimal)minLat && o.Latitude < (decimal)maxLat &&
            o.Longitude > (decimal)minLon && o.Longitude < (decimal)maxLon
        ).ToListAsync();
        
        // Make a 5x5 grid based on the viewport. Seperate each station into segments of the grid
        // List<Station>[,] grid = new List<Station>[gridSize,gridSize];
        List<StationGroup> markers = new();
        double latSegment = (maxLat - minLat) / gridSize;
        double lonSegment = (maxLon - minLon) / gridSize;
        for (int x = 0; x < gridSize; x++) {
            for (int y = 0; y < gridSize; y++) {
                decimal minLa = (decimal)(minLat + (latSegment*x));
                decimal maxLa = (decimal)(maxLat - (latSegment*x));
                decimal minLo = (decimal)(minLon + (lonSegment*y));
                decimal maxLo = (decimal)(maxLon - (lonSegment*y));
                
                // Get every station just in the cell
                List<Station> s = stations.Where(o =>
                    o.Latitude > minLa && o.Latitude < maxLa &&
                    o.Longitude > minLo && o.Longitude < maxLo
                ).ToList();
                // If there are too many stations in that cell, group them
                if (s.Count > maxViewableStations) {
                    StationGroup m = new() {
                        latitude = (double)s.Average(o => o.Latitude),
                        longitude = (double)s.Average(o => o.Longitude),
                        stationCount = s.Count
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

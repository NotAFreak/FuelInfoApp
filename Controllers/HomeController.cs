using System.Diagnostics;
using System.Security.Claims;
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
        HomeViewModel vm = new() {
            fuels = await db.Fuels.ToListAsync()
        };
        Claim? idClaim = HttpContext.User.FindFirst("userid");
		Claim? passwordClaim = HttpContext.User.FindFirst("password");
		if (idClaim is null || passwordClaim is null) {
			TempData["Error"] = "Couldn't verify cookies!";
			return RedirectToAction("Login", "Account");
		}
		User? user = await db.Users.FirstOrDefaultAsync(o=>o.UserId == Convert.ToInt32(idClaim.Value));
        if (user is null) {
            TempData["Error"] = "Couldn't find user!";
            return RedirectToAction("Login", "Account");
        }
        if (user.Password != passwordClaim.Value) {
            user = null;
        }
        if (user is not null) {
            vm.cars = user.Cars.ToList();
        }
        return View(vm);
    }
    
    // Requested through AJAX only
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
                decimal latStart = minLat + (latSegment * x);
                decimal latEnd = latStart + latSegment;
                decimal lonStart = minLon + (lonSegment * y);
                decimal lonEnd = lonStart + lonSegment;

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
    [HttpGet]
    public async Task<IActionResult> SearchStations(string? stationName, decimal? latitude, decimal? longitude, int? carId, int? fuelId, int page = 0) {
        HomeSearchViewModel vm = new();
        if (stationName is null) {
            vm.stations = await db.Stations
                .Include(o=>o.StationFuelChanges)
                .ToListAsync();
        } else {
            vm.stations = await db.Stations
                .Include(o=>o.StationFuelChanges)
                .Where(o => o.Name.ToLower().Contains(stationName.ToLower()))
                .ToListAsync();
        }

        if (fuelId is not null) {
            foreach (Station s in vm.stations.ToList()) {
                s.StationFuelChanges = s.StationFuelChanges
                    .OrderBy(o => DateTime.FromBinary(BitConverter.ToInt64(o.ChangeTime)))
                    .ToList();
                StationFuelChange? change = s.StationFuelChanges.FirstOrDefault(o => o.FuelId == fuelId);
                if (change is null || change.IsAvailable == 0) {
                    vm.stations.Remove(s);
                }
            }
        }
        
        if (latitude is not null && longitude is not null) {
            vm.stations = vm.stations
                .OrderBy(o => 
                    Common.LatitudeDistance(o.Latitude, (decimal)latitude) + 
                    Common.LongitudeDistance(o.Longitude, (decimal)longitude)
                )
                .ToList();
        }
        return Json(JsonConvert.SerializeObject(vm));
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

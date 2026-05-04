using System.Globalization;
using System.Runtime.InteropServices.JavaScript;
using System.Text.RegularExpressions;
using FuelInfoApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace FuelInfoApp.ViewModels;

public class AdminUploadViewModel : ViewModel {
	public IFormFile? UploadFile { get; set; }

	DateTime TimeFromDataset(string input) {
		return DateTime.Parse(input, CultureInfo.GetCultureInfo("en-AU"));
	}

	public async Task AddRecord(FuelDbContext db, string[] record) {
		string Site_Name = record[1];
		string Site_Brand = record[2];
		string Sites_Address_Line_1 = record[3];
		string Site_Suburb = record[4];
		string Site_State = record[5].Trim('"');
		uint Site_Post_Code = Convert.ToUInt32(record[6]);
		double Site_Latitude = Convert.ToDouble(record[7]);
		double Site_Longitude = Convert.ToDouble(record[8]);
		string Fuel_Type = record[9].Trim();
		uint Price = Convert.ToUInt32(record[10]);
		DateTime TransactionDateutc = TimeFromDataset(record[11].Trim());
		
		// Check if fuel already exists in db. If not, add it.
		Fuel? fuel = await db.Fuels.FirstOrDefaultAsync(o=>o.FuelName == Fuel_Type);
		if (fuel is null) {
			fuel = new Fuel() {
				FuelName = Fuel_Type,
			};
			
			db.Fuels.Add(fuel);
		}
		
		// Check if the station already exists in the db. If not, add it
		Station? station = await db.Stations.FirstOrDefaultAsync(o => o.Name == Site_Name);
		if (station is null) {
			Brand? brand = await db.Brands.FirstOrDefaultAsync(o => o.BrandName == Site_Brand);
			// Check if brand already exists in db. If not, add it
			if (brand is null) {
				brand = new Brand() {
					BrandName = Site_Brand
				};
				// await db.Brands.AddAsync(brand);
			}
			// Now make the station
			station = new() {
				Name = Site_Name,
				Address = Sites_Address_Line_1,
				Postcode = (int)Site_Post_Code,
				Suburb = Site_Suburb,
				State = Site_State,
				Latitude = Site_Latitude,
				Longitude = Site_Longitude,
				BrandId = brand.BrandId
			};
			await db.Stations.AddAsync(station);
			// await db.SaveChangesAsync();
		}
		
		// Check if we have a state for this time for this station at this change's time
		StationFuelState? state = await db.StationFuelStates
			.Where(o => o.StationId == station.StationId)
			.Where(o=>o.ChangeTime == BitConverter.GetBytes(TransactionDateutc.ToBinary()))
			.Include(o=>o.StationFuelStateFuels)
			.FirstOrDefaultAsync();
		// If not, create and add it
		if (state is null) {
			state = new() {
				ChangeTime = BitConverter.GetBytes(TransactionDateutc.ToBinary()),
				StationId = station.StationId
			};
			await db.StationFuelStates.AddAsync(state);
			// await db.SaveChangesAsync();
		}
		
		// Check if the fuel is on the state
		StationFuelStateFuel? existingFuel = state.StationFuelStateFuels
			.FirstOrDefault(o => o.FuelId == fuel.FuelId);
		
		// If the state doesn't have the fuel, add it
		if (existingFuel is null) {
			state.StationFuelStateFuels.Add(new() {
				StateId = state.StateId,
				FuelId = fuel.FuelId,
				IsAvailable = 1,
				Price = (int)Price,
			});
			// await db.SaveChangesAsync();
		}
	}
	public async Task UploadData(FuelDbContext db, string filePath) {
		// 63533 records
		string[] records = await File.ReadAllLinesAsync(filePath);
		int progress = 0;
		int max = records.Skip(1).Count();
		DateTime startTime = DateTime.UtcNow;
		DateTime recordStartTime = DateTime.UtcNow;
		foreach (string row in records.Skip(1)) {
			startTime = DateTime.UtcNow;
			// Regex for splitting .csv data
			string[] record = Regex.Split(row, ",(?=(?:[^\\\"]*\\\"[^\\\"]*\\\")*[^\\\"]*$)");
			await AddRecord(db, record);
			await db.SaveChangesAsync();
			progress++;
			// sanity
			Console.WriteLine("Seeding progress: " + ((float)progress / (float)max)*100 + "% (" + progress + " / " + max + ")");
			TimeSpan remaining = DateTime.UtcNow.Subtract(startTime) * (max - progress);
			Console.WriteLine($"Estimated time left: {(int)remaining.TotalHours} hours, {remaining.Minutes} minutes, {remaining.Seconds} seconds");
		}
		await db.SaveChangesAsync();
	}
}
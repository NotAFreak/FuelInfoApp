using System.Text.RegularExpressions;
using FuelInfoApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace FuelInfoApp.ViewModels;

public class AdminUploadViewModel : ViewModel {
	public IFormFile? UploadFile { get; set; }

	public async Task UploadData(FuelDbContext db, string filePath) {
		// 63533 records
		string[] records = await File.ReadAllLinesAsync(filePath);
		int progress = 0;
		int max = records.Skip(1).Count();
		foreach (string row in records.Skip(1)) {
			// Regex for splitting .csv data
			string[] record = Regex.Split(row, ",(?=(?:[^\\\"]*\\\"[^\\\"]*\\\")*[^\\\"]*$)");

			string SiteId = record[0].Trim('"');
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
			string TransactionDateutc = record[11].Trim();
			
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
					await db.Brands.AddAsync(brand);
				}
				// Now make the station
				station = new() {
					Name = Site_Name,
					Address = Sites_Address_Line_1,
					Postcode = (int)Site_Post_Code,
					Suburb = Site_Suburb,
					State = Site_State,
					Latitude = (decimal)Site_Latitude,
					Longitude = (decimal)Site_Longitude,
					BrandId = brand.BrandId
				};
				await db.Stations.AddAsync(station);
			}
			progress++;
			
			/*StationFuelChange? change = db.StationFuelChanges
				.Where(o=>o.StationId == station.StationId)
				.Where(o=>o.FuelId )
				*/
		}
		await db.SaveChangesAsync();
	}
}
#nullable enable
using FuelInfoApp.Models;

public class StationFuelGraph {
	public List<string> fuels = new();
	public List<List<StationFuelState>> prices = new();
}

public enum GraphType : byte {
	Price = 0
}

public enum GraphDuration : byte {
	Week = 0,
	Month = 1,
	Year = 2
}
public class StationFuelState {
	public string changeTime = "yyyy-mm-dd-hh";
	public int fuelPrice;
	
	
}


using FuelInfoApp.Models;

namespace FuelInfoApp.ViewModels;

public class Marker {
	public double latitude;
	public double longitude;
	public int stationCount = 0;
}
public class MapViewModel : ViewModel {
	public List<Marker> points = new();
	public List<Station> stations = new();
}
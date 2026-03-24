using FuelInfoApp.Models;

namespace FuelInfoApp.ViewModels;

public class Point {
	double latitude;
	double longitude;
	int stationCount = 0;
}
public class MapViewModel : ViewModel {
	public List<Point> points = new();
	public List<Station> stations = new();
}
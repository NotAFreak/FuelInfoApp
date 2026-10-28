using FuelInfoApp.Models;

namespace FuelInfoApp.ViewModels;

public class StationInfo {
	public Station station;
	public List<Fuel> fuel = new();
} 
public class HomeSearchViewModel : ViewModel {
	public List<Station> stations = new();
}
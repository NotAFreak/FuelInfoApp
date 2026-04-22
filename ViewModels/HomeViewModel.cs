using FuelInfoApp.Models;

namespace FuelInfoApp.ViewModels;

public class HomeViewModel : ViewModel {
	public List<Car> cars = new();
	public List<Fuel> fuels = new();
}
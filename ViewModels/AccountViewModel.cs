using FuelInfoApp.Models;

namespace FuelInfoApp.ViewModels;

public class AccountViewModel : ViewModel {
	public User user;
	public List<Fuel> fuels;
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FuelInfoApp.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller {
	public AdminController() {
		
	}
	public async Task<IActionResult> Index() {
		return View();
	}
	public async Task<IActionResult> User() {
		return View();
	}
	public async Task<IActionResult> Fuel() {
		return View();
	}
	public async Task<IActionResult> Station() {
		return View();
	}
	public async Task<IActionResult> Brand() {
		return View();
	}
}
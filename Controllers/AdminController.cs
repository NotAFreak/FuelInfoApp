using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FuelInfoApp.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller {
	public AdminController() { }
	[HttpGet]
	public async Task<IActionResult> Index() {
		return View();
	}
	[HttpGet]
	public async Task<IActionResult> Upload() {
		return View();
	}
	[HttpGet]
	public async Task<IActionResult> User() {
		return View();
	}
	[HttpGet]
	public async Task<IActionResult> Fuel() {
		return View();
	}
	[HttpGet]
	public async Task<IActionResult> Station() {
		return View();
	}
	[HttpGet]
	public async Task<IActionResult> Brand() {
		return View();
	}
	
	// Post actions
	[HttpPost]
	public async Task<IActionResult> UploadPost(IFormFile file) {
		var filePath = Path.GetTempFileName();
		using (var stream = System.IO.File.Create(filePath)) {
			await file.CopyToAsync(stream);
		}
		return RedirectToAction("Upload", "Admin");
	}
}
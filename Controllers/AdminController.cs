using FuelInfoApp.Models;
using FuelInfoApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FuelInfoApp.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller {
	IWebHostEnvironment webHostEnvironment;
	FuelDbContext db;

	public AdminController(IWebHostEnvironment env, FuelDbContext db) {
		this.db = db;
		this.webHostEnvironment = env;
	}
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
	public async Task<IActionResult> UploadPost(AdminUploadViewModel vm) {
		if (vm.UploadFile is null) {
			return RedirectToAction("Upload", "Admin");
		}

		var webRootPath = webHostEnvironment.WebRootPath;
		string uploadFolder = Path.Combine(webRootPath, "Uploads");
		string fileName = DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss") + "_" + Path.GetFileName(vm.UploadFile.FileName);
		string filePath = Path.Combine(uploadFolder, fileName);
		using (var stream = new FileStream(filePath, FileMode.Create)) {
			await vm.UploadFile.CopyToAsync(stream);
		}

		try {
			await vm.UploadData(db, filePath);
		}
		catch (Exception ex) {
			if (System.IO.File.Exists(filePath)) {
				System.IO.File.Delete(filePath);
			}
			return new StatusCodeResult(400);
		}
		return RedirectToAction("Upload", "Admin");
	}
}
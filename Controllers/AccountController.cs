using System.Diagnostics;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using FuelInfoApp.Models;
using FuelInfoApp.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace FuelInfoApp.Controllers;
public enum Role : int {
	User = 0,
	Admin = 1
}
public class AccountController : Controller {
	FuelDbContext db;
	public AccountController(FuelDbContext db) {
		this.db = db;
	}
	
	public string HashPassword(string password) {
		string encryptionSalt = "jvnko salty";
		byte[] salt = Encoding.UTF8.GetBytes(encryptionSalt);
		string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
			password, salt,
			KeyDerivationPrf.HMACSHA512,
			69420,
			256 / 8
		));
		return hashed;
	}
	public async Task AssignCookie(User user) {
		List<Claim> claims = new () {
			new ("userid", user.UserId.ToString()),
			new ("password", user.Password),
			new (ClaimTypes.Email, user.Email),
			new (ClaimTypes.Role, ((Role)user.Role).ToString()),
		};

		ClaimsIdentity claimIdentities = new (claims, CookieAuthenticationDefaults.AuthenticationScheme);
		var claimPrinciples = new ClaimsPrincipal(claimIdentities);
		var authenticationManager = Request.HttpContext;
		authenticationManager.User = claimPrinciples;

		await HttpContext.SignInAsync(
			CookieAuthenticationDefaults.AuthenticationScheme,
			new ClaimsPrincipal(claimIdentities)
		);
	}
	
	[HttpGet]
	[Authorize]
	public async Task<ActionResult> Index() {
		AccountViewModel vm = new();
		Claim? idClaim = HttpContext.User.FindFirst("userid");
		if (idClaim is null) {
			return RedirectToAction("Login", "Account");
		}
		User? user = await db.Users
			.Include(o => o.Cars)
			.ThenInclude(o=>o.Fuel)
			.FirstOrDefaultAsync(o=>o.UserId == Convert.ToInt32(idClaim.Value));
		if (user is null) {
			return RedirectToAction("Signup", "Account");
		}
		vm.user = user;
		vm.fuels = await db.Fuels.ToListAsync();
		return View(vm);
	}
	[HttpGet]
	[AllowAnonymous]
	public async Task<IActionResult> Login() {
		return View();
	}
	[HttpGet]
	[AllowAnonymous]
	public async Task<IActionResult> Signup() {
		return View(new ViewModel());
	}

	[HttpPost]
	[AllowAnonymous]
	public async Task<IActionResult> LoginPost(string email, string password) {
		if (string.IsNullOrEmpty(email.Trim())) {
			TempData["Error"] = "Email required!";
			return RedirectToAction("Login", "Account");	
		}
		if (string.IsNullOrEmpty(password.Trim())) {
			TempData["Error"] = "Password required!";
			return RedirectToAction("Login", "Account");
		}
		User? user = await db.Users.FirstOrDefaultAsync(o => o.Email == email);
		if (user is null) {
			TempData["Error"] = "User does not exist!";
			return RedirectToAction("Login", "Account");
		}
		if (user.Password != HashPassword(password)) {
			TempData["Error"] = "Password not correct!";
			return RedirectToAction("Login", "Account");
		}
		await AssignCookie(user);
		return RedirectToAction("Index", "Home");
	}
	[HttpPost]
	[AllowAnonymous]
	public async Task<IActionResult> SignupPost(string email, string password, string firstname, string lastname, string mobile, int postcode) {
		if (string.IsNullOrEmpty(email.Trim())) {
			TempData["Error"] = "Email required!";
			return RedirectToAction("Signup", "Account");
		}
		if (string.IsNullOrEmpty(password.Trim())) {
			TempData["Error"] = "Password required!";
			return RedirectToAction("Signup", "Account");
		}
		if (!Regex.IsMatch(email, "^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$")) {
			TempData["Error"] = "Email not valid!";
			return RedirectToAction("Signup", "Account");
		}
		if (password.Length < 6) {
			TempData["Error"] = "Password needs to be 6 or more characters long!";
			return RedirectToAction("Signup", "Account");
		}
		User? user = await db.Users.FirstOrDefaultAsync(o => o.Email == email);
		if (user != null) {
			TempData["Error"] = "User with that Email already exists!";
			return RedirectToAction("Signup", "Account");
		}
		User newUser = new() {
			Email = email,
			Password = HashPassword(password),
			FirstName = firstname,
			LastName = lastname,
			MobileNumber = mobile,
			Postcode = postcode
		};
		db.Users.Add(newUser);
		await AssignCookie(newUser);
		await db.SaveChangesAsync();
		return RedirectToAction("Index", "Home");
	}
	
	public async Task<IActionResult> UpdateInfo(
		string? email, string? password, string? firstname, string? lastname, 
		string? mobile, int? postcode, double? latitude, double longitude
		) {
		
		Claim? idClaim = HttpContext.User.FindFirst("userid");
		Claim? passwordClaim = HttpContext.User.FindFirst("password");
		if (idClaim is null || passwordClaim is null) {
			TempData["Error"] = "Couldn't verify cookies!";
			return RedirectToAction("Login", "Account");
		}
		
		User? user = await db.Users .FirstOrDefaultAsync(o=>o.UserId == Convert.ToInt32(idClaim.Value));
		if (user is null) {
			TempData["Error"] = "User does not exist!";
			return RedirectToAction("Signup", "Account");
		}
		// The user is actually correct, now update the information we want to
		if (email is not null) {
			if (!Regex.IsMatch(email, "^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$")) {
				TempData["Error"] = "Email not valid!";
				return RedirectToAction("Index", "Account");
			}

			user.Email = email;
		}

		if (password is not null) {
			if (password.Length < 6) {
				TempData["Error"] = "Password needs to be 6 or more characters long!";
				return RedirectToAction("Index", "Account");
			}
			user.Password = HashPassword(password);
		}

		if (firstname is not null) {
			user.FirstName = firstname;
		}

		if (lastname is not null) {
			user.LastName = lastname;
		}

		if (mobile is not null) {
			user.MobileNumber = mobile;
		}

		if (latitude is not null) {
			// user.Latitude = latitude;
		}
		
		await AssignCookie(user);
		
		await db.SaveChangesAsync();
		return RedirectToAction("Index", "Home");
	}

	[HttpPost]
	public async Task<IActionResult> AddCar(string name, string? registration, int fuelId, float fuelConsumption) {
		// Verify the user is the user
		Claim? idClaim = HttpContext.User.FindFirst("userid");
		Claim? passwordClaim = HttpContext.User.FindFirst("password");
		if (idClaim is null || passwordClaim is null) {
			TempData["Error"] = "Couldn't verify cookies!";
			return RedirectToAction("Login", "Account");
		}
		User? user = await db.Users .FirstOrDefaultAsync(o=>o.UserId == Convert.ToInt32(idClaim.Value));
		if (user is null) {
			TempData["Error"] = "User does not exist!";
			return RedirectToAction("Signup", "Account");
		}

		if (string.IsNullOrEmpty(name)) {
			TempData["Error"] = "Car name required!";
			return RedirectToAction("Index", "Account");
		}
		
		// Create and add the car
		Car car = new() {
			Name = name,
			Registration = registration,
			FuelConsumption = (decimal)fuelConsumption,
			FuelId = fuelId,
			UserId = user.UserId
		};
		await db.Cars.AddAsync(car);
		await db.SaveChangesAsync();
		return RedirectToAction("Index", "Account");
	}
	[HttpPost]
	public async Task<IActionResult> EditCar(int carId, string name, string? registration, int fuelId, float 
		fuelConsumption) {
		// Verify the user is the user
		Claim? idClaim = HttpContext.User.FindFirst("userid");
		Claim? passwordClaim = HttpContext.User.FindFirst("password");
		if (idClaim is null || passwordClaim is null) {
			TempData["Error"] = "Couldn't verify cookies!";
			return RedirectToAction("Login", "Account");
		}
		User? user = await db.Users .FirstOrDefaultAsync(o=>o.UserId == Convert.ToInt32(idClaim.Value));
		if (user is null) {
			TempData["Error"] = "User does not exist!";
			return RedirectToAction("Signup", "Account");
		}

		if (string.IsNullOrEmpty(name)) {
			TempData["Error"] = "Car name required!";
			return RedirectToAction("Index", "Account");
		}
		
		// Get and edit the car
		Car? car = await db.Cars.FirstOrDefaultAsync(o => o.CarId == carId);
		if (car is null) {
			TempData["Error"] = "Car does not exist!";
			return RedirectToAction("Index", "Account");
		}
		if (car.UserId != user.UserId) {
			TempData["Error"] = "Car not owned by user!";
			return RedirectToAction("Index", "Account");
		}

		car.Name = name;
		car.Registration = registration;
		car.FuelId = fuelId;
		car.FuelConsumption = (decimal)fuelConsumption;
		await db.SaveChangesAsync();
		return RedirectToAction("Index", "Account");
	}
	[HttpPost]
	public async Task<IActionResult> DeleteCar(int carId) {
		// Verify the user is the user
		Claim? idClaim = HttpContext.User.FindFirst("userid");
		Claim? passwordClaim = HttpContext.User.FindFirst("password");
		if (idClaim is null || passwordClaim is null) {
			TempData["Error"] = "Couldn't verify cookies!";
			return RedirectToAction("Login", "Account");
		}
		User? user = await db.Users .FirstOrDefaultAsync(o=>o.UserId == Convert.ToInt32(idClaim.Value));
		if (user is null) {
			TempData["Error"] = "User does not exist!";
			return RedirectToAction("Signup", "Account");
		}

		Car? car = await db.Cars.FirstOrDefaultAsync(o=>o.CarId == carId);
		if (car is null) {
			TempData["Error"] = "Car does not exist!";
			return RedirectToAction("Index", "Account");
		}

		if (car.UserId != user.UserId) {
			TempData["Error"] = "Car not owned by user!";
			return RedirectToAction("Index", "Account");
		}

		db.Cars.Remove(car);
		await db.SaveChangesAsync();
		return RedirectToAction("Index", "Account");
	} 
}
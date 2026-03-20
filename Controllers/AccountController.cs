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
			new (ClaimTypes.Role, user.Role.ToString()),
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
	[Authorize(Roles = "User")]
	public async Task<ActionResult> Index() {
		return View(new ViewModel());
	}
	[HttpGet]
	[AllowAnonymous]
	public async Task<IActionResult> Login() {
		return View(new ViewModel());
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
		AssignCookie(newUser);
		await db.SaveChangesAsync();
		return RedirectToAction("Index", "Home");
	}
	
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error() {
		return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
	}
}
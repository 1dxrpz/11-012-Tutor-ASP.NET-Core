using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using tutor1.Models;

namespace tutor1.Controllers
{
	public class HomeController : Controller
	{
		ApplicationContext db;

		public HomeController(ApplicationContext context)
		{
			db = context;
		}

		public IActionResult Account(string id)
		{
			var model = new User();
			model = db.Users.FirstOrDefault<User>(x => x.Id.ToString() == id);
			return View(model);
		}
		public IActionResult Index()
		{
			var model = new User();
			if (User.Identity.IsAuthenticated)
			{
				var id = User.Claims
				   .First(x => x.Type.Equals(ClaimTypes.NameIdentifier))
				   .Value;

				model = db.Users.FirstOrDefault<User>(x => x.Id.ToString() == id);
			}
			return View(model);
		}
		public IActionResult Privacy()
		{
			return View();
		}
		[HttpGet]
		public IActionResult Login()
		{
			return View();
		}
		[HttpGet]
		public IActionResult Register()
		{
			return View();
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(UserViewModel model)
		{
			if (ModelState.IsValid)
			{
				User user = await db.Users.FirstOrDefaultAsync(u => u.Username == model.Username);
				if (user == null)
				{
					var currentUser = new User
					{
						Password = model.Password,
						Username = model.Username
					};
					db.Users.Add(currentUser);
					await db.SaveChangesAsync();

					await Authenticate(currentUser);

					return RedirectToAction("Index", "Home");
				}
				else
					ModelState.AddModelError("", "user already exists");
			}
			return View(model);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(UserViewModel model)
		{
			if (ModelState.IsValid)
			{
				User user = await db.Users.FirstOrDefaultAsync(u => u.Username == model.Username && u.Password == model.Password);

				if (user != null)
				{
					await Authenticate(user);
					ViewBag.User = user;
					return RedirectToAction("Index", "Home");
				}
				ModelState.AddModelError("", "Некорректные логин и(или) пароль");
			}
			return View(model);
		}
		private async Task Authenticate(User model)
		{
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, model.Username),
				new Claim(ClaimTypes.NameIdentifier, model.Id.ToString())
			};
			ClaimsIdentity id = new ClaimsIdentity(
				claims, "ApplicationCookie",
				ClaimsIdentity.DefaultNameClaimType,
				ClaimsIdentity.DefaultRoleClaimType
			);
			await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
		}
		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return RedirectToAction("Login", "Home");
		}
	}
}
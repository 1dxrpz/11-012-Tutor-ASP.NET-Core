using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tutor1.Models;

namespace tutor1.Controllers
{
	public class HomeController : Controller
	{
		public IActionResult Index()
		{
			User user = new User()
			{
				Id = 0,
				Password = "test",
				Userame = "test"
			};

			return View(user);
		}
		public IActionResult Privacy()
		{
			return View();
		}
	}
}
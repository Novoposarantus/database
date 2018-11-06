using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DataBase.Models;
using DataBase.Database;

namespace DataBase.Controllers
{
	public class HomeController : Controller
	{
		IStorage storage;
		public HomeController(IStorage storage)
		{
			this.storage = storage;
		}
		public /*IActionResult*/string Index()
		{
			var users = storage.GetUsersList();
			return "Hello World";
		}
	}
}

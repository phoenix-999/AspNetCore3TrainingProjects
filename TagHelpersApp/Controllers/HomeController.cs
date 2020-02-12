using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace TagHelpersApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index(int? id)
        {
            ViewBag.Id = id == null ? 1 : ++id;
            return View();
        }
    }
}
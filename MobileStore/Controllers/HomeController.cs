using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MobileStore.Models;

namespace MobileStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MobileContext _db;

        public HomeController(ILogger<HomeController> logger, MobileContext context)
        {
            _logger = logger;
            _db = context;
        }

        public IActionResult Index()
        {
            return View(_db.Phones.ToList());
        }

        [HttpGet]
        public IActionResult Buy(string id)
        {
            if (id == null)
                return RedirectToAction(nameof(Index));
            ViewBag.PhoneId = id;
            return View();
        }

        [HttpPost]
        public string Buy(Order order)
        {
            if(ModelState.IsValid)
            {
                _db.Orders.Add(order);
                _db.SaveChanges();
                return "Спасибо, " + order.User + ", за покупку!";
            }
            return "Произошла ошибка";
        }

    }
}

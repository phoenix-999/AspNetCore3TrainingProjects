using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TryAddEnumerableExample.Models;

namespace TryAddEnumerableExample.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISomeService _someService;

        public HomeController(ILogger<HomeController> logger, ISomeService someService)
        {
            _logger = logger;
            _someService = someService;
        }

        public IActionResult Index()
        {
            return View("SomeServiceInvokeResult", _someService.Invoke());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

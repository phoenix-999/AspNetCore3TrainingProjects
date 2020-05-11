using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using LogsAndStrategy.Models;
using LogsAndStrategy.StorageRepositories;

namespace LogsAndStrategy.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IItemRepository _itemRepository;

        public HomeController(ILogger<HomeController> logger, IItemRepository itemRepository)
        {
            _logger = logger;
            _itemRepository = itemRepository;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _itemRepository.GetAll());
        }

        [HttpPost]
        public async Task<IActionResult> AddItem()
        {
            await _itemRepository.AddItem(Guid.NewGuid().ToString());
            return RedirectToAction(nameof(Index));
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

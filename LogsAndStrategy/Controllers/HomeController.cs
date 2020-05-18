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
            await _itemRepository.AddItems(new Item("Item 1"), new Item("Item 2"));


            return View(await _itemRepository.GetAll());
        }

        [HttpPost]
        public async Task<IActionResult> PostItem(string itemName)
        {
            await _itemRepository.AddItem(itemName);
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public async Task<IActionResult> PostTag(string label, string itemName)
        {
            await _itemRepository.AddTag(label, itemName);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTag(string itemName)
        {
            await _itemRepository.RemoveItem(itemName);
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

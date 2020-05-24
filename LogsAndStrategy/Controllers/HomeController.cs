using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using LogsAndStrategy.Models;
using LogsAndStrategy.StorageRepositories;
using System.Transactions;
using System.Data.SQLite;

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

        void TransactionTest()
        {
            Blog blog = new Blog { BlogName = "Blog 1" };
            #region Sqlite
            if (!System.IO.File.Exists(@"C:\TestDB.db"))
            {
                SQLiteConnection.CreateFile(@"C:\TestDB.db");
            }

            using (SQLiteConnection Connect = new SQLiteConnection(@"Data Source=C:\TestDB.db; Version=3;")) // в строке указывается к какой базе подключаемся
            {
                // строка запроса, который надо будет выполнить
                string commandText = "Drop TABLE IF EXISTS [dbTableName]";
                SQLiteCommand Command = new SQLiteCommand(commandText, Connect);
                Connect.Open(); // открыть соединение
                Command.ExecuteNonQuery(); // выполнить запрос
                Connect.Close();
            }

            using (SQLiteConnection Connect = new SQLiteConnection(@"Data Source=C:\TestDB.db; Version=3;")) // в строке указывается к какой базе подключаемся
            {
                // строка запроса, который надо будет выполнить
                string commandText = "CREATE TABLE IF NOT EXISTS [dbTableName] ( [id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, [str] VARCHAR(10) )";
                SQLiteCommand Command = new SQLiteCommand(commandText, Connect);
                Connect.Open(); // открыть соединение
                Command.ExecuteNonQuery(); // выполнить запрос
                Connect.Close();
            }

            #endregion
            try
            {
                using (var transaction = new TransactionScope())
                {

                    using (var ctx = new AppDbContext())
                    {
                        ctx.Blogs.Add(blog);
                        ctx.SaveChanges();
                    }

                    #region SqLite

                    using (SQLiteConnection Connect = new SQLiteConnection(@"Data Source=C:\TestDB.db; Version=3;")) // в строке указывается к какой базе подключаемся
                    {
                        string commandText = "insert into [dbTableName] ([str]) values ('test')";
                        SQLiteCommand Command = new SQLiteCommand(commandText, Connect);
                        Connect.Open(); // открыть соединение
                        Command.ExecuteNonQuery();
                        Connect.Close(); // закрыть соединение
                    }

                    #endregion

                    throw new InvalidCastException();
                    transaction.Complete();
                }
            }
            catch (InvalidCastException) { }
        }

        public async Task<IActionResult> Index()
        {
            //TransactionTest();
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

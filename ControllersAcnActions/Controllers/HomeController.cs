using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ControllersAcnActions.Controllers
{
    public class HomeController : Controller
    {
        IWebHostEnvironment appEnv;

        public HomeController(IWebHostEnvironment environment)
        {
            this.appEnv = environment;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetPhysicalFile()
        {
            //Передает файл целиком по адресу относительно корня жесткого дика
            string filePath = Path.Combine(appEnv.ContentRootPath, "Files", "Xamarin.pdf");
            string contentType = "application/pdf"; //application/octet-stream если формат файла заранее не известен
            return PhysicalFile(filePath, contentType, "Xamarin.pdf"); //contentType - обязательный параметр
        }

        public IActionResult GetBytes()
        {
            //Читает байты с файла и передает их
            string filePath = Path.Combine(appEnv.ContentRootPath, "Files", "Xamarin.pdf");
            byte[] bytes = System.IO.File.ReadAllBytes(filePath);
            string contentType = "application/pdf";//application/octet-stream если формат файла заранее не известен
            return File(bytes, contentType, "Xamarin.pdf");//contentType - обязательный параметр
        }

        public IActionResult GetFileStream()
        {
            //Открывает файл и передает stream
            string filePath = Path.Combine(appEnv.ContentRootPath, "Files", "Xamarin.pdf");
            FileStream fs = new FileStream(filePath, FileMode.Open);
            string contentType = "application/pdf";//application/octet-stream если формат файла заранее не известен
            return File(fs, contentType, "Xamarin.pdf");//contentType - обязательный параметр
        }

        public IActionResult GetVirtualFile()
        {
            //Передает файл целиком по адресу относительно wwwroot
            string filePath = Path.Combine("~Files", "file.txt"); //wwwroot/Files/file.txt
            string contentType = "text/plain";//application/octet-stream если формат файла заранее не известен
            return File(filePath, contentType, "file.txt");//contentType - обязательный параметр
        }

        [NonAction]
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if(context.HttpContext.Request.Headers.ContainsKey("User-Agent"))
            {
                string browser = context.HttpContext.Request.Headers["User-Agent"];
                if(browser.Contains("MSIE") || browser.Contains("Trident"))
                {
                    context.Result = Content("Internet Explorer не поддреживается");
                }
            }
            base.OnActionExecuting(context);
        }

        public IActionResult Fallback()
        {
            return Content($"Путь {Request.Path} не существует.");
        }
    }
}
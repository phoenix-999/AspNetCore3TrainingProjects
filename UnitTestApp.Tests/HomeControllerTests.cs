using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using UnitTestApp.Controllers;
using Xunit;

namespace UnitTestApp.Tests
{
    public class HomeControllerTests
    {
        [Fact]
        public void IndexViewDataMessage()
        {
            var homeController = new HomeController();
            ViewResult result = homeController.Index() as ViewResult;
            Assert.Equal("Hello", result?.ViewData["Message"]);
        }

        [Fact]
        public void IndexViewResultNotNull()
        {
            var homeController = new HomeController();
            ViewResult result = homeController.Index() as ViewResult;
            Assert.NotNull(result);
        }

        [Fact]
        public void IndexViewNameEqualIndex()
        {
            var homeController = new HomeController();
            ViewResult result = homeController.Index() as ViewResult;
            Assert.Equal("Index", result?.ViewName);
        }
    }
}

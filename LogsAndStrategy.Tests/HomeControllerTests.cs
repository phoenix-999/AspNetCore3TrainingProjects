using LogsAndStrategy.Controllers;
using LogsAndStrategy.Models;
using LogsAndStrategy.StorageRepositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace LogsAndStrategy.Tests
{
    public class HomeControllerTests
    {
        [Fact]
        public async void IndexGetModelItems()
        {
            using var ctx = new AppContextTest(true);
            var itemRepository = new ItemRepositoty(ctx, null);
            var controller = new HomeController(null, itemRepository);
            var result = await controller.Index() as ViewResult;
            var model = result.Model;
            Assert.NotNull(result);
            Assert.IsAssignableFrom<IEnumerable<Item>>(model);
            Assert.True(((IEnumerable<Item>)model).Count() > 0);
        }
    }
}

using LogsAndStrategy.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace LogsAndStrategy.Tests
{
    public class OrderTests
    {
        [Fact]
        public void HasEqualGeneratedId()
        {
            var order = new Order
            {
                Status = "Status 1",//Ручная установка одинакового значения в оба экземпляра обязательна
                OrderDetail = new OrderDetail { Address = "Address 1", Status = "Status 1" }
            };

            using(var ctx = new AppContextTest(true))
            {
                ctx.Orders.Add(order);
                ctx.SaveChanges();
            }

            Assert.Equal(order.Id, order.OrderDetail.Id);
            Assert.Equal(order.Status, order.OrderDetail.Status);
        }

        [Fact]
        public void CanNotAutoIncludeInjectEnity()
        {
            var order = new Order
            {
                Status = "Status 1",//Ручная установка одинакового значения в оба экземпляра обязательна
                OrderDetail = new OrderDetail { Address = "Address 1", Status = "Status 1" }
            };

            using (var ctx = new AppContextTest(true))
            {
                ctx.Orders.Add(order);
                ctx.SaveChanges();
            }

            Order savedOrder = null;
            using (var ctx = new AppContextTest())
            {
                savedOrder = ctx.Orders.Where(o => o.Id == order.Id).Single();
            }

            Assert.Null(savedOrder.OrderDetail);

            using (var ctx = new AppContextTest())
            {
                savedOrder = ctx.Orders.Include(o => o.OrderDetail).Where(o => o.Id == order.Id).Single();
            }

            Assert.NotNull(savedOrder.OrderDetail);
            Assert.Equal("Address 1", savedOrder.OrderDetail.Address);
        }
    }
}

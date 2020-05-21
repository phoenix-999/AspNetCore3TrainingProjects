using LogsAndStrategy.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace LogsAndStrategy.Tests
{
    public class DeliveryTests
    {
        [Fact]
        public void CanChangeOwnedPropety()
        {
            var delivery1 = new Delivery();

            var a1 = new Address { Street = "Street 1" };
            var a2 = new Address { Street = "Street 2" };
            int a1Id = 0;
            int a2Id = 0;

            var addresses = new List<Address> { a1, a2};
            delivery1.Addresses = addresses;

            using(var ctx = new AppContextTest(true))
            {
                ctx.Deliveries.Add(delivery1);
                ctx.SaveChanges();
                a1Id = (int)ctx.Entry(delivery1.Addresses[0]).Property("Id").CurrentValue;
                a2Id = (int)ctx.Entry(delivery1.Addresses[1]).Property("Id").CurrentValue;
            }

            Assert.NotEqual(default, delivery1.DeliveryId);

            Delivery delivery2 = new Delivery();
            var a3 = new Address { Street = "Street 3" };
            using (var ctx  = new AppContextTest())
            {
                var savedDelivery = ctx.Deliveries.Include(d => d.Addresses).Where(d => d.DeliveryId == delivery1.DeliveryId).Single();
                delivery2.Addresses = savedDelivery.Addresses;
                savedDelivery.Addresses = new List<Address> { new Address { Street = "Street 3" } };
                ctx.Deliveries.Add(delivery2);
                ctx.SaveChanges();
            }

            Assert.NotNull(delivery2);

            using (var ctx = new AppContextTest())
            {
                //Include не надо - собственные типы включютс по умолчанию
                var d1 = ctx.Deliveries./*Include(d => d.Addresses).*/Where(d => d.DeliveryId == delivery1.DeliveryId).Single();
                var d2 = ctx.Deliveries./*Include(d => d.Addresses).*/Where(d => d.DeliveryId == delivery2.DeliveryId).Single();

                Assert.Single(d1.Addresses);
                Assert.Equal(2, d2.Addresses.Count());

                Assert.Equal(a3.Street, d1.Addresses[0].Street);
                Assert.Equal(a1.Street, d2.Addresses[0].Street);
                Assert.Equal(a2.Street, d2.Addresses[1].Street);

                Assert.Equal(a1Id, ctx.Entry(d2.Addresses[0]).Property("Id").CurrentValue);
                Assert.Equal(a2Id, ctx.Entry(d2.Addresses[1]).Property("Id").CurrentValue);
            }
        }
    }
}

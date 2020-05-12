using LogsAndStrategy.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogsAndStrategy.Tests
{
    public class AppContextTest : AppDbContext
    {
        public AppContextTest(DbContextOptions<AppDbContext> options, bool seed)
            :base(options)
        {
            if(seed)
                Seed();
        }

        public AppContextTest(bool seed=false)
            : this(new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer("Data Source =.; Initial Catalog = Tutorials.Tests; Integrated Security = true")
                .Options, seed) {}

        private void Seed()
        {
            using (var context = new AppContextTest())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                var one = new Item("ItemOne");
                one.AddTag("Tag11");
                one.AddTag("Tag12");
                one.AddTag("Tag13");

                var two = new Item("ItemTwo");

                var three = new Item("ItemThree");
                three.AddTag("Tag31");
                three.AddTag("Tag31");
                three.AddTag("Tag31");
                three.AddTag("Tag32");
                three.AddTag("Tag32");

                context.AddRange(one, two, three);

                context.SaveChanges();
            }
        }

    }
}

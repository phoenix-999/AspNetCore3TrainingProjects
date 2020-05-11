using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace LogsAndStrategy.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            :base(options)
        {
            Seek();
        }

        public DbSet<Item> Items { get; set; }
        public DbSet<AppTransaction> Transactions { get; set; }

        protected virtual void Seek()
        {
            bool newDbCreated = Database.EnsureCreated();

            if (newDbCreated)
            {
                var item1 = new Item();
                var item2 = new Item();
                Items.AddRange(item1, item2);

                SaveChanges();

                item1.Count = 1;
                var item3 = new Item();
                Items.Add(item3);

                SaveChanges();
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Item>(ItemConfig);
        }

        protected internal virtual void ItemConfig(EntityTypeBuilder<Item> builder)
        {
            builder.Property("_id");
            builder.HasKey("_id");
            builder.Property("Name");
        }
    }
}

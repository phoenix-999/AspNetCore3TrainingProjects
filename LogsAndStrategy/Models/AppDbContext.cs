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
        public DbSet<Tag> Tags { get; set; }
        public DbSet<AppTransaction> Transactions { get; set; }

        protected virtual void Seek()
        {
            bool newDbCreated = Database.EnsureCreated();

            if (newDbCreated)
            {
                
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
            modelBuilder.Entity<Tag>(TagConfig);
        }

        protected internal virtual void ItemConfig(EntityTypeBuilder<Item> builder)
        {
            builder.Property("_id");
            builder.HasKey("_id");
            builder.Property(i => i.Name);
            builder.HasMany(i => i.Tags).WithOne().IsRequired();
        }

        protected internal void TagConfig(EntityTypeBuilder<Tag> builder)
        {
            builder.Property("_id");
            builder.HasKey("_id");
            builder.Property(t => t.Label);
        }
    }
}

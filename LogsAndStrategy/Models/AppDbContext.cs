using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
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


        public DbSet<Employee> Employees { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<AppTransaction> Transactions { get; set; }

        public delegate void ActionBuilder(ModelBuilder builder);
        public static event ActionBuilder EventActionBuilder;

        protected virtual void Seek()
        {
            Database.EnsureDeleted();
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
            modelBuilder.Entity<Employee>(EmployeeConfig);
            modelBuilder.Entity<Blog>(BlogConfig);

            if(EventActionBuilder != null)
                EventActionBuilder(modelBuilder);
        }

        protected virtual void ItemConfig(EntityTypeBuilder<Item> builder)
        {
            builder.Property("_id");
            builder.HasKey("_id");
            builder.Property(i => i.Name);
            builder.HasMany(i => i.Tags).WithOne().IsRequired();
        }

        protected void TagConfig(EntityTypeBuilder<Tag> builder)
        {
            builder.Property("_id");
            builder.HasKey("_id");
            builder.Property(t => t.Label);
        }

        protected virtual void EmployeeConfig(EntityTypeBuilder<Employee> builder)
        {
            builder.HasAlternateKey(e => e.PassportId);

            builder.HasAlternateKey(e => e.Guid);
            builder.Property(e => e.Guid).ValueGeneratedOnAdd();
            //Или так:
            //builder.Property(e => e.Guid).HasDefaultValueSql("newid()");

            builder.Property(e => e.LastPayRaise).ValueGeneratedOnAddOrUpdate();

            builder.Property(e => e.Description).HasDefaultValue("No description");

            builder.Property(e => e.EmployeeFullName).HasComputedColumnSql("EmployeeName + ', ' + EmployeeLastName");
        }

        protected virtual void BlogConfig(EntityTypeBuilder<Blog> builder)
        {
            builder.HasIndex(b => b.BlogId);
            builder.HasIndex(b => new { b.BlogId, b.BlogName})
                .IsUnique()
                .HasFilter("BlogName is not null");//По умолчанию в уникальных индексах поставщика SqlServer. Дл отмены HasFilter("null")
        }
    }
}

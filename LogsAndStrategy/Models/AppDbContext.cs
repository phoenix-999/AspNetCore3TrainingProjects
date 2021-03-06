﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
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

        public AppDbContext()
        {

        }

        public DbSet<BlogExtension> BlogExtensions { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Coords> Coords { get; set; }
        public DbSet<ProductionCoords> ProductionCoords { get; set; }
        public DbSet<ClientCoords> ClientCoords { get; set; }
        public DbSet<Shedule> Shedules { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<AppTransaction> Transactions { get; set; }

        public virtual DbSet<Person> People { get; set; }
        public virtual DbSet<School> Schools { get; set; }

        public delegate void ActionBuilder(ModelBuilder builder);
        public static event ActionBuilder EventActionBuilder;

        protected virtual void Seek()
        {
            //Database.EnsureDeleted();
            //Database.EnsureCreated();
            
        }

        public virtual void CreateViews()
        {
            Database.OpenConnection();
            Database.ExecuteSqlRaw(
                @"IF OBJECT_ID('dbo.BlogCountView') IS not NULL
                    BEGIN
                        DROP VIEW dbo.BlogCountView
                    END

                EXEC('CREATE VIEW dbo.BlogCountView AS select count(*) as CountBlogs from Blogs')"
                );
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseLazyLoadingProxies(/*useLazyLoadingProxies: false*/);
            optionsBuilder.UseSqlServer("Data Source =.; Initial Catalog = Tutorials.Tests; Integrated Security = true");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Ignore<Period>();

            modelBuilder.Entity<Item>(ItemConfig);
            modelBuilder.Entity<Tag>(TagConfig);
            modelBuilder.Entity<Employee>(EmployeeConfig);
            modelBuilder.Entity<Blog>(BlogConfig);
            modelBuilder.Entity<Shedule>(SheduleConfig);
            modelBuilder.Entity<Order>(OrderConfig);
            modelBuilder.Entity<OrderDetail>(OrderDetailConfig);
            modelBuilder.Entity<Delivery>(DeliveryConfig);
            modelBuilder.Entity<BlogExtension>(BlogExtensionConfig);
            modelBuilder.Entity<School>(SchoolConfig);

            if (EventActionBuilder != null)
                EventActionBuilder(modelBuilder);
        }

        protected virtual void SchoolConfig(EntityTypeBuilder<School> builder)
        {
            builder.HasMany(s => s.Students).WithOne(s => s.School);
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

            builder.Property<byte[]>("RowVersion").IsRowVersion();
            builder.Property<int>("_id").HasComputedColumnSql("BlogId");
            builder.Property(b => b.Title).HasDefaultValue("Title");
            builder.Property("Author").HasDefaultValue("Author");

            builder.HasData(new Blog { BlogId = 11, BlogName = "Test blog" }, new Blog { BlogId = 20, BlogName = "Test blog 2" });
        }

        protected virtual void SheduleConfig(EntityTypeBuilder<Shedule> builder)
        {
            var valueComparer = new ValueComparer<ComparsionList<Period>>(//Только для полей с изменияемыми типами. Не касается колекций навигации
                (c1, c2) => c1.Equals(c2),
                c => c.GetHashCode(),
                c => new ComparsionList<Period>(c.ToList())
                );


            builder.Property(s => s.Periods)
                .HasConversion<string>((ps) => Shedule.ListToStr(ps), (str) => Shedule.StrToList(str))
                .Metadata
                .SetValueComparer(valueComparer);
        }

        protected virtual void OrderConfig(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");
            builder.HasOne(o => o.OrderDetail).WithOne(od => od.Order).HasForeignKey<OrderDetail>(od => od.Id);//Будет общим полем
            builder.Property(o => o.Status).HasColumnName("Status");//Значение в общих полях должно быть одинаковое
            builder.Property<byte[]>("RowVersion").IsRowVersion().HasColumnName("RowVersion");//Должно быть в главной и зависимой сущности при разделении таблицы с явно указаным одинаковым именем (иначе будет сгенерировано имя с приставкой типа сущности)
        }

        protected virtual void OrderDetailConfig(EntityTypeBuilder<OrderDetail> builder)
        {
            builder.ToTable("Orders");
            builder.Property(o => o.Status).HasColumnName("Status");//Значение в общих полях должно быть одинаковое
            builder.Property<byte[]>("RowVersion").IsRowVersion().HasColumnName("RowVersion");//Должно быть в главной и зависимой сущности при разделении таблицы с явно указаным одинаковым именем (иначе будет сгенерировано имя с приставкой типа сущности)
        }

        protected virtual void DeliveryConfig(EntityTypeBuilder<Delivery> builder)
        {
            builder.OwnsMany(d => d.Addresses, a => {
                a.Property<int>("Id");
                a.HasKey("Id");
                a.OwnsMany<Belay>(ad => ad.AddressBelay);
            });

            builder.OwnsMany(d => d.DefaultAddresses, a => {
                a.Property<int>("Id");
                a.HasKey("Id");
                a.OwnsMany<Belay>(ad => ad.AddressBelay);
            });

            builder.OwnsOne(typeof(Belay), "Belay");
            builder.OwnsOne(typeof(Belay), "DefaultBelay");
        }

        protected virtual void BlogExtensionConfig(EntityTypeBuilder<BlogExtension> builder)
        {
            builder.HasNoKey();
            builder.ToView("BlogCountView");
        }
    }
}

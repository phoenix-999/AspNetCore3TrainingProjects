using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityAppFromEmpty.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace IdentityAppFromEmpty.Data
{
    public class IdentityDbContext : IdentityDbContext<User>
    {
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            IdentityRole roleAdmin = new IdentityRole { Id = "1", Name = "Admins" };
            builder.Entity<IdentityRole>().HasData(roleAdmin);
            SaveChanges();
            
            base.OnModelCreating(builder);
        }
    }
}

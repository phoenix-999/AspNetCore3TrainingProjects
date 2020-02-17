using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SortPaginationFilters.Models
{
    public class UsersContext : DbContext
    {
        public UsersContext(DbContextOptions<UsersContext> options)
            : base(options)
        {
            Database.EnsureCreated();
            CreateStartData();
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Company> Companies { get; set; }

        private void CreateStartData()
        {
            if (Users.Count() > 0 && Companies.Count() > 0)
                return;

            Company oracle = new Company { Name = "Oracle" };
            Company google = new Company { Name = "Google" };
            Company microsoft = new Company { Name = "Microsoft" };
            Company apple = new Company { Name = "Apple" };

            User user1 = new User { Name = "Олег Васильев", Company = oracle, Age = 26 };
            User user2 = new User { Name = "Александр Овсов", Company = oracle, Age = 24 };
            User user3 = new User { Name = "Алексей Петров", Company = microsoft, Age = 25 };
            User user4 = new User { Name = "Иван Иванов", Company = microsoft, Age = 26 };
            User user5 = new User { Name = "Петр Андреев", Company = microsoft, Age = 23 };
            User user6 = new User { Name = "Василий Иванов", Company = google, Age = 23 };
            User user7 = new User { Name = "Олег Кузнецов", Company = google, Age = 25 };
            User user8 = new User { Name = "Андрей Петров", Company = apple, Age = 24 };

            Companies.AddRange(oracle, microsoft, google, apple);
            Users.AddRange(user1, user2, user3, user4, user5, user6, user7, user8);
            SaveChanges();
        }

    }
}

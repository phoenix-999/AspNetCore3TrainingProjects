using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LogsAndStrategy.Models
{
    public class Blog
    {
        public int BlogId { get; set; }
        private readonly int _id;
        public string BlogName { get; set; }
        public List<Post> Posts { get; set; }
        protected string Author { get; set; }
        public string Title { get; }
        public int Year { get; private set; }
        public string Era { get; private set; }
        [NotMapped]
        public bool Token { get; set; }
        [NotMapped]
        private AppDbContext Context { get; set; }

        private Blog(string era, AppDbContext context)
        {
            this.Era = era;
            Context = context;
            Token = true;
        }
        public Blog()
        {

        }

        public string GetAuthor()
        {
            return Author;
        }

        public void SetYear(int year)
        {
            Year = year;
        }

        public int GetId()
        {
            return _id;
        }

        public AppDbContext GetContext()
        {
            return Context;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace LogsAndStrategy.Models
{
    public class Blog : IDisposable
    {
        public int BlogId { get; set; }
        private readonly int _id;
        public string BlogName { get; set; }
        public virtual List<Post> Posts { get; set; }
        public virtual byte[] Bytes { get; set; }
        protected string Author { get; set; }
        public string Title { get; }
        public int Year { get; private set; }
        public string Era { get; private set; }
        [NotMapped]
        public int Count { get; set; } = 0;
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

        public Blog InceremntCount()
        {
            this.Count++;
            return this;
        }

        public void Dispose()
        {
            Debug.WriteLine("Blog dispose");
        }

        ~Blog()
        {
            Debug.WriteLine("Blog finalize");
        }
    }
}

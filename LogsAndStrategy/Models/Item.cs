using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogsAndStrategy.Models
{
    public class Item
    {
        private readonly int _id;

        private Item(int id, string name)
        {
            _id = id;
            Name = name;
        }

        public Item()
        {

        }

        public string Name { get; }

        public int Count { get; set; }
    }
}

using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogsAndStrategy.Models
{
    public class Item
    {
        private readonly int _id;
        private readonly List<Tag> _tags = new List<Tag>();

        private Item(int id, string name)
        {
            _id = id;
            Name = name;
        }

        public Item(string name)
        {
            Name = name;
        }

        public Item()
        {

        }

        public string Name { get; }

        public IReadOnlyList<Tag> Tags => _tags;


        public Tag AddTag(string label)
        {
            var tag = _tags.FirstOrDefault(i => i.Label == label);
            if (tag == null)
            {
                tag = new Tag(label);
                _tags.Add(tag);
            }
            tag.Count++;

            return tag;
        }
    }
}

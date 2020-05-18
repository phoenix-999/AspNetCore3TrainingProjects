using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogsAndStrategy.Models
{
    public class Post
    {
        public int PostId { get; set; }
        public string PostName { get; set; }
        public int? BlogId { get; set; }
        public Blog Blog { get; set; }
    }
}

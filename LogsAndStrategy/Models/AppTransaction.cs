using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogsAndStrategy.Models
{
    public class AppTransaction
    {
        public Guid Id { get; set; }
        public AppTransaction()
        {
            Id = new Guid();
        }
    }
}

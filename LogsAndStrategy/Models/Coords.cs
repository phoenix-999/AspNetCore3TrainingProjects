using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogsAndStrategy.Models
{
    public class Coords
    {
        public int Id { get; set; }
        public string Lat { get; set; }
        public string Lng { get; set; }
        public virtual int  Radius { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogsAndStrategy.Models
{
    public class ClientCoords : Coords
    {
        public override int Radius { get; set; }
        public string Name { get; set; }
    }
}

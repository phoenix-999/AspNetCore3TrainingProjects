using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LogsAndStrategy.Models
{
    public class ProductionCoords : Coords
    {
        public string Name { get; set; }
        public int Count { get; set; }
    }
}

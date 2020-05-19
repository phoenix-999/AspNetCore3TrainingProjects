using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogsAndStrategy.Models
{
    public class Period
    {
        public int Day { get; set; }

        public bool MemberviseEquals(Period other)
        {
            return
                Day == other.Day;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DependencyInjectionApp.Services
{
    public class TimeService
    {
        public DateTime GetTime() => DateTime.Now;
    }
}

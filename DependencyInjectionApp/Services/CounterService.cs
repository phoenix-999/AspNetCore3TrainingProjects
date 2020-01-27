using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DependencyInjectionApp.Services
{
    public class CounterService
    {
        public ICounter Counter { get; }
        public CounterService(ICounter counter)
        {
            this.Counter = counter;
        }
    }
}

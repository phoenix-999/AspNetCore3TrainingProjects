using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DependencyInjectionApp.Services
{
    public class ScopedService : IService
    {
        public string Text => "Scoped";
    }
}

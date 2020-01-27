using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DependencyInjectionApp.Services
{
    public class TransientService : IService
    {
        //public TransientService(ScopedService service)
        //{

        //}

        public string Text => "Transient";
    }
}

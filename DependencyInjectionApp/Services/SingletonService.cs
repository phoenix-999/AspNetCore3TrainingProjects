using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DependencyInjectionApp.Services
{
    public class SingletonService : IService
    {
        //public SingletonService(ScopedService service) //Error
        //{

        //}

        public SingletonService(TransientService service)
        {

        }

        public string Text => "Singleton";
    }
}

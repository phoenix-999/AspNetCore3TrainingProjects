using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SingletonServiceExample
{
    public interface ISomeService
    {
        Guid Id { get; }
    }

    public interface ISingletonService : ISomeService { }
    public interface IScopedService : ISomeService { }
    public interface ITransientService : ISomeService { }

    class SingletonSomeService : ISingletonService
    { 
        public SingletonSomeService(ITransientService transientService/*, IScopedService scopedService*/)
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; }
    }

    class ScopedSomeService :  IScopedService
    {
        public ScopedSomeService(ITransientService transientService)
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; }
    }

    class TransientSomeService : ITransientService
    {
        public TransientSomeService()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; }
    }

}

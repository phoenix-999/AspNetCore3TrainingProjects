using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TryAddEnumerableExample
{
    public interface ISomeService
    {
        string Invoke();
    }

    public class SomeService1 : ISomeService
    {
        public string Invoke()
        {
            return this.GetType().ToString();
        }
    }

    public class SomeService2 : ISomeService
    {
        public string Invoke()
        {
            return this.GetType().ToString();
        }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DependencyInjectionApp.Services
{
    public class RandomCounter :  ICounter
    {
        private int count;
        static Random Rand { get; } = new Random();

        public RandomCounter()
        {
            count = Rand.Next(1, 1000000);
        }

        public int Value => count;
    }
}

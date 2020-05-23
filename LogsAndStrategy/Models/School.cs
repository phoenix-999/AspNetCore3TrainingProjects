using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogsAndStrategy.Models
{
    public class School
    {
        public int Id{ get; set; }
        public string Name { get; set; }
        public virtual List<Student> Students { get; set; }
        //private Blog blog;
        //public  Blog Blog {  
        //    get {
        //        return LazyLoader.Load(this, ref blog);
        //    }
        //    set {
        //        blog = value;
        //    }
        //}

        public School()
        {
            Students = new List<Student>();
        }

        protected School(LazyLoader lazyLoader)//Нужен для ленивой загрузки в обычном режиме если опреден конструктор кроме стандартного
            : this()
        //Если прокси обьект видит, что в классе сущности есть контсруктор кроме стандартного - ищет подходящий себе конструктор
        //или выбрасывает исключение
        {
            LazyLoader = lazyLoader;
        }

        private School(ILazyLoader lazyLoader)
            : this()
        {
            LazyLoader = lazyLoader;
        }

        ILazyLoader LazyLoader { get; }
    }
}

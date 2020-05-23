using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace LogsAndStrategy.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Student: Person
    {
        public virtual School School { get; set; }
    }
}

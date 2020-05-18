using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LogsAndStrategy.Models
{
    public class Employee
    {
        private Employee(string employeeFullName)
        {
            EmployeeFullName = employeeFullName;
        }

        public Employee()
        { }


        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeLastName { get; set; }
        public string EmployeeFullName { get; set; }
        public string PassportId { get; set; } = Guid.NewGuid().ToString();
        public Guid Guid { get; set; }
        public DateTime? LastPayRaise { get; set; }
        public string Description { get; set; }
        [ConcurrencyCheck]
        public string RowVersion { get; set; }
    }
}

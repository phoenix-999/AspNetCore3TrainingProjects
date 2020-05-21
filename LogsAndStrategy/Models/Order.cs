using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LogsAndStrategy.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public OrderDetail OrderDetail { get; set; }
    }
    public class OrderDetail
    {
        public int Id { get; set; }
        public string Status { get; set; }
        [Required]//Не сморя на это будет NULL в БД, так как это зависимая сущность в разделенной таблице
        public string Address { get; set; }
        public Order Order { get; set; }
    }

}

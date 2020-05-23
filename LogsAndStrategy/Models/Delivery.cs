using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LogsAndStrategy.Models
{
    public class Delivery
    {
        public int DeliveryId { get; set; }
        public virtual List<Address> Addresses { get; set; }
        public virtual List<Address> DefaultAddresses { get; set; }
        protected virtual Belay Belay { get; set; }
        protected virtual Belay DefaultBelay { get; set; }
    }

    public class Address
    {
        [Required]
        public string Street { get; set; }
        public int Number { get; set; }
        public virtual List<Belay> AddressBelay { get; set; }
    }

    public class Belay
    {
        public string BelayNumber { get; set; }
    }
}

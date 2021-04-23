using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingRavenDB
{
    class Rental
    {
        public string Id { get; set; }
        public string RentalDate { get; set; }

        public string ReturnDate { get; set; }

        public string LastUpdate { get; set; }

        public string Staff { get; set; }

        public string Customer { get; set; }

        public string Inventory { get; set; }
    }
}

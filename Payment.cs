using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingRavenDB
{
    class Payment
    {

        public string Id { get; set; }
        public double Amount { get; set; }

        public string PaymentDate { get; set; }

        public string Staff { get; set; }

        public string Customer { get; set; }

        public string Rental { get; set; }
    }
}

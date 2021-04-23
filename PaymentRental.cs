namespace TestingRavenDB
{
    internal class PaymentRental
    {
        public string Id { get; set; }
        public string PaymentId { get; set; }
        public double Amount { get; set; }

        public string PaymentDate { get; set; }

        public string Staff { get; set; }

        public string Customer { get; set; }

        public string RentalId { get; set; }

        public string RentalDate { get; set; }

        public string ReturnDate { get; set; }

        public string LastUpdate { get; set; }

        public string RentalStaff { get; set; }

        public string RentalCustomer { get; set; }

        public string Inventory { get; set; }

        public int Count_smaller_pay { get; set; }
    }
}
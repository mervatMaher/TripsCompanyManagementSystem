namespace TripsCompanySystem.ViewModel
{
    public class CreatePaymentViewModel
    {
        public double Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string? Currency { get; set; }
        public string? ProductName { get; set; }
        public string? ProductDescription { get; set; }
    }
}

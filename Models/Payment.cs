namespace TripsCompanySystem.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public double Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public int BookingId { get; set; } 
        public Booking Booking { get; set; }
    }
}

namespace TripsCompanySystem.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int TripId { get; set; }
        public DateTime BookingDate { get; set; }
        public int NumOfFriends { get; set; }
        public int TotalCost { get; set; }

        public ApplicationUser User { get; set; }
        public Trip Trip { get; set; }
        public ICollection<Payment> Payments { get; set; }
    }
}

using System.ComponentModel.DataAnnotations.Schema;

namespace TripsCompanySystem.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int TripId { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public int NumOfFriends { get; set; }
  
        public double TotalCost { get; set; } 

        public ApplicationUser User { get; set; }
        public Trip Trip { get; set; }
        public ICollection<Payment> Payments { get; set; }
    }
}

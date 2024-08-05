using TripsCompanySystem.Models;

namespace TripsCompanySystem.ViewModel
{
    public class BookingViewModel
    {
        public string FullName {  get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public int NumOfFriends { get; set; }
        public double TotalCost { get; set; } /*=> (double)(Trip != null ? Trip.TripPrice * NumOfFriends : 0);*/

        //public int TripId { get; set; }
        public Trip Trip { get; set; }

    }
}

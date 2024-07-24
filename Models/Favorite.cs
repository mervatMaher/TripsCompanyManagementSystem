namespace TripsCompanySystem.Models
{
    public class Favorite
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int TripId { get; set; }
        public ApplicationUser User { get; set; }
        public Trip Trip { get; set; }
    }
}

namespace TripsCompanySystem.Models
{
    public class Rating
    {
        public int Id { get; set; }
        public int Score { get; set; }
        public string UserId { get; set; }
        public int TripId { get; set; }
        public int CompanyId { get; set; }
        public ApplicationUser User { get; set; }
        public Trip Trip { get; set; }
        public Company Company { get; set; }
    }
}

namespace TripsCompanySystem.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? ImageUrl { get; set; } 
        public double? AverageRating => Ratings?.Any() == true ? Ratings.Average(r => r.Score) : 0.0;
        public ICollection<Trip> Trips { get; set; }
        public ICollection<Rating> Ratings { get; set; }
        public ICollection<ApplicationUser> Users { get; set; }
    }
}

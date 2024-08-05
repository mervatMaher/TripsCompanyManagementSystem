using Microsoft.AspNetCore.Identity;

namespace TripsCompanySystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } 
        public string? MainImage { get; set; }
        public int? CompanyId { get; set; } 
        public ICollection<Booking> Bookings { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<Favorite> Favorites {  get; set; }
        public ICollection<Rating> Ratings { get; set; }
        public Company Company { get; set; }
       
    }
}

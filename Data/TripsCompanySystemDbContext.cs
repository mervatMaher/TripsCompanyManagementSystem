using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TripsCompanySystem.Models;

namespace TripsCompanySystem.Data
{
    public class TripsCompanySystemDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<ApplicationUser> Users {  get; set; }  
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Reviews> Reviews { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public TripsCompanySystemDbContext(DbContextOptions options) : base (options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Booking>().HasOne(b => b.User).WithMany(b => b.Bookings)
                .HasForeignKey(u => u.UserId);

            modelBuilder.Entity<Booking>().HasOne(b => b.Trip).WithMany(t => t.Bookings)
                .HasForeignKey(t => t.TripId);

           

        }
    }
}

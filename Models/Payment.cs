using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace TripsCompanySystem.Models
{
    public class Payment
    {
        public int Id { get; set; }
        [Required]
        [Precision(18, 2)]
        public decimal Amount { get; set; }
        public string? StripePaymentId { get; set; }
        public DateTime PaymentDate { get; set; }
        public string? Currency { get; set; }
        public string? PaymentStatus { get; set; }
        public int BookingId { get; set; } 
        public Booking Booking { get; set; }
    }
}

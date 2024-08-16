﻿namespace TripsCompanySystem.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public double Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string? Currency { get; set; }
        public string? ProductName { get; set; }
        public string? ProductDescription { get; set; }
        public int BookingId { get; set; } 
        public Booking Booking { get; set; }
    }
}

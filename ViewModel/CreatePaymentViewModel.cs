using Microsoft.EntityFrameworkCore;

namespace TripsCompanySystem.ViewModel
{
    public class CreatePaymentViewModel
    {
       
        [Precision(18, 2)]
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string? Currency { get; set; }

    }
}

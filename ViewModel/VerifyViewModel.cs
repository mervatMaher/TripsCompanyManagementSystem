using System.ComponentModel.DataAnnotations;

namespace TripsCompanySystem.ViewModel
{
    public class VerifyViewModel
    {
        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress]
        public string Email { get; set; }
    }
}

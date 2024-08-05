using System.ComponentModel.DataAnnotations;

namespace TripsCompanySystem.ViewModel
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is Required")]
        [StringLength(40, MinimumLength = 8, ErrorMessage = "the MinimumLength is 8 Character")]
        [DataType(DataType.Password)]
        [Display(Name = "New Passowrd")]
        [Compare("ConfirmNewPassword", ErrorMessage = "Password doesnt match")]
        
        public string NewPassword { get; set; }


        [Required(ErrorMessage = "Confirm Password is Required")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm New PAssowrd")]
        public string ConfirmNewPassword { get; set; }
    }
}

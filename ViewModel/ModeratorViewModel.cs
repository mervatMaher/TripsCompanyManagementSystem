using System.ComponentModel.DataAnnotations;

namespace TripsCompanySystem.ViewModel
{
    public class ModeratorViewModel
    {
        [Required(ErrorMessage = "FullName is Required")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "UserName is Required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "PhoneNum is Required")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Password is Required")]
        [StringLength(40, MinimumLength = 8, ErrorMessage = "the MinimumLength is 8 Character")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        //public int CompanyId { get; set; }
        public IFormFile MainImage { get; set; }
        public string? ImageUrl { get; set; }
    }
}

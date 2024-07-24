using TripsCompanySystem.Models;

namespace TripsCompanySystem.ViewModel
{
    public class RegistrationModel
    {
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNum { get; set; }
        public string Password { get; set; }
        public IFormFile Image { get; set; }
        public bool RememberMe { get; set; }

    }
}

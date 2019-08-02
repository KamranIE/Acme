using System.ComponentModel.DataAnnotations;


namespace Acme.UI.Models.Authentication
{
    public class LoginModel
    {
        [Display(Name = "Username")]
        [Required]
        public string Username { get; set; }

        [Display(Name = "Password")]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        
    }
}
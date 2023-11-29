using System.ComponentModel.DataAnnotations;

namespace BlazorApp1.Client.Models.Account
{
    public class Login
    {
        [Required(ErrorMessage = "Please enter {0}.")]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}

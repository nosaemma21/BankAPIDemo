using System.ComponentModel.DataAnnotations;

namespace BankAccountManager.Models.IdentityModels;

public class LoginModel
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = string.Empty;
}

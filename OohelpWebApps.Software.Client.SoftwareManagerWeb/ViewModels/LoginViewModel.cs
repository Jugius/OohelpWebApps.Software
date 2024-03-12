using System.ComponentModel.DataAnnotations;

namespace OohelpWebApps.Software.Client.SoftwareManagerWeb.ViewModels;

public class LoginViewModel
{
    [Required, EmailAddress(ErrorMessage = "It is not valid email.")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6, ErrorMessage = "Minimal lenght is 6 chars")]
    public string Password { get; set; }
}

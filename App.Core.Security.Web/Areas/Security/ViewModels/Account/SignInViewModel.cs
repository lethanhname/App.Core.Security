using System.ComponentModel.DataAnnotations;

namespace App.Core.Security.Web.ViewModels.Account
{
  public class SignInViewModel 
  {
    public int? Id { get; set; }
    public string Next { get; set; }

    [Display(Name = "User")]
    [Required]
    [StringLength(64)]
    public string Email { get; set; }

    [Display(Name = "Password")]
    [Required]
    [StringLength(64)]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Display(Name = "Remember me")]
    public bool RememberMe { get; set; }
  }
}
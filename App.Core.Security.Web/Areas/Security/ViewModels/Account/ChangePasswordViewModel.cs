using System.ComponentModel.DataAnnotations;
using App.CoreLib.EF.Data.Entity;

namespace App.Core.Security.Web.ViewModels.Account
{
  public class ChangePasswordViewModel : EntityBase
  {
        
        [Display(Name = "Password")]
        [Required]
        [StringLength(64)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Confirm Password")]
        [Required]
        [StringLength(64)]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
  }
}
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using App.Core.Security.Web.ViewModels.UserRole;
using App.CoreLib.EF.Data.Entity;

namespace App.Core.Security.Web.ViewModels.User
{
    public class CreateUserViewModel : EntityBase
    {
        [Display(Name = "UserName * ")]
        [Required]
        [StringLength(64)]
        public string UserName { get; set; }


        [Display(Name = "Email * ")]
        [Required]
        [StringLength(64)]
        public string Email { get; set; }

        public List<UserRoleViewModel> UserRoles { get; set; }

        [Display(Name = "FirstName * ")]
        [Required]
        [StringLength(64)]
        public string FirstName { get; set; }

        [Display(Name = "LastName * ")]
        [Required]
        [StringLength(64)]
        public string LastName { get; set; }
    }
}

using App.CoreLib.EF.Data.Entity;

namespace App.Core.Security.Web.ViewModels.UserRole
{
    public class UserRoleViewModel : EntityBase
    {
        public string Name { get; set; }
        public bool IsAssigned { get; set; }
    }
}
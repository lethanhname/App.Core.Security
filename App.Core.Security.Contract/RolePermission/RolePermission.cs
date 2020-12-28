using System.ComponentModel.DataAnnotations.Schema;
using App.CoreLib.EF.Data.Entity;
using App.CoreLib.EF.Data.Identity;

namespace App.Core.Security.Contract
{
  /// <summary>
  /// Represents a many-to-many relationship between the roles and permissions.
  /// </summary>
  public class RolePermission : EntityBase
  {
    /// <summary>
    /// Gets or sets the role identifier this role permission is related to.
    /// </summary>
    public string RoleId { get; set; }

    /// <summary>
    /// Gets or sets the permission identifier this role permission is related to.
    /// </summary>
    public string PermissionId { get; set; }

    public virtual AppRole Role { get; set; }
    public virtual Permission Permission { get; set; }
  }
}
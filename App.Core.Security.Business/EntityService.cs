using Microsoft.EntityFrameworkCore;
using App.Core.Security.Contract;
using App.CoreLib.EF.Data;

namespace App.Core.Security.Business
{
    public class EntityService
    {
        public static void RegisterEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Permission>(etb =>
              {
                  etb.HasKey(e => e.Code);
                  etb.Property(e => e.Code).IsRequired().HasMaxLength(32);
                  etb.Property(e => e.Name).IsRequired().HasMaxLength(64);
                  etb.Property(e => e.Product).IsRequired().HasMaxLength(64);
                  etb.Property(e => e.FunctionName).IsRequired().HasMaxLength(64);
                  etb.Property(e => e.RowVersion).IsConcurrencyToken();
                  etb.ToTable("Permissions");
              }
            );
            modelBuilder.Entity<RolePermission>(etb =>
                {
                    etb.HasKey(e => new { e.RoleId, e.PermissionId });
                    etb.ToTable("RolePermissions");
                }
            );
        }
    }
}
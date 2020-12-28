using Microsoft.EntityFrameworkCore;
using App.Core.Security.Business;
using App.CoreLib.EF.Data;

namespace App.Core.Security.Web
{
  public class EntityRegistrar : IEntityRegistrar
  {
    public void RegisterEntities(ModelBuilder modelBuilder) => EntityService.RegisterEntities(modelBuilder);
  }
}
using App.CoreLib.EF.Data.Entity;

namespace App.Core.Security.Contract
{
    public class Permission : EntityBase
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public string Product { get; set; }

        public string FunctionName { get; set; }

    }
}
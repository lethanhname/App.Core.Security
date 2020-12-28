using NUnit.Framework;
using System;
using App.Core.Security.Contract;
using App.CoreLib.EF.Data;
using App.CoreLib.EF.Data.Repositories;
using App.CoreLib.IntegrationTest;
using Microsoft.AspNetCore.Http;
using System.Security.Principal;
using System.Security.Claims;
using System.Collections.Generic;
using App.CoreLib;
using Microsoft.Extensions.DependencyInjection;
using App.CoreLib.EF.Context;
using App.CoreLib.EF.Events;

namespace App.Core.Security.IntegrationTests
{
    [TestFixture]
    public class PermissionRepositoryTests : IntegrationTestBase
    {
        private IRepository<Permission> permissionRepository;
        public override void TestInitialize()
        {
            base.TestInitialize();
            permissionRepository = new Repository<Permission>(context);
        }
        [Test]
        public void SaveShouldReturnNoError()
        {
            RunTest(() =>
            {
                Console.WriteLine(Globals.ContentRootPath);
                Console.WriteLine(Globals.ExtensionsPath); 
                permissionRepository.Add(new Permission { Product = "Security", FunctionName = "List Roles", Code = "test", Name = "View Role", RowVersion = 1 });

                var permissionResult = permissionRepository.SaveChangesAsync().Result;
                Assert.AreEqual(true, permissionResult.Succeeded);
            }, false);
        }
    }
}


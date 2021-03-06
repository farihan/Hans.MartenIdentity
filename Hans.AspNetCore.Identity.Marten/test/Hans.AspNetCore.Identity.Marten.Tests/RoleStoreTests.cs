﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Hans.AspNetCore.Identity.Marten.Data.Domains;
using Hans.AspNetCore.Identity.Marten.Data.Persistence;
using Hans.AspNetCore.Identity.Marten.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Hans.AspNetCore.Identity.Marten.Tests
{
    [TestClass]
    public class RoleStoreTests : BaseTests
    {
        [TestInitialize]
        public void BeforeEachTest()
        {
            DocStore.Advanced.Clean.CompletelyRemoveAll();
        }

        [TestMethod]
        public async void CanRoles()
        {
            var store = new RoleStore<IdentityRole>(DocStore);

            // arrange
            var role1 = new IdentityRole("Role1");
            var role2 = new IdentityRole("Role2");
            var role3 = new IdentityRole("Role3");
            var role4 = new IdentityRole("Role4");
            var role5 = new IdentityRole("Role5");

            // act
            await store.CreateAsync(role1);
            await store.CreateAsync(role2);
            await store.CreateAsync(role3);
            await store.CreateAsync(role4);
            await store.CreateAsync(role5);

            // assert
            Assert.AreEqual(5, store.Roles.Count());
        }

        [TestMethod]
        public async void CanCreateUpdateDeleteRole()
        {
            var store = new RoleStore<IdentityRole>(DocStore);

            // arrange
            var role1 = new IdentityRole("Role1");

            // act
            await store.CreateAsync(role1);
            var r1 = store.Roles.FirstOrDefault();

            // assert
            Assert.AreEqual(role1.Id, r1.Id);

            r1.Name = "Role2";
            await store.UpdateAsync(r1);
            var r2 = store.Roles.FirstOrDefault();

            // assert
            Assert.AreEqual(r1.Name, r2.Name);

            await store.DeleteAsync(r1);
            var r3 = store.Roles.FirstOrDefault();

            // assert
            Assert.IsNull(r3);
        }

        [TestMethod]
        public async void CanFindByIdAsync()
        {
            var store = new RoleStore<IdentityRole>(DocStore);

            // arrange
            var role1 = new IdentityRole("Role1");

            // act
            await store.CreateAsync(role1);

            var r = store.FindByIdAsync(role1.Id);

            // assert
            Assert.AreEqual("Role1", r.Result.Name);
        }

        [TestMethod]
        public async void CanFindByNameAsync()
        {
            var lookup = new LowerInvariantLookupNormalizer();
            var store = new RoleStore<IdentityRole>(DocStore);

            // arrange
            var role1 = new IdentityRole("Role1");
            role1.NormalizedName = lookup.Normalize(role1.Name);

            // act
            await store.CreateAsync(role1);

            var r = store.FindByNameAsync(role1.NormalizedName);

            // assert
            Assert.AreEqual(role1.Id, r.Result.Id);
        }

        [TestMethod]
        public void CanSetGetNormalizedRoleNameAsync()
        {
            var lookup = new LowerInvariantLookupNormalizer();
            var store = new RoleStore<IdentityRole>(DocStore);

            // arrange
            var role1 = new IdentityRole("Role1");

            // act
            store.SetNormalizedRoleNameAsync(role1, lookup.Normalize(role1.Name));

            var r = store.GetNormalizedRoleNameAsync(role1);

            // assert
            Assert.AreEqual(lookup.Normalize(role1.Name), r.Result);
        }

        [TestMethod]
        public void CanGetRoleIdAsync()
        {
            var store = new RoleStore<IdentityRole>(DocStore);

            // arrange
            var role1 = new IdentityRole("Role1");

            // act

            var r = store.GetRoleIdAsync(role1);

            // assert
            Assert.AreEqual(role1.Id, r.Result);
        }

        [TestMethod]
        public void CanSetGetRoleNameAsync()
        {
            var store = new RoleStore<IdentityRole>(DocStore);

            // arrange
            var role1 = new IdentityRole();

            // act
            store.SetRoleNameAsync(role1, "Role1000");
            var r = store.GetRoleNameAsync(role1);

            // assert
            Assert.AreEqual("Role1000", r.Result);
        }
    }
}

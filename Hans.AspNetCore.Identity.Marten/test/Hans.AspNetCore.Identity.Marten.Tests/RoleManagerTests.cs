using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hans.AspNetCore.Identity.Marten.Tests
{
    [TestClass]
    public class RoleManagerTests : BaseTests
    {
        [TestInitialize]
        public void BeforeEachTest()
        {
            DocStore.Advanced.Clean.CompletelyRemoveAll();
        }

        [TestMethod]
        public void CanCreateRoleManager()
        {
            // arrange
            // act
            var manager = GetRoleManager(DocStore);

            // assert
            Assert.IsNotNull(manager);
        }
    }
}

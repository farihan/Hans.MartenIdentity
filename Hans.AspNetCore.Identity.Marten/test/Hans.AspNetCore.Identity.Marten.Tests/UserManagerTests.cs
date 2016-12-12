using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hans.AspNetCore.Identity.Marten.Tests
{
    [TestClass]
    public class UserManagerTests : BaseTests
    {
        [TestInitialize]
        public void BeforeEachTest()
        {
            DocStore.Advanced.Clean.CompletelyRemoveAll();
        }

        [TestMethod]
        public void CanCreateUserManager()
        {
            // arrange
            // act
            var manager = GetUserManager(DocStore);

            // assert
            Assert.IsNotNull(manager);
        }
    }
}

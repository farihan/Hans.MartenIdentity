using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hans.AspNetCore.Identity.Marten.Tests
{
    [TestClass]
    public class SignInManagerTests : BaseTests
    {
        [TestInitialize]
        public void BeforeEachTest()
        {
            DocStore.Advanced.Clean.CompletelyRemoveAll();
        }

        [TestMethod]
        public void CanGetSignInManager()
        {
            // arrange
            // act
            var manager = GetSignInManager(DocStore);

            // assert
            Assert.IsNotNull(manager);
        }
    }
}

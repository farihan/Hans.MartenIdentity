using Baseline;
using Hans.AspNetCore.Identity.Marten.Data;
using Hans.AspNetCore.Identity.Marten.Data.Domains;
using Hans.AspNetCore.Identity.Marten.Data.Persistence;
using Marten;
using Marten.Schema;
using Marten.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hans.AspNetCore.Identity.Marten.Tests
{
    public class BaseTests
    {
        protected readonly string PASSWORD = "User@123456";
        protected string Connection { get; set; }
        protected IDocumentStore DocStore { get; set; }

        protected BaseTests()
        {
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("appsettings.json");

            var configuration = configurationBuilder.Build();
            Connection = configuration["ConnectionStrings:IdentityConnection"];

            DocStore = DocumentStore.For(Connection);
        }

        protected RoleManager<IdentityRole> GetRoleManager(IDocumentStore document)
        {
            var store = new RoleStore<IdentityRole>(document);
            var validator = new List<RoleValidator<IdentityRole>>();
            var keyNormalizer = new LowerInvariantLookupNormalizer();
            var errors = new IdentityErrorDescriber();
            var logger = new Microsoft.Extensions.Logging.Logger<RoleManager<IdentityRole>>(new LoggerFactory());

            var context = new Mock<Microsoft.AspNetCore.Http.HttpContext>();
            var contextAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);

            return new RoleManager<IdentityRole>(store, validator, keyNormalizer, errors, logger, contextAccessor.Object);
        }

        protected UserManager<IdentityUser> GetUserManager(IDocumentStore document)
        {
            var store = new UserStore<IdentityUser>(document);

            var identityOptions = new IdentityOptions();
            var options = new Mock<IOptions<IdentityOptions>>();
            options.Setup(x => x.Value).Returns(identityOptions);

            var passwordHasher = new PasswordHasher<IdentityUser>();
            var validator = new List<UserValidator<IdentityUser>>();
            var passwordValidator = new List<PasswordValidator<IdentityUser>>();
            var keyNormalizer = new LowerInvariantLookupNormalizer();
            var errors = new IdentityErrorDescriber();

            var services = new ServiceCollection();
            services.AddSingleton<Microsoft.AspNetCore.Http.IHttpContextAccessor, Microsoft.AspNetCore.Http.HttpContextAccessor>();
            services.AddLogging();
            var providers = services.BuildServiceProvider();

            var logger = new Microsoft.Extensions.Logging.Logger<UserManager<IdentityUser>>(new Microsoft.Extensions.Logging.LoggerFactory());

            return new UserManager<IdentityUser>(store, options.Object, passwordHasher, validator, passwordValidator, keyNormalizer, errors, providers, logger);
        }

        protected SignInManager<IdentityUser> GetSignInManager(IDocumentStore store)
        {
            var userManager = GetUserManager(store);
            var roleManager = GetRoleManager(store);

            var context = new Mock<Microsoft.AspNetCore.Http.HttpContext>();
            var contextAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context.Object);

            var identityOptions = new IdentityOptions();
            identityOptions.Cookies.ApplicationCookie.ExpireTimeSpan = TimeSpan.FromMinutes(10);
            identityOptions.Cookies.ApplicationCookie.SlidingExpiration = false;

            var options = new Mock<IOptions<IdentityOptions>>();
            options.Setup(x => x.Value).Returns(identityOptions);

            var claims = new UserClaimsPrincipalFactory<IdentityUser, IdentityRole>(userManager, roleManager, options.Object);
            var logger = new Microsoft.Extensions.Logging.Logger<SignInManager<IdentityUser>>(new Microsoft.Extensions.Logging.LoggerFactory());

            return new SignInManager<IdentityUser>(userManager, contextAccessor.Object, claims, options.Object, logger);
        }
    }
}

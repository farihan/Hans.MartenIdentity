﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Globalization;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using Hans.AspNetCore.Identity.Marten.Data.Domains;
using Marten;

namespace Hans.AspNetCore.Identity.Marten.Data.Persistence
{
    public class UserStore<TDomain> :
        IUserLoginStore<TDomain>,
        IUserRoleStore<TDomain>,
        IUserClaimStore<TDomain>,
        IUserPasswordStore<TDomain>,
        IUserSecurityStampStore<TDomain>,
        IUserEmailStore<TDomain>,
        IUserLockoutStore<TDomain>,
        IUserPhoneNumberStore<TDomain>,
        IQueryableUserStore<TDomain>,
        IUserTwoFactorStore<TDomain>,
        IUserAuthenticationTokenStore<TDomain>
        where TDomain : IdentityUser
    {
        private readonly IRepository<TDomain> userRepository;
        private readonly IRepository<IdentityRole> roleRepository;
        private readonly IRepository<IdentityUserToken> tokenRepository;

        public UserStore(IDocumentStore store)
        {
            userRepository = new Repository<TDomain>(store);
            roleRepository = new Repository<IdentityRole>(store);
            tokenRepository = new Repository<IdentityUserToken>(store);
        }

        public IQueryable<TDomain> Users
        {
            get
            {
                return userRepository.FindAll();
            }
        }

        public Task AddClaimsAsync(TDomain user, IEnumerable<Claim> claims, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (claims == null)
            {
                throw new ArgumentNullException(nameof(claims));
            }

            foreach (var claim in claims)
            {
                user.Claims.Add(new IdentityUserClaim()
                {
                    ClaimType = claim.Type,
                    ClaimValue = claim.Value
                });
            }

            return Task.FromResult(false);
        }

        public Task AddLoginAsync(TDomain user, UserLoginInfo login, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (login == null)
            {
                throw new ArgumentNullException(nameof(login));
            }

            user.Logins.Add(new IdentityUserLogin()
            {
                ProviderKey = login.ProviderKey,
                LoginProvider = login.LoginProvider
            });

            return Task.FromResult(false);
        }

        public async Task AddToRoleAsync(TDomain user, string roleName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (string.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(roleName));
            }

            var identityRole = await roleRepository.FindOneByAsync(x => x.Name.ToLower() == roleName.ToLower(), cancellationToken);

            if (identityRole == null)
            {
                throw new InvalidOperationException(string.Format("Role {0} does not exist", nameof(roleName)));
            }

            user.Roles.Add(identityRole);
        }

        public async Task<IdentityResult> CreateAsync(TDomain user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            await userRepository.SaveAsync(user, cancellationToken);

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(TDomain user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            await userRepository.DeleteAsync(user, cancellationToken);

            return IdentityResult.Success;
        }

        public void Dispose()
        {

        }

        public Task<TDomain> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            return userRepository.FindOneByAsync(x => x.NormalizedEmail.ToLower() == normalizedEmail.ToLower(), cancellationToken);
        }

        public Task<TDomain> FindByIdAsync(string userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            return userRepository.FindOneByAsync(x => x.Id.ToLower() == userId.ToLower(), cancellationToken);
        }

        public async Task<TDomain> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            var userLogins = await userRepository.FindAllByAsync(x => x.Logins
                .Any(l => l.LoginProvider == loginProvider && l.ProviderKey == providerKey), cancellationToken);

            return userLogins.FirstOrDefault();
        }

        public Task<TDomain> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            return userRepository.FindOneByAsync(x => x.NormalizedUserName.ToLower() == normalizedUserName.ToLower(), cancellationToken);
        }

        public Task<int> GetAccessFailedCountAsync(TDomain user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.AccessFailedCount);
        }

        public Task<IList<Claim>> GetClaimsAsync(TDomain user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var list = new List<Claim>();

            foreach (var identityClaim in user.Claims)
            {
                list.Add(new Claim(identityClaim.ClaimType, identityClaim.ClaimValue));
            }

            return Task.FromResult<IList<Claim>>(list);
        }

        public Task<string> GetEmailAsync(TDomain user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(TDomain user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.EmailConfirmed);
        }

        public Task<bool> GetLockoutEnabledAsync(TDomain user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.LockoutEnabled);
        }

        public Task<DateTimeOffset?> GetLockoutEndDateAsync(TDomain user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.LockoutEnd);
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(TDomain user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var list = new List<UserLoginInfo>();

            foreach (var identityUserLogin in user.Logins)
            {
                list.Add(new UserLoginInfo(identityUserLogin.LoginProvider, identityUserLogin.ProviderKey, identityUserLogin.ProviderDisplayName));
            }

            return Task.FromResult<IList<UserLoginInfo>>(list);
        }

        public Task<string> GetNormalizedEmailAsync(TDomain user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.NormalizedEmail);
        }

        public Task<string> GetNormalizedUserNameAsync(TDomain user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.NormalizedUserName);
        }

        public Task<string> GetPasswordHashAsync(TDomain user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.PasswordHash);
        }

        public Task<string> GetPhoneNumberAsync(TDomain user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(TDomain user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        public Task<IList<string>> GetRolesAsync(TDomain user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var roles = user.Roles.Select(x => x.Name);

            return Task.FromResult<IList<string>>(roles.ToList());
        }

        public Task<string> GetSecurityStampAsync(TDomain user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.SecurityStamp);
        }

        public async Task<string> GetTokenAsync(TDomain user, string loginProvider, string name, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var matchedToken = await tokenRepository.FindOneByAsync(x => x.UserId.Equals(user.Id) &&
                x.LoginProvider == loginProvider &&
                x.Name == name, cancellationToken);

            if (matchedToken != null)
            {
                return matchedToken.Value;
            }

            return string.Empty;
        }

        public Task<bool> GetTwoFactorEnabledAsync(TDomain user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.TwoFactorEnabled);
        }

        public Task<string> GetUserIdAsync(TDomain user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(TDomain user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.UserName);
        }

        public async Task<IList<TDomain>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }

            var users = await userRepository.FindAllByAsync(x => x.Claims
                .Any(c => c.ClaimValue == claim.Value && c.ClaimType == claim.Type), cancellationToken);

            return users;
        }

        public async Task<IList<TDomain>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentNullException(nameof(roleName));
            }

            var users = await userRepository.FindAllByAsync(x => x.Roles
                .Any(r => r.Name.ToLower() == roleName.ToLower()), cancellationToken);

            return users;
        }

        public Task<bool> HasPasswordAsync(TDomain user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user.PasswordHash != null);
        }

        public Task<int> IncrementAccessFailedCountAsync(TDomain user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.AccessFailedCount = user.AccessFailedCount + 1;

            return Task.FromResult(user.AccessFailedCount);
        }

        public Task<bool> IsInRoleAsync(TDomain user, string roleName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (string.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(roleName));
            }

            var role = user.Roles.Where(x => x.Name.ToLower() == roleName.ToLower());

            return Task.FromResult(role.Count() > 0);
        }

        public Task RemoveClaimsAsync(TDomain user, IEnumerable<Claim> claims, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            foreach (var claim in claims)
            {
                var matchedClaim = user.Claims.FirstOrDefault(uc => uc.ClaimValue == claim.Value &&
                    uc.ClaimType == claim.Type);

                if (matchedClaim != null)
                {
                    user.Claims.Remove(matchedClaim);
                }
            }

            return Task.FromResult(0);
        }

        public Task RemoveFromRoleAsync(TDomain user, string roleName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (string.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(roleName));
            }

            var roles = user.Roles.Where(x => x.Name.ToLower() == roleName.ToLower()).ToList();

            foreach (var role in roles)
            {
                user.Roles.Remove(role);
            }

            return Task.FromResult(0);
        }

        public Task RemoveLoginAsync(TDomain user, string loginProvider, string providerKey, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var info = user.Logins.FirstOrDefault(x => x.LoginProvider == loginProvider && x.ProviderKey == providerKey);

            if (info != null)
            {
                user.Logins.Remove(info);
            }

            return Task.FromResult(0);
        }

        public async Task RemoveTokenAsync(TDomain user, string loginProvider, string name, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var matchedToken = await tokenRepository.FindOneByAsync(x => x.UserId.Equals(user.Id) &&
                x.LoginProvider == loginProvider &&
                x.Name == name, cancellationToken);

            if (matchedToken != null)
            {
                tokenRepository.Delete(matchedToken);
            }
        }

        public Task ReplaceClaimAsync(TDomain user, Claim claim, Claim newClaim, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }

            if (newClaim == null)
            {
                throw new ArgumentNullException(nameof(newClaim));
            }

            var matchedClaims = user.Claims.Where(uc => uc.ClaimValue == claim.Value &&
                uc.ClaimType == claim.Type);

            foreach (var matchedClaim in matchedClaims)
            {
                matchedClaim.ClaimValue = newClaim.Value;
                matchedClaim.ClaimType = newClaim.Type;
            }

            return Task.FromResult(0);
        }

        public Task ResetAccessFailedCountAsync(TDomain user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.AccessFailedCount = 0;

            return Task.FromResult(0);
        }

        public Task SetEmailAsync(TDomain user, string email, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.Email = email;

            return Task.FromResult(0);
        }

        public Task SetEmailConfirmedAsync(TDomain user, bool confirmed, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.EmailConfirmed = confirmed;

            return Task.FromResult(0);
        }

        public Task SetLockoutEnabledAsync(TDomain user, bool enabled, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.LockoutEnabled = enabled;

            return Task.FromResult(0);
        }

        public Task SetLockoutEndDateAsync(TDomain user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.LockoutEnd = lockoutEnd;

            return Task.FromResult(0);
        }

        public Task SetNormalizedEmailAsync(TDomain user, string normalizedEmail, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.NormalizedEmail = normalizedEmail;

            return Task.FromResult(0);
        }

        public Task SetNormalizedUserNameAsync(TDomain user, string normalizedName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.NormalizedUserName = normalizedName;

            return Task.FromResult(0);
        }

        public Task SetPasswordHashAsync(TDomain user, string passwordHash, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.PasswordHash = passwordHash;

            return Task.FromResult(0);
        }

        public Task SetPhoneNumberAsync(TDomain user, string phoneNumber, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.PhoneNumber = phoneNumber;

            return Task.FromResult(0);
        }

        public Task SetPhoneNumberConfirmedAsync(TDomain user, bool confirmed, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.PhoneNumberConfirmed = confirmed;

            return Task.FromResult(0);
        }

        public Task SetSecurityStampAsync(TDomain user, string stamp, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.SecurityStamp = stamp;

            return Task.FromResult(0);
        }

        public async Task SetTokenAsync(TDomain user, string loginProvider, string name, string value, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var matchedToken = await tokenRepository.FindOneByAsync(x => x.UserId.Equals(user.Id) &&
                x.LoginProvider == loginProvider &&
                x.Name == name, cancellationToken);

            if (matchedToken == null)
            {
                var token = new IdentityUserToken()
                {
                    UserId = user.Id,
                    LoginProvider = loginProvider,
                    Name = name,
                    Value = value
                };

                tokenRepository.Save(token);
            }
        }

        public Task SetTwoFactorEnabledAsync(TDomain user, bool enabled, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.TwoFactorEnabled = enabled;

            return Task.FromResult(0);
        }

        public Task SetUserNameAsync(TDomain user, string userName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.UserName = userName;

            return Task.FromResult(0);
        }

        public async Task<IdentityResult> UpdateAsync(TDomain user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            await userRepository.UpdateAsync(user, cancellationToken);

            return IdentityResult.Success;
        }
    }
}

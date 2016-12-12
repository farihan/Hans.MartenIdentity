﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Hans.AspNetCore.Identity.Marten.Data.Domains;
using Marten;

namespace Hans.AspNetCore.Identity.Marten.Data.Persistence
{
    public class RoleStore<TDomain> : IRoleStore<TDomain>, IQueryableRoleStore<TDomain> where TDomain : IdentityRole
    {
        private readonly IRepository<TDomain> repository;

        public RoleStore(IDocumentStore store)
        {
            repository = new Repository<TDomain>(store);
        }

        public IQueryable<TDomain> Roles
        {
            get
            {
                return repository.FindAll();
            }
        }

        public async Task<IdentityResult> CreateAsync(TDomain role, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            //repository.Save(role);
            //return Task.FromResult(IdentityResult.Success);

            await repository.SaveAsync(role);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(TDomain role, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            //repository.Delete(role);
            //return Task.FromResult(IdentityResult.Success);

            await repository.DeleteAsync(role);
            return IdentityResult.Success;
        }

        public void Dispose()
        {

        }

        public Task<TDomain> FindByIdAsync(string roleId, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            //return Task.FromResult(repository.FindOneBy(x => x.Id.ToLower() == roleId.ToLower()));
            return repository.FindOneByAsync(x => x.Id.ToLower() == roleId.ToLower());
        }

        public Task<TDomain> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            //return Task.FromResult(repository.FindOneBy(x => x.Name.ToLower() == normalizedRoleName.ToLower()));
            return repository.FindOneByAsync(x => x.Name.ToLower() == normalizedRoleName.ToLower());
        }

        public Task<string> GetNormalizedRoleNameAsync(TDomain role, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            return Task.FromResult(role.NormalizedName);
        }

        public Task<string> GetRoleIdAsync(TDomain role, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            return Task.FromResult(role.Id);
        }

        public Task<string> GetRoleNameAsync(TDomain role, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            return Task.FromResult(role.Name);
        }

        public Task SetNormalizedRoleNameAsync(TDomain role, string normalizedName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            role.NormalizedName = normalizedName;

            return Task.FromResult(0);
        }

        public Task SetRoleNameAsync(TDomain role, string roleName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            role.Name = roleName;

            return Task.FromResult(0);
        }

        public async Task<IdentityResult> UpdateAsync(TDomain role, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            //repository.Update(role);
            //return Task.FromResult(IdentityResult.Success);

            await repository.UpdateAsync(role);
            return IdentityResult.Success;
        }
    }
}

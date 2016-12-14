using Hans.AspNetCore.Identity.Marten.Data.Domains;
using Marten;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hans.AspNetCore.Identity.Marten.Data
{
    public static class MartenStoreOptionsExtensions
    {
        public static void ConfigureIdentityStoreOptions<TUser, TKey>(this StoreOptions storeOptions, IdentityOptions identityOptions) where TUser : IdentityUser
        {
            var userSchema = storeOptions.Schema.For<TUser>();

            userSchema.Index(x => x.NormalizedUserName, option => { option.IsUnique = true; });

            if (identityOptions.User.RequireUniqueEmail)
            {
                userSchema.Index(x => x.NormalizedEmail, option => { option.IsUnique = true; });
            }
        }
    }
}

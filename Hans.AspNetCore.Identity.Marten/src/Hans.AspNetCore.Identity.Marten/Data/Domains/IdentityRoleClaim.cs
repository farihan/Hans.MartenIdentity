using Marten.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hans.AspNetCore.Identity.Marten.Data.Domains
{
    public class IdentityRoleClaim
    {
        public virtual Guid Id { get; set; }
        public virtual string ClaimType { get; set; }
        public virtual string ClaimValue { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hans.AspNetCore.Identity.Marten.Data.Domains
{
    public class IdentityUserToken
    {
        public virtual Guid Id { get; set; }
        public virtual string UserId { get; set; }
        public virtual string LoginProvider { get; set; }
        public virtual string Name { get; set; }
        public virtual string Value { get; set; }
    }
}

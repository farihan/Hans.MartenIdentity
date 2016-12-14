using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Hans.AspNetCore.Identity.Marten.Data.Persistence
{
    public interface IRepository<TDomain> where TDomain : class
    {
        void Save(TDomain instance);
        void Update(TDomain instance);
        void Delete(TDomain instance);
        IQueryable<TDomain> FindAll();
        IQueryable<TDomain> FindAllBy(Expression<Func<TDomain, bool>> where);
        TDomain FindOneBy(Expression<Func<TDomain, bool>> where);

        //Task SaveAsync(TDomain instance, CancellationToken cancellationToken);
        //Task UpdateAsync(TDomain instance, CancellationToken cancellationToken);
        //Task DeleteAsync(TDomain instance, CancellationToken cancellationToken);
        //Task<IList<TDomain>> FindAllAsync(CancellationToken cancellationToken);
        //Task<IList<TDomain>> FindAllByAsync(Expression<Func<TDomain, bool>> where, CancellationToken cancellationToken);
        //Task<TDomain> FindOneByAsync(Expression<Func<TDomain, bool>> where, CancellationToken cancellationToken);
    }
}

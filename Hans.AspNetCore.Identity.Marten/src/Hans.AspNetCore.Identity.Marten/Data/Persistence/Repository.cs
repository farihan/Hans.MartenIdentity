using Marten;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Hans.AspNetCore.Identity.Marten.Data.Persistence
{
    public class Repository<TDomain> : IRepository<TDomain>, IDisposable where TDomain : class
    {
        private readonly IDocumentStore store;

        public Repository(IDocumentStore store)
        {
            this.store = store;
        }

        public void Delete(TDomain instance)
        {
            using (IDocumentSession session = store.LightweightSession())
            {
                session.Delete(instance);

                session.SaveChanges();
            }
        }

        public void Dispose()
        {
            store.Dispose();
        }

        public IQueryable<TDomain> FindAll()
        {
            using (IQuerySession session = store.QuerySession())
            {
                return session.Query<TDomain>().AsQueryable();
            }
        }

        public IQueryable<TDomain> FindAllBy(Expression<Func<TDomain, bool>> where)
        {
            using (IQuerySession session = store.QuerySession())
            {
                return session.Query<TDomain>().Where(where.Compile()).AsQueryable();
            }
        }

        public TDomain FindOneBy(Expression<Func<TDomain, bool>> where)
        {
            using (IQuerySession session = store.QuerySession())
            {
                return session.Query<TDomain>().SingleOrDefault(where.Compile());
            }
        }

        public void Save(TDomain instance)
        {
            using (IDocumentSession session = store.LightweightSession())
            {
                session.Store(instance);

                session.SaveChanges();
            }
        }

        public void Update(TDomain instance)
        {
            using (IDocumentSession session = store.LightweightSession())
            {
                session.Store(instance);

                session.SaveChanges();
            }
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MultiTenant.Helpers;
using MultiTenant.Web.Data.Interfaces;
using MultiTenant.Web.Repositories.Interfaces;
using System.Linq;
using System.Reflection;

namespace MultiTenant.Web.Repositories
{
    public class RepositoryBase<TEntity, TContext> : IRepositoryBase<TEntity, TContext> 
        where TEntity : class, new()
        where TContext : DbContext
    {
        private readonly HttpContext _http;
        protected readonly TContext _ctx;

        public RepositoryBase(TContext context, IHttpContextAccessor httpContextAccessor)
        {
            this._ctx = context;
            this._http = httpContextAccessor.HttpContext;
        }

        public virtual IQueryable<TEntity> GetAll()
        {
            var tenantId = ClaimsHelper.GetClaim<int>(_http.User, "Tenant");

            var query = _ctx.Set<TEntity>().AsNoTracking().AsQueryable();

            if (typeof(ITenant).GetTypeInfo().IsAssignableFrom(typeof(TEntity)))
                query = query.Where(i => (i as ITenant).TenantId == tenantId);

            return query;
        }

        public virtual TEntity Update(TEntity entity)
        {
            if (entity is ITenant)
                (entity as ITenant).TenantId = ClaimsHelper.GetClaim<int>(_http.User, "Tenant");

            var entry = _ctx.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                var keyName = _ctx.Model.FindEntityType(typeof(TEntity)).FindPrimaryKey()
                    .Properties.Select(x => x.Name).Single();
                var id = entity.GetType().GetProperty(keyName).GetValue(entity, null);

                var currentEntity = _ctx.Set<TEntity>().Find(id);
                if (currentEntity != null)
                {
                    var attachedEntry = _ctx.Entry(currentEntity);
                    attachedEntry.CurrentValues.SetValues(entity);
                    attachedEntry.State = EntityState.Modified;
                }
                else
                {
                    _ctx.Set<TEntity>().Attach(entity);
                    entry.State = EntityState.Modified;
                }
            }

            _ctx.SaveChanges();

            return entity;
        }
    }
}

using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace MultiTenant.Web.Repositories.Interfaces
{
    public interface IRepositoryBase<TEntity, TContext>
        where TEntity : class, new()
        where TContext : DbContext
    {
        IQueryable<TEntity> GetAll();
        TEntity Update(TEntity entity);
    }
}
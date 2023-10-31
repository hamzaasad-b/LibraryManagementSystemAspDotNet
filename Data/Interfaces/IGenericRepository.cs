using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Data.Interfaces
{
    internal interface IGenericRepository<TEntity, TKey>
    {
        Task<TEntity> Add(TEntity entity);
        Task<bool> Delete(TKey id);
        Task<TEntity> Update(TEntity entity);
        Task<IEnumerable<TEntity>> GetAll();
        Task<TEntity> GetById(TKey id);

        Task<TEntity> Find(Expression<Func<TEntity, bool>> filter);

        Task<IEnumerable<TEntity>> GetWithPagination(Expression<Func<TEntity, bool>> filter, int pageSize = 10,
            int pageNumber = 1);
    }
}

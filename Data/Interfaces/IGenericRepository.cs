using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Data.Dto;

namespace Data.Interfaces
{
    internal interface IGenericRepository<TEntity, TKey>
    {
        Task<TEntity> Add(TEntity entity);
        Task<bool> Delete(TKey id);
        Task<TEntity> Update(TEntity entity);
        Task<IEnumerable<TEntity>> GetAll();
        Task<IEnumerable<TEntity>> GetAll(Expression<Func<TEntity, bool>> filter);
        Task<TEntity?> GetById(TKey id);

        Task<TEntity?> Find(Expression<Func<TEntity, bool>> filter);

        Task<PaginationDto<TEntity>> GetWithPagination(Expression<Func<TEntity, bool>> filter, int pageSize = 10,
            int pageNumber = 1);
        Task<PaginationDto<TEntity>> GetAllWithPagination(int pageSize = 10,
            int pageNumber = 1);
    }
}
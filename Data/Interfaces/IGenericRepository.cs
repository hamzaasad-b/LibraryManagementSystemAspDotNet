using System.Linq.Expressions;
using Data.Dto;

namespace Data.Interfaces
{
    internal interface IGenericRepository<TEntity, TDto, TKey>
    {
        Task<TDto> Add(TDto data);
        Task<IEnumerable<TDto>> AddMultiple(IEnumerable<TDto> entities);
        Task<bool> Delete(TKey id);

        Task<TDto> Update(TKey id, TDto entity);

        // to be removed
        Task<IEnumerable<TDto>> GetAll();

        // to be removed
        Task<IEnumerable<TDto>> GetAllWithFilters(Expression<Func<TEntity, bool>> filter);
        Task<TDto?> GetById(TKey id);
        Task<TDto?> Find(Expression<Func<TEntity, bool>> filter);

        Task<PaginationDto<TDto>> GetFilteredWithPagination(Expression<Func<TEntity, bool>> filter,
            int pageSize = 10,
            int pageNumber = 1);

        Task<PaginationDto<TDto>> GetAllWithPagination(int pageSize = 10,
            int pageNumber = 1);
    }
}
using System.Linq.Expressions;
using Data.Dto;
using Data.Entities;
using Data.Repositories;
using Domain.Common;

namespace Domain.Services;

public class BaseService<TEntity> where TEntity : class
{
    protected GenericRepository<TEntity> BaseRepository { get; }

    protected BaseService(GenericRepository<TEntity> repository)
    {
        BaseRepository = repository;
    }

    public async Task<ServiceResult<IEnumerable<TEntity>>> GetAllEntities()
    {
        return ServiceResult<IEnumerable<TEntity>>.SuccessfulFactory(
            await BaseRepository.GetAll());
    }

    public async Task<ServiceResult<PaginationDto<TEntity>>> GetEntitiesWithPagination(
        Expression<Func<TEntity, bool>>? filter,
        int pageNumber,
        int pageSize)
    {
        if (filter is null)
        {
            return ServiceResult<PaginationDto<TEntity>>.SuccessfulFactory(
                await BaseRepository.GetAllWithPagination(pageSize, pageNumber));
        }

        return ServiceResult<PaginationDto<TEntity>>.SuccessfulFactory(
            await BaseRepository.GetFilteredWithPagination(filter, pageSize, pageNumber));
    }

    public async Task<ServiceResult<TEntity>> AddTEntity(TEntity entity)
    {
        return ServiceResult<TEntity>.SuccessfulFactory(
            await BaseRepository.Add(entity));
    }

    public async Task<ServiceResult<TEntity?>> DeleteTEntity(uint entityId)
    {
        if (await BaseRepository.Delete(entityId))
        {
            return ServiceResult<TEntity?>.SuccessfulFactory();
        }

        return ServiceResult<TEntity?>.FailedFactory("Failed to Delete");
    }
}
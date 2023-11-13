using System.Linq.Expressions;
using Data.Dto;
using Data.Repositories;
using Domain.Common;

namespace Domain.Services;

public class BaseService<TEntity, TDto> where TEntity : class
    where TDto : class
{
    protected GenericRepository<TEntity, TDto> BaseRepository { get; }

    protected BaseService(GenericRepository<TEntity, TDto> repository)
    {
        BaseRepository = repository;
    }

    public async Task<ServiceResult<IEnumerable<TDto>>> GetAllEntities()
    {
        return ServiceResult<IEnumerable<TDto>>.SuccessfulFactory(
            await BaseRepository.GetAll());
    }

    public async Task<ServiceResult<PaginationDto<TDto>>> GetEntitiesWithPagination(
        Expression<Func<TEntity, bool>>? filter,
        int pageNumber,
        int pageSize)
    {
        if (filter is null)
        {
            return ServiceResult<PaginationDto<TDto>>.SuccessfulFactory(
                await BaseRepository.GetAllWithPagination(pageSize, pageNumber));
        }

        return ServiceResult<PaginationDto<TDto>>.SuccessfulFactory(
            await BaseRepository.GetFilteredWithPagination(filter, pageSize, pageNumber));
    }

    public async Task<ServiceResult<TDto>> AddTEntity(TDto entity)
    {
        return ServiceResult<TDto>.SuccessfulFactory(
            await BaseRepository.Add(entity));
    }

    public async Task<ServiceResult<TDto>> Update(uint id, TDto entity)
    {
        var res = await BaseRepository.GetById(id);
        if (res is null)
        {
            return ServiceResult<TDto>.FailedFactory("Entity not found");
        }

        return ServiceResult<TDto>.SuccessfulFactory(
            await BaseRepository.Update(id, entity));
    }

    public async Task<ServiceResult<TDto?>> DeleteTEntity(uint entityId)
    {
        if (await BaseRepository.Delete(entityId))
        {
            return ServiceResult<TDto?>.SuccessfulFactory();
        }

        return ServiceResult<TDto?>.FailedFactory("Failed to Delete");
    }

    public async Task<ServiceResult<TDto?>> GetById(uint entityId)
    {
        return ServiceResult<TDto?>.SuccessfulFactory(await BaseRepository.GetById(entityId));
    }
}
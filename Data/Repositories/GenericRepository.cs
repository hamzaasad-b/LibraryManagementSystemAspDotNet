using System.Linq.Expressions;
using AutoMapper;
using Data.Context;
using Data.Dto;
using Data.Interfaces;
using Data.Transactions;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;

public class GenericRepository<TEntity, TDto> : IGenericRepository<TEntity, TDto, uint>
    where TEntity : class
    where TDto : class
{
    protected LmsDbContext Context { get; }

    protected IMapper Mapper { get; }

    public GenericRepository(LmsDbContext context, IMapper mapper)
    {
        Context = context;
        Mapper = mapper;
    }


    /// <summary>
    /// It adds an entity to database
    /// </summary>
    /// <param name="dto">The Entity to add</param>
    /// <returns>Added Entity to Database</returns>
    /// <exception>Exception thrown by ef core
    ///     <cref>DpUpdateException</cref>
    /// </exception>
    /// <exception>Exception thrown by ef core
    ///     <cref>DpUpdateConcurrencyException</cref>
    /// </exception>
    /// <exception>Exception thrown by ef core
    ///     <cref>OperationCancelledException</cref>
    /// </exception>
    public async Task<TDto> Add(TDto dto)
    {
        var entity = await Context.Set<TEntity>().AddAsync(Mapper.Map<TEntity>(dto));
        await Context.SaveChangesAsync();
        return Mapper.Map<TDto>(entity.Entity);
    }

    /// <summary>
    /// It adds an entity to database
    /// </summary>
    /// <param name="dto">The Entity to add</param>
    /// <returns>Added Entity to Database</returns>
    /// <exception>Exception thrown by ef core
    ///     <cref>DpUpdateException</cref>
    /// </exception>
    /// <exception>Exception thrown by ef core
    ///     <cref>DpUpdateConcurrencyException</cref>
    /// </exception>
    /// <exception>Exception thrown by ef core
    ///     <cref>OperationCancelledException</cref>
    /// </exception>
    public async Task<IEnumerable<TDto>> AddMultiple(IEnumerable<TDto> dto)
    {
        var entityList = Mapper.Map<List<TEntity>>(dto);
        await Context.Set<TEntity>().AddRangeAsync(entityList);
        await Context.SaveChangesAsync();
        return Mapper.Map<List<TDto>>(entityList);
    }


    /// <summary>
    /// It deletes an entity from database
    /// </summary>
    /// <param name="id">The Id of the entity to delete</param>
    /// <returns>Added Entity to Database</returns>
    /// <exception>Exception thrown by ef core
    ///     <cref>DpUpdateException</cref>
    /// </exception>
    /// <exception>Exception thrown by ef core
    ///     <cref>DpUpdateConcurrencyException</cref>
    /// </exception>
    /// <exception>Exception thrown by ef core
    ///     <cref>OperationCancelledException</cref>
    /// </exception>
    public async Task<bool> Delete(uint id)
    {
        var entity = await Context.Set<TEntity>().FindAsync(id);
        if (entity is null) throw new ArgumentException("Invalid Id");
        Context.Set<TEntity>().Remove(entity);
        return await Context.SaveChangesAsync() > 0;
    }

    /// <summary>
    /// It Updates an entity in database
    /// </summary>
    /// <param name="id">Id of entity</param>
    /// <param name="dto">The Entity to add</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>Added Entity to Database</returns>
    /// <exception>Exception thrown by ef core
    ///     <cref>DpUpdateException</cref>
    /// </exception>
    /// <exception>Exception thrown by ef core
    ///     <cref>DpUpdateConcurrencyException</cref>
    /// </exception>
    /// <exception>Exception thrown by ef core
    ///     <cref>OperationCancelledException</cref>
    /// </exception>
    public async Task<TDto> Update(uint id, TDto dto)
    {
        var existing = await Context.Set<TEntity>().FindAsync(id);
        if (existing is null)
        {
            throw new ArgumentException("Invalid Id");
        }

        var entity = Mapper.Map(dto, existing);

        if (entity is null)
        {
            throw new ArgumentNullException(nameof(entity));
        }
        Context.Set<TEntity>().Update(entity);
        await Context.SaveChangesAsync();
        return Mapper.Map<TDto>(entity);
    }


    /// <summary>
    /// Get All records from database 
    /// </summary>
    /// <param name="id">Id of the entity to find</param>
    /// <returns>Entity Found or Null</returns>
    public async Task<TDto?> GetById(uint id)
    {
        var entity = await Context.Set<TEntity>().FindAsync(id);
        return entity is null
            ? null
            : Mapper.Map<TDto>(entity);
    }

    /// <summary>
    /// Find a record with given conditions from database
    /// </summary>
    /// <param name="filter">conditions in form of query predicate</param>
    /// <returns>Entity Found or Null</returns>
    public async Task<TDto?> Find(Expression<Func<TEntity, bool>> filter)
    {
        var entity = await Context.Set<TEntity>().Where(filter).FirstOrDefaultAsync();
        return entity is null
            ? null
            : Mapper.Map<TDto>(entity);
    }
    
    // TODO: Remove
    /// <summary>
    /// Find all records with given conditions from database
    /// </summary>
    /// <param name="filter">conditions in form of query predicate</param>
    /// <returns>List of records</returns>
    public async Task<IEnumerable<TDto>> GetAll()
    {
        return Mapper.Map<List<TDto>>(await Context.Set<TEntity>().ToListAsync());
    }
    
    
    // TODO: Remove
    /// <summary>
    /// Find all records with given conditions from database
    /// </summary>
    /// <param name="filter">conditions in form of query predicate</param>
    /// <returns>List of records</returns>
    public async Task<IEnumerable<TDto>> GetAllWithFilters(Expression<Func<TEntity, bool>> filter)
    {
        return Mapper.Map<List<TDto>>(await Context.Set<TEntity>().Where(filter).ToListAsync());
    }


    /// <summary>
    /// Get Paginated Result  
    /// </summary>
    /// <param name="filter">Filter query predicate</param>
    /// <param name="pageSize">page size</param>
    /// <param name="pageNumber">page number</param>
    /// <returns>A List of Records</returns>
    public async Task<PaginationDto<TDto>> GetFilteredWithPagination(Expression<Func<TEntity, bool>> filter,
        int pageSize = 10,
        int pageNumber = 1)
    {
        var query = Context.Set<TEntity>().Where(filter);
        return new PaginationDto<TDto>(
            Mapper.Map<List<TDto>>(await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync()),
            pageNumber,
            pageSize,
            await query.CountAsync()
        );
    }

    /// <summary>
    /// Get Paginated Result Without Filters
    /// </summary>
    /// <param name="pageSize">page size</param>
    /// <param name="pageNumber">page number</param>
    /// <returns>A List of Records</returns>
    public async Task<PaginationDto<TDto>> GetAllWithPagination(int pageSize = 10,
        int pageNumber = 1)
    {
        var query = Context.Set<TEntity>();
        return new PaginationDto<TDto>(
            Mapper.Map<List<TDto>>(await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync()),
            pageNumber,
            pageSize,
            await query.CountAsync()
        );
    }

    public Transaction GetTransaction()
    {
        return new Transaction(Context);
    }
}
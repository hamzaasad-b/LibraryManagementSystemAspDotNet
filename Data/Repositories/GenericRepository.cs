using System.Linq.Expressions;
using Data.Dto;
using Data.Interfaces;
using Data.Transactions;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;

public class GenericRepository<TEntity> : IGenericRepository<TEntity, uint>
    where TEntity : class
{
    protected DbContext Context { get; }

    public GenericRepository(DbContext context)
    {
        Context = context;
    }


    /// <summary>
    /// It adds an entity to database
    /// </summary>
    /// <param name="entity">The Entity to add</param>
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
    public async Task<TEntity> Add(TEntity entity)
    {
        var addedEntity = await Context.Set<TEntity>().AddAsync(entity);
        await Context.SaveChangesAsync();
        return addedEntity.Entity;
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
    /// <param name="entity">The Entity to add</param>
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
    public async Task<TEntity> Update(TEntity entity)
    {
        var updatedEntity = Context.Set<TEntity>().Update(entity);
        await Context.SaveChangesAsync();
        return updatedEntity.Entity;
    }


    /// <summary>
    /// Get All records from database 
    /// </summary>
    /// <returns>Added Entity to Database</returns>
    /// <exception>Exception thrown by ef core
    ///     <cref>ArgumentNullException</cref>
    /// </exception>
    /// <exception>Exception thrown by ef core
    ///     <cref>OperationCancelledException</cref>
    /// </exception>
    public async Task<IEnumerable<TEntity>> GetAll()
    {
        return await Context.Set<TEntity>().ToListAsync();
    }

    /// <summary>
    /// Get All records from database 
    /// </summary>
    /// <param name="id">Id of the entity to find</param>
    /// <returns>Entity Found or Null</returns>
    public async Task<TEntity?> GetById(uint id)
    {
        return await Context.Set<TEntity>().FindAsync(id);
    }

    /// <summary>
    /// Find a record with given conditions from database
    /// </summary>
    /// <param name="filter">conditions in form of query predicate</param>
    /// <returns>Entity Found or Null</returns>
    public async Task<TEntity?> Find(Expression<Func<TEntity, bool>> filter)
    {
        return await Context.Set<TEntity>().Where(filter).FirstOrDefaultAsync();
    }

    /// <summary>
    /// Find all records with given conditions from database
    /// </summary>
    /// <param name="filter">conditions in form of query predicate</param>
    /// <returns>List of records</returns>
    public async Task<IEnumerable<TEntity>> GetAllWithFilters(Expression<Func<TEntity, bool>> filter)
    {
        return await Context.Set<TEntity>().Where(filter).ToListAsync();
    }


    /// <summary>
    /// Get Paginated Result  
    /// </summary>
    /// <param name="filter">Filter query predicate</param>
    /// <param name="pageSize">page size</param>
    /// <param name="pageNumber">page number</param>
    /// <returns>A List of Records</returns>
    public async Task<PaginationDto<TEntity>> GetFilteredWithPagination(Expression<Func<TEntity, bool>> filter,
        int pageSize = 10,
        int pageNumber = 1)
    {
        var query = Context.Set<TEntity>().Where(filter).AsQueryable();
        return new PaginationDto<TEntity>(
            await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(),
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
    public async Task<PaginationDto<TEntity>> GetAllWithPagination(int pageSize = 10,
        int pageNumber = 1)
    {
        var query = Context.Set<TEntity>().AsQueryable();
        return new PaginationDto<TEntity>(
            await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(),
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
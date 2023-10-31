using System.Linq.Expressions;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    internal class GenericRepository<TEntity> : IGenericRepository<TEntity, uint> 
        where TEntity : class
    {
        protected DbContext Context { get; }

        public GenericRepository(DbContext context)
        {
            Context = context;
        }



        public async Task<TEntity> Add(TEntity entity)
        {
            await Context.Set<TEntity>().AddAsync(entity);
            var res = await Context.SaveChangesAsync();
            if (res > 0)
            {
                return entity;
            }
            throw new Exception("Unable to Add Entity");
        }

        public async Task<bool> Delete(uint id)
        {
            var entity = await Context.Set<TEntity>().FindAsync(id);
            if (entity is null) throw new HttpNotFoundException();
            Context.Set<TEntity>().Remove(entity);
            return await Context.SaveChangesAsync() > 0;
        }

        public async Task<TEntity> Update(TEntity entity)
        {
            var updatedEntity = Context.Set<TEntity>().Update(entity);
            var res = await Context.SaveChangesAsync();
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TEntity>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> GetById(uint id)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> Find(Expression<Func<TEntity, bool>> filter)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TEntity>> GetWithPagination(Expression<Func<TEntity, bool>> filter, int pageSize = 10,
            int pageNumber = 1)
        {
            throw new NotImplementedException();
        }
    }
}

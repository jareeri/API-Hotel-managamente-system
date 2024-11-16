using Hotel.Context;
using Hotel.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Generic
{
    public class Genric<T> : IGenric<T> where T : class
    {
        private readonly DataContext context;

        public Genric(DataContext _context)
        {
            context = _context;
        }

        // Add a new entity and return it after saving
        public async Task<T> Add(T obj)
        {
            await context.Set<T>().AddAsync(obj);
            await context.SaveChangesAsync();
            return obj;
        }

        // Get a single entity by ID
        public async Task<T> Get(int id)
        {
            T entity = await context.Set<T>().FindAsync(id);
            if (entity == null)
            {
                throw new KeyNotFoundException($"Entity with id {id} was not found.");
            }
            return entity;
        }

        // Get all entities
        public async Task<List<T>> GetAll()
        {
            return await context.Set<T>().ToListAsync();
        }

        // Remove an entity by ID and return the removed entity
        public async Task<T> Remove(int id)
        {
            T entity = await context.Set<T>().FindAsync(id);
            if (entity == null)
            {
                throw new KeyNotFoundException($"Entity with id {id} was not found.");
            }

            context.Set<T>().Remove(entity);
            await context.SaveChangesAsync();
            return entity;
        }

        // Update an entity and return it after saving
        public async Task<T> Update(T entity)
        {
            if (!context.Set<T>().Local.Contains(entity))
            {
                context.Set<T>().Attach(entity);
            }

            context.Entry(entity).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return entity;
        }

        // Optional method to perform multiple operations in a transaction
        public async Task ExecuteInTransaction(Func<Task> operations)
        {
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                await operations();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}

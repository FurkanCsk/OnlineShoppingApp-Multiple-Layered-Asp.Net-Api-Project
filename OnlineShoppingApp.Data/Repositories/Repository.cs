using Microsoft.EntityFrameworkCore;
using OnlineShoppingApp.Data.Context;
using OnlineShoppingApp.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingApp.Data.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity>
        where TEntity : BaseEntity
    {
        private readonly OnlineShoppingAppDbContext _db; // Database context for accessing data
        private readonly DbSet<TEntity> _dbSet; // Set of entities of type TEntity

        public Repository(OnlineShoppingAppDbContext db)
        {
            _db = db; // Initialize the database context
            _dbSet = _db.Set<TEntity>(); // Get the DbSet for the entity type
        }


        public void Add(TEntity entity)
        {
            entity.CreatedDate = DateTime.Now; // Set the creation date
            _dbSet.Add(entity); // Add the entity to the DbSet


        }

        public void Delete(TEntity entity, bool softDelete = true)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity), "The entity to delete cannot be null.");
            }

            if (softDelete)
            {
                entity.ModifiedDate = DateTime.Now; // Set the modification date
                entity.IsDeleted = true; // Mark the entity as deleted
                _dbSet.Update(entity); // Update the entity in the DbSet
            }
            else
            {
                _dbSet.Remove(entity); // Permanently remove the entity
            }
        }

        public void Delete(int id)
        {
            var entity = _dbSet.Find(id); // Find the entity by its ID
            Delete(entity); // Call the overloaded Delete method
        }

        public TEntity Get(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbSet.FirstOrDefault(predicate); // Retrieve the first entity that matches the predicate
        }

        public IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate = null)
        {
            // Return all entities or those that match the provided predicate
            return predicate is null ? _dbSet : _dbSet.Where(predicate);
        }

        public TEntity GetById(int id)
        {
            return _dbSet.Find(id); // Find and return the entity by its ID
        }

        public void Update(TEntity entity)
        {
            entity.ModifiedDate = DateTime.Now; // Set the modification date
            _dbSet.Update(entity); // Update the entity in the DbSet

        }

        public async Task<bool> UserExistAsync(int userId)
        {
            return await _db.Users.AnyAsync(x => x.Id == userId);  // Check if a user exists with the given ID
        }
    }
}

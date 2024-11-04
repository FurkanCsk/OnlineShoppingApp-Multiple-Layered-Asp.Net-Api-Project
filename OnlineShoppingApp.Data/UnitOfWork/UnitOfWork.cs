using Microsoft.EntityFrameworkCore.Storage;
using OnlineShoppingApp.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingApp.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly OnlineShoppingAppDbContext _db; // The database context for managing entity framework operations
        private IDbContextTransaction _dbTransaction; // Transaction object for managing database transactions

        public UnitOfWork(OnlineShoppingAppDbContext db)
        {
            _db = db;   // Initialize the database context
        }

        public async Task BeginTransaction()
        {
            _dbTransaction = await _db.Database.BeginTransactionAsync(); // Start a newdatabase transaction
        }

        public async Task CommitTransaction()
        {
            await _dbTransaction.CommitAsync(); // Commit the current transaction to the database
        }

        public void Dispose()
        {
            _db.Dispose();  // Dispose of the database context when done
        }

        public async Task RollBackTransaction()
        {
            await _dbTransaction.RollbackAsync(); // Roll back the current transaction if an error occurs
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _db.SaveChangesAsync(); // Save all changes made in the context to the database
        }
    }
}

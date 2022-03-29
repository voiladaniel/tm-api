using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TravianManager.Core.Data;

namespace TravianManager.Core.Context
{
    public interface IEntityFrameworkDbContext
    {
        /// <summary>
        /// Get items from DB.
        /// </summary>
        /// <typeparam name="TEntity">Object type.</typeparam>
        /// <returns>The objects from table with TEntity type.</returns>
        DbSet<TEntity> Set<TEntity>() where TEntity : class;

        /// <summary>
        /// Save changes to DB.
        /// </summary>
        /// <returns>Return if saved or not.</returns>
        int SaveChanges();

        Task<int> SaveChangesAsync();

    }
}

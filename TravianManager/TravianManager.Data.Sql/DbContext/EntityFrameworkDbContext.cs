// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityFrameworkDbContext.cs" company="LEGO System A/S">
//   Copyright (c) LEGO System A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the Entity Framework Database Context.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TravianManager.Data.Sql.DbContext
{
    using Core.DbContext;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The EntityFramework Database Context.
    /// </summary>
    public sealed class EntityFrameworkDbContext : DbContext, IEntityFrameworkDbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityFrameworkDbContext"/> class.
        /// </summary>
        /// <param name="options"> The options for DataContext.</param>
        public EntityFrameworkDbContext(DbContextOptions options) : base(options)
        {
            Database.SetCommandTimeout(0);
        }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DapperDbContext.cs" company="LEGO System A/S">
//   Copyright (c) LEGO System A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the Dapper Database Context.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TravianManager.Data.Sql.DbContext
{
    using System.Data;
    using Core.DbContext;

    /// <summary>
    /// The Dapper Database Context.
    /// </summary>
    public class DapperDbContext : IDapperDbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DapperDbContext"/> class.
        /// </summary>
        /// <param name="connection"> The configuration of the project.</param>
        public DapperDbContext(IDbConnection connection)
        {
            this.Connection = connection;
        }

        /// <summary>
        /// Gets or sets the Database Connection.
        /// </summary>
        public IDbConnection Connection { get; set; }
    }
}

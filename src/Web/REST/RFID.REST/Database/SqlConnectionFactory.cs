﻿namespace RFID.REST.Database
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Factory for creating new sql connections
    /// </summary>
    public class SqlConnectionFactory
    {
        private readonly SqlConnectionString connectionString;

        /// <summary>
        /// Creates new instance
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        public SqlConnectionFactory(SqlConnectionString connectionString)
        {
            this.connectionString = connectionString;
        }

        /// <summary>
        /// Creates new connection
        /// </summary>
        /// <param name="open">If true opens the connection asynchronously</param>
        /// <returns></returns>
        public async Task<IDbConnection> CreateConnectionAsync(bool open = false)
        {
            var connection = new SqlConnection(this.connectionString.Value);
            if (open)
            {
                await connection.OpenAsync();
            }

            return connection;
        }
    }
}

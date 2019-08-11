namespace RFID.REST.Database
{
    using Dapper;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;

    public static class DatabaseExtensions
    {
        public static async Task<T> ExecuteStoreProcedureAsync<T>(this IDbTransaction transaction, String name, Object param)
        {
            return await transaction.Connection.ExecuteScalarAsync<T>(
                sql: name,
                param: param,
                transaction: transaction,
                commandType: CommandType.StoredProcedure
            );
        }

        public static async Task<int> ExecuteStoreProcedureAsync(this IDbTransaction transaction, String name, Object param)
        {
            return await transaction.Connection.ExecuteScalarAsync<int>(
                sql: name,
                param: param,
                transaction: transaction,
                commandType: CommandType.StoredProcedure
            );
        }
    }
}

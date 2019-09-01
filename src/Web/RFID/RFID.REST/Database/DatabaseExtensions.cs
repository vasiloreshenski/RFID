namespace RFID.REST.Database
{
    using Dapper;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
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

        public static async Task<T> ExecuteScalarAsync<T>(this IDbTransaction transaction, String sql, Object param)
        {
            return await transaction.Connection.ExecuteScalarAsync<T>(
                sql: sql,
                param: param,
                transaction: transaction
            );
        }

        public static int Identity(this DynamicParameters dparams) => dparams.Get<int>("identity");

        public static IReadOnlyCollection<int> Ints<T>(this IReadOnlyCollection<T> enums) where T: struct, IConvertible
        {
            return enums.Select(x => x.ToInt32(CultureInfo.InvariantCulture)).ToList();
        }

        public static IReadOnlyCollection<int> Ints(this Enum @enum)
        {
            var ints = (from e in Enum.GetValues(@enum.GetType()).Cast<Enum>()
                        where @enum.HasFlag(e)
                        select e)
                       .Cast<int>()
                       .ToList();
            
            return ints;
        }
    }
}

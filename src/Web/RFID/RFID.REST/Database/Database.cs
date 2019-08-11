namespace RFID.REST.Database
{
    using RFID.REST.Models.Common;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using Dapper;

    /// <summary>
    /// Class used to encapsulate the communication with the database stored procedures
    /// </summary>
    public class Database
    {
        /// <summary>
        /// Registers a user with the provided username if does not exists already and returns the Id of the user
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="transaction">Transaction</param>
        /// <returns></returns>
        public async Task<int> GetIdOrRegisterUserAsync(String username, IDbTransaction transaction)
        {
            var dynamicParams = new DynamicParameters(new { @username = username });
            dynamicParams.Add("user_id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await transaction.ExecuteStoreProcedureAsync<int>(
                name: "[administration].[usp_insert_user_if_not_exists]",
                param: dynamicParams
            );

            return dynamicParams.Get<int>("user_id");
        }

        /// <summary>
        /// Registers a tag with the specified number and access level and associated user if such tag does not exists already.
        /// </summary>
        /// <param name="number">Number of the tag</param>
        /// <param name="userId">User id</param>
        /// <param name="accessLevel">Tag access level</param>
        /// <param name="transaction">Transaction</param>
        /// <returns>True if the tag does not already exists and is added to the database</returns>
        public async Task<bool> RegisterTagIfNotExistsAsync(String number, int userId, TagAccessLevel accessLevel, IDbTransaction transaction)
        {
            var added = await transaction.ExecuteStoreProcedureAsync<bool>(
                name: "[administration].[usp_insert_tag_if_not_exists]",
                param: new { @number = number, @level_id = accessLevel, @is_active = true, @is_deleted = false, @user_id = userId }
            );

            return added;
        }
    }
}

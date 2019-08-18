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
        public async Task<InsertOrUpdDbResult> RegisterUserAsync(String username, IDbTransaction transaction)
        {
            var dynamicParams = new DynamicParameters(new { @username = username });
            dynamicParams.Add("user_id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var inserted = await transaction.ExecuteStoreProcedureAsync<bool>(
                name: "[administration].[usp_insert_user_if_not_exists]",
                param: dynamicParams
            );
            
            return new InsertOrUpdDbResult
            {
                IsInserted = inserted,
                Id = dynamicParams.Get<int>("user_id")
            };
        }

        /// <summary>
        /// Registers a tag with the specified number and access level and associated user if such tag does not exists already
        /// </summary>
        /// <param name="number">Number of the tag</param>
        /// <param name="userId">User id</param>
        /// <param name="accessLevel">Tag access level</param>
        /// <param name="transaction">Transaction</param>
        /// <returns>True if the tag does not already exists and is added to the database</returns>
        public Task<InsertOrUpdDbResult> InsertTagIfNotExistsAsync(String number, int userId, TagAccessLevel accessLevel, IDbTransaction transaction)
        {
            return this.InsertOrUpdateTagAsync(
                number: number,
                userId: userId,
                accessLevel: accessLevel,
                isActive: true,
                isDeleted: false,
                transaction: transaction
            );
        }

        /// <summary>
        /// Updates a tag with the specified values. If all values are NULL no changes made to the database
        /// </summary>
        /// <param name="tagId">Tag id to be updated</param>
        /// <param name="transaction">Transaction</param>
        /// <param name="userId">User id</param>
        /// <param name="isActive">Is active status</param>
        /// <param name="isDeleted">Is deleted status</param>
        /// <param name="accessLevel">Access level</param>
        /// <returns></returns>
        public async Task UpdateTagAsync(
            int tagId, 
            IDbTransaction transaction, 
            int? userId = null, 
            bool? isActive = null, 
            bool? isDeleted = null, 
            TagAccessLevel? accessLevel = null)
        {
            var tagNumber = await this.GetTagNumberByIdAsync(tagId, transaction);

            await this.InsertOrUpdateTagAsync(number: tagNumber, userId: userId, isActive: isActive, isDeleted: isDeleted, accessLevel: accessLevel, transaction: transaction);
        }
        
        
        private async Task<InsertOrUpdDbResult> InsertOrUpdateTagAsync(
            String number,
            IDbTransaction transaction,
            int? userId = null, 
            bool? isActive = null,
            bool? isDeleted = null,
            TagAccessLevel? accessLevel = null)
        {
            var dynamicParams = new DynamicParameters(new { @number = number, @level_id = accessLevel, @is_active = isActive, @is_deleted = isDeleted, @user_id = userId });
            dynamicParams.Add("tag_id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var inserted = await transaction.ExecuteStoreProcedureAsync<bool>(
                name: "[administration].[usp_insert_or_update_tag]",
                param: dynamicParams
            );

            return new InsertOrUpdDbResult
            {
                IsInserted = inserted,
                IsUpdated = inserted == false,
                Id = dynamicParams.Get<int>("tag_id")
            };
        }

        private async Task<String> GetTagNumberByIdAsync(int tagId, IDbTransaction transaction)
        {
            return await transaction.ExecuteScalarAsync<String>("select x.Number from control_access.Tags as x where x.Id=@tagId", param: new { @tagId = tagId });
        }
    }
}

namespace RFID.REST.Database
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using Dapper;
    using RFID.REST.Models;

    /// <summary>
    /// Class used to encapsulate the communication with the database stored procedures
    /// </summary>
    public class Database
    {
        private readonly SqlConnectionFactory connectionFactory;

        public Database(SqlConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }

        /// <summary>
        /// Registers a user with the provided username if does not exists already and returns the Id of the user
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="transaction">Transaction</param>
        /// <returns></returns>
        public async Task<InsertOrUpdDbResult> InsertAccessPointUserAsync(String username, IDbTransaction transaction)
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
        public Task<InsertOrUpdDbResult> InsertTagIfNotExistsAsync(String number, int userId, AccessLevel accessLevel, IDbTransaction transaction)
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
        public async Task<InsertOrUpdDbResult> UpdateTagAsync(
            int tagId,
            IDbTransaction transaction,
            int? userId = null,
            bool? isActive = null,
            bool? isDeleted = null,
            AccessLevel? accessLevel = null)
        {
            var tagNumber = await this.GetTagNumberByIdAsync(tagId, transaction);
            if (String.IsNullOrEmpty(tagNumber))
            {
                return InsertOrUpdDbResult.NotFound;
            }
            else
            {
                return await this.InsertOrUpdateTagAsync(number: tagNumber, userId: userId, isActive: isActive, isDeleted: isDeleted, accessLevel: accessLevel, transaction: transaction);
            }
        }

        /// <summary>
        /// Get the access level for the specified access point
        /// </summary>
        /// <param name="accessPointIdentifier">access point identifier</param>
        /// <returns></returns>
        public async Task<AccessLevel?> GetAccessLevelForAccessPointAsync(Guid accessPointIdentifier)
        {
            using (var connection = await this.connectionFactory.CreateConnectionAsync())
            {
                return await connection.ExecuteScalarAsync<AccessLevel?>("select access_control.f_get_access_level_for_access_point(@identifier)", param: new { @identifier = accessPointIdentifier });
            }
        }

        /// <summary>
        /// Get the access level for the specified tag
        /// </summary>
        /// <param name="tagId">tag id</param>
        /// <returns></returns>
        public async Task<AccessLevel?> GetAccessLevelForTagAsync(int tagId)
        {
            using (var connection = await this.connectionFactory.CreateConnectionAsync())
            {
                return await connection.ExecuteScalarAsync<AccessLevel?>("select access_control.f_get_access_level_for_tag(@tag_id)", param: new { @tag_id = tagId });
            }
        }

        /// <summary>
        /// Inserts access point if does not exists or updates it by identifier. If any of the provided values is null the column won't be updated
        /// </summary>
        /// <param name="identifier">identifier of the access point</param>
        /// <param name="transaction">transaction</param>
        /// <param name="description">description of the access point</param>
        /// <param name="IsActive">is active flag</param>
        /// <param name="accessLevel">access level of the access point</param>
        /// <returns></returns>
        public async Task<InsertOrUpdDbResult> InsertOrUpdateAccessPointAsync(Guid identifier, IDbTransaction transaction, String description = null, bool? IsActive = null, AccessLevel? accessLevel = null)
        {
            var param = new DynamicParameters(new { identifier = identifier, description = description, is_active = IsActive, level_id = accessLevel });
            param.Add("access_point_id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var isInserted = await transaction.ExecuteStoreProcedureAsync<bool>("access_control.usp_insert_or_update_access_point", param: param);

            return InsertOrUpdDbResult.Create(param.Identity(), isInserted);
        }

        /// <summary>
        /// Insert administration user
        /// </summary>
        /// <param name="email">email</param>
        /// <param name="passwordHash">password hash</param>
        /// <param name="roles">roles</param>
        /// <param name="transaction">transaction</param>
        /// <returns></returns>
        public async Task<InsertOrUpdDbResult> InsertAdministrationUserAsync(String email, String passwordHash, IReadOnlyCollection<Areas.User.Models.UserRole> roles, IDbTransaction transaction)
        {
            var param = new DynamicParameters(new { @email = email, @password_hash = passwordHash, @roles = AsIntList(roles.Ints()) });
            param.Add("identity", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await transaction.ExecuteStoreProcedureAsync("administration.usp_insert_user", param: param);

            return InsertOrUpdDbResult.Create(param.Identity(), true);
        }

        private async Task<InsertOrUpdDbResult> InsertOrUpdateTagAsync(
            String number,
            IDbTransaction transaction,
            int? userId = null,
            bool? isActive = null,
            bool? isDeleted = null,
            AccessLevel? accessLevel = null)
        {
            var dynamicParams = new DynamicParameters(new { @number = number, @level_id = accessLevel, @is_active = isActive, @is_deleted = isDeleted, @user_id = userId });
            dynamicParams.Add("tag_id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var inserted = await transaction.ExecuteStoreProcedureAsync<bool>(
                name: "[administration].[usp_insert_or_update_tag]",
                param: dynamicParams
            );

            return InsertOrUpdDbResult.Create(dynamicParams.Identity(), inserted);
        }

        private async Task<String> GetTagNumberByIdAsync(int tagId, IDbTransaction transaction)
        {
            return await transaction.ExecuteScalarAsync<String>("select x.Number from control_access.Tags as x where x.Id=@tagId", param: new { @tagId = tagId });
        }


        private SqlMapper.ICustomQueryParameter AsIntList(IReadOnlyCollection<int> values)
        {
            var datatable = new DataTable();
            datatable.Columns.Add("Value", typeof(int));

            foreach (var value in values)
            {
                datatable.Rows.Add(value);
            }

            return datatable.AsTableValuedParameter("dbo.IntList");
        }
    }
}

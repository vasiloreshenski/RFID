namespace RFID.REST.Database
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using Dapper;
    using RFID.REST.Areas.Administration.Models;
    using RFID.REST.Areas.Auth.Models;
    using RFID.REST.Areas.Log.Models;
    using RFID.REST.Areas.Stat.Models;
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
        public async Task<InsertOrUpdDbResult> InsertAccessPointUserIfNotExistsAsync(String username, IDbTransaction transaction)
        {
            var dynamicParams = new DynamicParameters(new { @username = username });
            dynamicParams.AddIdentity();

            var inserted = await transaction.ExecuteStoreProcedureAsync<bool>(
                name: "[access_control].[usp_insert_user_if_not_exists]",
                param: dynamicParams
            );

            return new InsertOrUpdDbResult
            {
                IsInserted = inserted,
                Id = dynamicParams.Identity()
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
        public async Task<InsertOrUpdDbResult> InsertTagIfNotExistsAsync(String number, int userId, AccessLevel accessLevel, IDbTransaction transaction)
        {
            var dynamicParams = new DynamicParameters(new { @number = number, @user_id = userId, @level_id = accessLevel, @is_active = true, @is_deleted = false });
            dynamicParams.AddIdentity();

            var isInserted = await transaction.ExecuteStoreProcedureAsync<bool>("access_control.usp_insert_tag_if_not_exists", dynamicParams);

            return InsertOrUpdDbResult.Create(dynamicParams.Identity(), isInserted, false);
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
        public async Task<AccessLevel?> GetAccessLevelForAccessPointAsync(String serialNumber)
        {
            using (var connection = await this.connectionFactory.CreateConnectionAsync())
            {
                return await connection.ExecuteScalarAsync<AccessLevel?>(
                    "select top 1 x.LevelId from access_control.AccessPoints as x where x.SerialNumber=@serial_number",
                    param: new { @serial_number = serialNumber }
                );
            }
        }

        /// <summary>
        /// Get the access level for the specified tag
        /// </summary>
        /// <param name="tagId">tag id</param>
        /// <returns></returns>
        public async Task<AccessLevel?> GetAccessLevelForTagAsync(String number)
        {
            using (var connection = await this.connectionFactory.CreateConnectionAsync())
            {
                return await connection.ExecuteScalarAsync<AccessLevel?>("select top 1 x.LevelId from access_control.Tags as x where x.Number=@number", param: new { @number = number });
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
        public async Task<InsertOrUpdDbResult> InsertAccessPointIfNotExistsAsync(
            String serialNumber,
            IDbTransaction transaction,
            String description,
            bool IsActive,
            AccessLevel accessLevel,
            AccessPointDirectionType direction)
        {
            var param = new DynamicParameters(new { serial_number = serialNumber, description = description, is_active = IsActive, level_id = accessLevel, direction_id = direction });
            param.AddIdentity();

            var isInserted = await transaction.ExecuteStoreProcedureAsync<bool>("access_control.usp_insert_access_point_if_not_exists", param: param);

            return InsertOrUpdDbResult.Create(param.Identity(), isInserted, false);
        }

        /// <summary>
        /// Updates the access point with the provided values. Any null values will be ignored during the update
        /// </summary>
        /// <param name="id">access point id</param>
        /// <param name="transaction">transaction</param>
        /// <param name="description">access point description</param>
        /// <param name="isActive">access point is active flag</param>
        /// <param name="accessLevel">access point access level</param>
        /// <returns></returns>
        public async Task<InsertOrUpdDbResult> UpdateAccessPointAsync(
            int id,
            IDbTransaction transaction,
            String description = null,
            bool? isActive = null,
            bool? isDeleted = null,
            AccessLevel? accessLevel = null,
            AccessPointDirectionType? direction = null)
        {
            var serialNumber = await this.GetAccessPointSerialNumberAsync(id, transaction);
            if (String.IsNullOrEmpty(serialNumber))
            {
                return InsertOrUpdDbResult.NotFound;
            }
            else
            {
                var param = new DynamicParameters(new { serial_number = serialNumber, description = description, is_active = isActive, is_deleted = isDeleted, level_id = (int?)accessLevel, direction_id = (int?)direction });
                param.AddIdentity();

                var inserted = await transaction.ExecuteStoreProcedureAsync<bool>("access_control.usp_insert_or_update_access_point", param);

                return InsertOrUpdDbResult.Create(param.Identity(), inserted, inserted == false);
            }
        }

        /// <summary>
        /// Insert administration user
        /// </summary>
        /// <param name="email">email</param>
        /// <param name="passwordHash">password hash</param>
        /// <param name="roles">roles</param>
        /// <param name="transaction">transaction</param>
        /// <returns></returns>
        public async Task<InsertOrUpdDbResult> InsertAdministrationUserAsync(String email, String passwordHash, UserRoles roles, IDbTransaction transaction)
        {
            var param = new DynamicParameters(new { @email = email, @password_hash = passwordHash, @roles = AsIntList(roles.Ints()) });
            param.Add("identity", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var isInserted = await transaction.ExecuteStoreProcedureAsync<bool>("administration.usp_insert_user_if_not_exists", param: param);

            return InsertOrUpdDbResult.Create(param.Identity(), isInserted, isUpdated: false);
        }

        /// <summary>
        /// Returns administration user by email
        /// </summary>
        /// <param name="email">email</param>
        /// <returns></returns>
        public async Task<AdministrationUser> GetAdministrationUserAsync(string email)
        {
            using (var connection = await this.connectionFactory.CreateConnectionAsync())
            {
                var dbUser = await connection.QuerySingleOrDefaultAsync<(String email, String passwordHash, int roleId)>("select x.Email, x.PasswordHash, x.RoleId from administration.f_get_user(@email) as x", param: new { @email = email });
                if (dbUser.Equals(default) == false)
                {
                    return new AdministrationUser(dbUser.email, dbUser.passwordHash, (UserRoles)dbUser.roleId);
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Returns auth user by email
        /// </summary>
        /// <returns></returns>
        public async Task<AuthUser> GeAuthUserAsync(String email)
        {
            if (String.IsNullOrEmpty(email))
            {
                return null;
            }

            using (var connection = await this.connectionFactory.CreateConnectionAsync())
            {
                var dbUser = await connection.QuerySingleOrDefaultAsync<(String email, String refreshToken, String passwordHash, int roleId)>("select x.Email, x.RefreshToken, x.PasswordHash, x.RoleId from administration.f_get_user(@email) as x", param: new { @email = email });
                if (dbUser.Equals(default) == false)
                {
                    return new AuthUser(dbUser.email, dbUser.refreshToken, dbUser.passwordHash, (UserRoles)dbUser.roleId);
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Replaces the current refresh token for the user with the specified one
        /// </summary>
        /// <param name="email">email of the user</param>
        /// <param name="newRefreshToken">new refresh token</param>
        /// <returns></returns>
        public async Task<InsertOrUpdDbResult> ReplaceRefreshTokenAsync(String email, String newRefreshToken, IDbTransaction transaction)
        {
            var dynamicParams = new DynamicParameters(new { @email = email, @refresh_token = newRefreshToken });
            dynamicParams.Add("identity", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var inserted = await transaction.ExecuteStoreProcedureAsync<bool>("administration.usp_replace_refresh_token", param: dynamicParams);

            return InsertOrUpdDbResult.Create(dynamicParams.Identity(), inserted, inserted == false);
        }

        public async Task<InsertOrUpdDbResult> InsertOrUpdateUnknownAccessPointAsync(String serialNumber, IDbTransaction transaction)
        {
            var param = new DynamicParameters();
            param.Add("serial_number", value: serialNumber);
            param.AddIdentity();

            var inserted = await transaction.ExecuteStoreProcedureAsync<bool>("access_control.usp_insert_or_update_unknown_access_point", param);

            return InsertOrUpdDbResult.Create(param.Identity(), inserted, inserted == false);
        }

        public async Task<InsertOrUpdDbResult> DeleteAccessPointAsync(String serialNumber, IDbTransaction transaction)
        {
            var param = new DynamicParameters();
            param.Add("serial_number", serialNumber);
            param.AddIdentity();

            var updated = await transaction.ExecuteStoreProcedureAsync<bool>("access_control.usp_delete_access_point", param);

            return InsertOrUpdDbResult.Create(param.Identity(), false, updated);
        }

        public async Task DeleteUnknownAccessPointAsync(String serialNumber, IDbTransaction transaction)
        {
            var param = new DynamicParameters();
            param.Add("serial_number", value: serialNumber);

            await transaction.ExecuteStoreProcedureAsync("access_control.usp_delete_unknown_access_point", param);
        }

        public async Task<InsertOrUpdDbResult> InsertOrUpdateUnknownTagAsync(String number, IDbTransaction transaction)
        {
            var param = new DynamicParameters();
            param.Add("number", value: number);
            param.AddIdentity();

            var inserted = await transaction.ExecuteStoreProcedureAsync<bool>("access_control.usp_insert_or_update_unknown_tag", param);

            return InsertOrUpdDbResult.Create(param.Identity(), inserted, inserted == false);
        }

        public async Task DeleteUnknownTagAsync(String number, IDbTransaction transaction)
        {
            var param = new DynamicParameters();
            param.Add("number", number);

            await transaction.ExecuteStoreProcedureAsync("access_control.usp_delete_unknown_tag", param);
        }

        public async Task InsertEventAsync(String accessPointSerialNumber, String tagNumber, IDbTransaction transaction)
        {
            await transaction.ExecuteStoreProcedureAsync(
                "stat.usp_insert_event",
                new { @access_point_serial_number = accessPointSerialNumber, @tag_number = tagNumber }
            );
        }

        public async Task InsertClientLogAsync(String message, LogType type, IDbTransaction transaction)
        {
            await transaction.ExecuteStoreProcedureAsync("[log].[insert_client_log]", param: new { @message = message, @type_id = (int)type });
        }


        public async Task<int> GetActiveAccessPointsCountAsync()
        {
            using (var connection = await this.connectionFactory.CreateConnectionAsync())
            {
                var dbResult = await connection.ExecuteScalarAsync<int>(@"select count(*) from access_control.f_get_access_points(null, null, 1, 0) as x");
                return dbResult;
            }
        }

        public async Task<int> GetInActiveAccessPointsCountAsync()
        {
            using (var connection = await this.connectionFactory.CreateConnectionAsync())
            {
                var dbResult = await connection.ExecuteScalarAsync<int>(@"select count(*) from access_control.f_get_access_points(null, null, 0, 0) as x");
                return dbResult;
            }
        }

        public async Task<int> GetDeletedAccessPointsCountAsync()
        {
            using (var connection = await this.connectionFactory.CreateConnectionAsync())
            {
                var dbResult = await connection.ExecuteScalarAsync<int>(@"select count(*) from access_control.f_get_access_points(null, null, null, 1) as x");
                return dbResult;
            }
        }

        public async Task<int> GetUnknownAccessPointsCountAsync()
        {
            using (var connection = await this.connectionFactory.CreateConnectionAsync())
            {
                var dbResult = await connection.ExecuteScalarAsync<int>(@"select count(*) from access_control.f_get_unknown_access_points(null, null) as x");
                return dbResult;
            }
        }

        public Task<IReadOnlyCollection<AccessPointResponseModel>> GetActiveAccessPointsAsync(int? page, int? pageSize)
        {
            return this.GetAccessPointsInternalAsync(page: page, pageSize: pageSize, isActive: true, isDeleted: false);
        }

        public Task<IReadOnlyCollection<AccessPointResponseModel>> GetInActiveAccessPointsAsync(int? page, int? pageSize)
        {
            return this.GetAccessPointsInternalAsync(page: page, pageSize: pageSize, isActive: false, isDeleted: false);
        }

        public Task<IReadOnlyCollection<AccessPointResponseModel>> GetDeletedAccessPointsAsync(int? page, int? pageSize)
        {
            return this.GetAccessPointsInternalAsync(page: page, pageSize: pageSize, isActive: null, isDeleted: true);
        }

        public async Task<IReadOnlyCollection<UnKnownAccessPointResponseModel>> GetUnKnownAccessPointsAsync(int? page, int? pageSize)
        {
            using (var connection = await this.connectionFactory.CreateConnectionAsync())
            {
                var dbResult = await connection.QueryAsync<UnKnownAccessPointResponseModel>(
                    "select x.Id, x.SerialNumber, x.AccessDate from access_control.f_get_unknown_access_points(@page, @page_size) as x",
                    param: new { @page = page, @page_size = pageSize }
                );

                return dbResult.ToList();
            }
        }

        public async Task<int> GetActiveTagsCountAsync()
        {
            using (var connection = await this.connectionFactory.CreateConnectionAsync())
            {
                return await connection.ExecuteScalarAsync<int>("select count(*) from access_control.f_get_tags(null, null, 1, 0)");
            }
        }

        public async Task<int> GetInActiveTagsCountAsync()
        {
            using (var connection = await this.connectionFactory.CreateConnectionAsync())
            {
                return await connection.ExecuteScalarAsync<int>("select count(*) from access_control.f_get_tags(null, null, 0, 0)");
            }
        }

        public async Task<int> GetDeletedTagsCountAsync()
        {
            using (var connection = await this.connectionFactory.CreateConnectionAsync())
            {
                return await connection.ExecuteScalarAsync<int>("select count(*) from access_control.f_get_tags(null, null, null, 1)");
            }
        }

        public async Task<int> GetUnknownTagsCountAsync()
        {
            using (var connection = await this.connectionFactory.CreateConnectionAsync())
            {
                return await connection.ExecuteScalarAsync<int>("select count(*) from access_control.f_get_unknown_tags(null, null)");
            }
        }


        public Task<IReadOnlyCollection<TagResponseModel>> GetActiveTagsAsync(int? page, int? pageSize)
        {
            return this.GetTagsInternalAsync(page, pageSize, true, false);
        }

        public Task<IReadOnlyCollection<TagResponseModel>> GetInActiveTagsAsync(int? page, int? pageSize)
        {
            return this.GetTagsInternalAsync(page, pageSize, false, false);
        }

        public Task<IReadOnlyCollection<TagResponseModel>> GetDeletedTagsAsync(int? page, int? pageSize)
        {
            return this.GetTagsInternalAsync(page, pageSize, null, true);
        }

        public async Task<IReadOnlyCollection<UserResponseModel>> GetTagsUsersAsync()
        {
            using (var connection = await this.connectionFactory.CreateConnectionAsync())
            {
                var dbResult = await connection.QueryAsync<UserResponseModel>("select x.Id, x.[Name] as [UserName] from access_control.f_get_users() as x");

                return dbResult.ToList();
            }
        }

        public async Task<IReadOnlyCollection<UnKnownTagResponseModel>> GetUnKnownTagsAsync(int? page, int? pageSize)
        {
            using (var connection = await this.connectionFactory.CreateConnectionAsync())
            {
                var dbResult = await connection.QueryAsync<UnKnownTagResponseModel>(
                    "select x.Id, x.AccessDate, x.Number from access_control.f_get_unknown_tags(@page, @page_size) as x",
                    param: new { @page = page, @page_size = pageSize }
                );

                return dbResult.ToList();
            }
        }

        public async Task<TimeSpan?> GetUsersAvgEntranceTimeAsync()
        {
            using (var connection = await this.connectionFactory.CreateConnectionAsync())
            {
                var minutes = await connection.ExecuteScalarAsync<int?>("select x.avg_time from stat.f_get_avg_entrance_time_in_minutes() as x");
                return minutes == null ? (TimeSpan?)null : TimeSpan.FromMinutes(minutes.Value);
            }
        }

        public async Task<TimeSpan?> GetUsersAvgExitTimeAsync()
        {
            using (var connection = await this.connectionFactory.CreateConnectionAsync())
            {
                var minutes = await connection.ExecuteScalarAsync<int?>("select x.avg_time from stat.f_get_avg_exit_time_in_minutes() as x");
                return minutes == null ? (TimeSpan?)null : TimeSpan.FromMinutes(minutes.Value);
            }
        }

        public async Task<TimeSpan?> GetUsersAvgWorkHourNormAsync()
        {
            using (var connection = await this.connectionFactory.CreateConnectionAsync())
            {
                var minutes = await connection.ExecuteScalarAsync<int?>("select x.avg_time from stat.f_get_avg_work_hour_norm_in_minutes() as x");
                return minutes == null ? (TimeSpan?)null : TimeSpan.FromMinutes(minutes.Value);
            }
        }

        public async Task<IReadOnlyCollection<UserStatResponseModel>> GetUsersStatAsync()
        {
            using (var connection = await this.connectionFactory.CreateConnectionAsync())
            {
                var dbResult = await connection.QueryAsync<(int id, String userName, int avgEntranceTimeInMinutes, int avgExitTimeInMinutes, int avgWorkHourNormInMinutes)>(
                    "select x.Id, x.UserName, x.AvgEntranceTimeInMinutes, x.AvgExitTimeInMinutes, x.AvgWorkHourNormInMinutes from stat.f_get_users() as x"
                );

                return dbResult.Select(x => new UserStatResponseModel
                {
                    Id = x.id,
                    UserName = x.userName,
                    AvgEntranceTime = TimeSpan.FromMinutes(x.avgEntranceTimeInMinutes),
                    AvgExitTime = TimeSpan.FromMinutes(x.avgExitTimeInMinutes),
                    AvgWorkHourNorm = TimeSpan.FromMinutes(x.avgWorkHourNormInMinutes)
                }).ToList();
            }
        }

        public async Task<IReadOnlyCollection<DateAndTimeResponseModel>> GetUserWorkHourNormForRangeAsync(int userId, DateTime? startDate, DateTime? endDate)
        {
            using (var connection = await this.connectionFactory.CreateConnectionAsync())
            {
                var dbResult = await connection.QueryAsync<(int normInMinutes, DateTime day)>(
                    "select x.norm as [normInMinutes], x.[day] as [day] from stat.f_get_work_hour_norm_in_minutes(@user_id, @start_date, @end_date) as x",
                    param: new { @user_id = userId, start_date = startDate, end_date = endDate }
                );

                return dbResult.Select(x => new DateAndTimeResponseModel { Time = TimeSpan.FromMinutes(x.normInMinutes), Day = x.day }).ToList();
            }
        }

        public async Task<IReadOnlyCollection<DateAndTimeResponseModel>> GetUserEntranceTimeForRangeAsync(int userId, DateTime? startDate, DateTime? endDate)
        {
            using (var connection = await this.connectionFactory.CreateConnectionAsync())
            {
                var dbResult = await connection.QueryAsync<DateTime>("select x.entrance_time from stat.f_get_entrance_time_for_user(@user_id, @start_date, @end_date) as x", param: new { @user_id = userId, @start_date = startDate, @end_date = endDate });
                return dbResult.Select(x => new DateAndTimeResponseModel
                {
                    Day = x.Date,
                    Time = x.TimeOfDay
                }).ToList();
            }
        }

        public async Task<IReadOnlyCollection<DateAndTimeResponseModel>> GetUserExitTimeForRangeAsync(int userId, DateTime? startDate, DateTime? endDate)
        {
            using (var connection = await this.connectionFactory.CreateConnectionAsync())
            {
                var dbResult = await connection.QueryAsync<DateTime>("select x.entrance_time from stat.f_get_exit_time_for_user(@user_id, @start_date, @end_date) as x", param: new { @user_id = userId, @start_date = startDate, @end_date = endDate });
                return dbResult.Select(x => new DateAndTimeResponseModel
                {
                    Day = x.Date,
                    Time = x.TimeOfDay
                }).ToList();
            }
        }

        public async Task<String> GetAccessPointSerialNumberAsync(int accessPointId, IDbTransaction transaction)
        {
            return await transaction.ExecuteScalarAsync<String>("select x.SerialNumber from access_control.AccessPoints as x where x.Id=@access_point_id", param: new { @access_point_id = accessPointId });
        }

        public async Task<bool?> CheckAccessAsync(String accessPointSerialNumber, String tagNumber, IDbTransaction transaction)
        {
            var hasAccess = await transaction.ExecuteScalarAsync<bool?>(
                "select access_control.f_check_access(@access_point_serial_number, @tag_number)",
                new { @access_point_serial_number = accessPointSerialNumber, @tag_number = tagNumber }
            );
            return hasAccess;
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
            dynamicParams.Add("identity", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var inserted = await transaction.ExecuteStoreProcedureAsync<bool>(
                name: "[access_control].[usp_insert_or_update_tag]",
                param: dynamicParams
            );

            return InsertOrUpdDbResult.Create(dynamicParams.Identity(), inserted, inserted == false);
        }

        private async Task<String> GetTagNumberByIdAsync(int tagId, IDbTransaction transaction)
        {
            return await transaction.ExecuteScalarAsync<String>("select x.Number from access_control.Tags as x where x.Id=@tag_id", param: new { @tag_id = tagId });
        }

        private async Task<IReadOnlyCollection<AccessPointResponseModel>> GetAccessPointsInternalAsync(int? page, int? pageSize, bool? isActive, bool? isDeleted)
        {
            using (var connection = await this.connectionFactory.CreateConnectionAsync())
            {
                var dbResult = await connection.QueryAsync<AccessPointResponseModel>(@"
                    select 
                        x.Id, 
                        x.SerialNumber, 
                        x.Description, 
                        x.LevelId as AccessLevel, 
                        x.DirectionId as Direction, 
                        x.CreateDate, 
                        x.ModificationDate, 
                        x.IsActive,
                        x.IsDeleted
                    from access_control.f_get_access_points(@page, @page_size, @is_active, @is_deleted) as x", param: new { @page = page, @page_size = pageSize, @is_active = isActive, @is_deleted = isDeleted });

                return dbResult.ToList();
            }
        }

        private async Task<IReadOnlyCollection<TagResponseModel>> GetTagsInternalAsync(int? page, int? pageSize, bool? isActive, bool? isDeleted)
        {
            using (var connection = await this.connectionFactory.CreateConnectionAsync())
            {
                var dbResult = await connection.QueryAsync<TagResponseModel>(@"
                    select
                        x.Id,
                        x.IsActive,
                        x.IsDeleted,
                        x.LevelId as AccessLevel,
                        x.CreateDate,
                        x.ModificationDate,
                        x.Number,
                        x.UserName
                    from access_control.f_get_tags(@page, @page_size, @is_active, @is_deleted) as x",
                    param: new { @page = page, @page_size = pageSize, @is_active = isActive, @is_deleted = isDeleted });

                return dbResult.ToList();
            }
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

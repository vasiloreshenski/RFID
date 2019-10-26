namespace RFID.REST.Test.Common
{
    using Dapper;
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Text;
    using System.Threading.Tasks;

    internal static class RfidDatabase
    {
        public static async Task<int> GetAdministrationUsersCountAsync()
        {
            using (var connection = CreateConnection())
            {
                return await connection.ExecuteScalarAsync<int>("select count(*) from administration.Users");
            }
        }

        public static async Task<int> GetAccessControlTagsCountAsync()
        {
            using (var connection = CreateConnection())
            {
                return await connection.ExecuteScalarAsync<int>("select count(*) from access_control.Tags");
            }
        }

        public static async Task<int> GetAccessControlUsersCountAsync()
        {
            using (var connection = CreateConnection())
            {
                return await connection.ExecuteScalarAsync<int>("select count(*) from access_control.Users");
            }
        }

        public static async Task<int> GetAccessPointsCountAsync()
        {
            using (var connection = CreateConnection())
            {
                return connection.ExecuteScalar<int>("select count(*) from access_control.AccessPoints");
            }
        }

        public static async Task<int> GetTagIdByNumberAsync(String number)
        {
            using (var connection = CreateConnection())
            {
                return await connection.ExecuteScalarAsync<int>("select x.Id from access_control.Tags as x where x.number = @number", param: new { number });
            }
        }

        public static async Task<int> GetAccessPointIdBySerialNumberAsync(String serialNumber)
        {
            using (var connection = CreateConnection())
            {
                return await connection.ExecuteScalarAsync<int>("select x.Id from access_control.AccessPoints as x where x.SerialNumber=@serial_number", param: new { serial_number = serialNumber });
            }
        }

        public static async Task DeActivateTagAsync(String number)
        {
            using (var connection = CreateConnection())
            {
                await connection.ExecuteAsync("update access_control.Tags set IsActive = 0 where Number = @number", param: new { number });
            }
        }

        public static async Task DeActivateAccessPointAsync(int id)
        {
            using (var connection = CreateConnection())
            {
                await connection.ExecuteAsync("update access_control.AccessPoints set IsActive=0 where Id=@id", param: new { @id = id });
            }
        }

        public static async Task<Dictionary<String, Object>> GetRecordStateByTableNameAsync(String tableName, int id)
        {
            using (var connection = CreateConnection())
            {
                using (var reader = await connection.ExecuteReaderAsync($"select top 1 x.* from {tableName} as x where x.Id = @id", param: new { id }))
                {
                    var result = new Dictionary<String, object>();

                    while (await reader.ReadAsync())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            var name = reader.GetName(i);
                            var value = reader.GetValue(i);

                            result.Add(name, value);
                        }
                    }

                    return result;
                }
            }
        }

        public static SqlConnection CreateConnection()
        {
            return new SqlConnection(Settings.GetDevelopmentConnectionString());
        }
    }
}

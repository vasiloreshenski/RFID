namespace RFID.REST.Areas.Administration.Commands
{
    using RFID.REST.Areas.Administration.Models;
    using RFID.REST.Database;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Command for registering new tags
    /// </summary>
    public class RegisterTagCommand
    {   
        private readonly SqlConnectionFactory sqlConnectionFactory;
        private readonly Database database;

        public RegisterTagCommand(SqlConnectionFactory sqlConnectionFactory, Database database)
        {
            this.sqlConnectionFactory = sqlConnectionFactory;
            this.database = database;
        }

        /// <summary>
        /// Returns true if the tag has been registered successfully or false if the tag is already registered
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool> RegisterAsync(RegisterTagRequestModel model)
        {
            using (var transaction = await this.sqlConnectionFactory.CreateTransactionAsync())
            {
                var userDbResult = await this.database.RegisterUserAsync(model.User.Name, transaction);
                var tagDbResult = await this.database.InsertTagIfNotExistsAsync(model.Number, userDbResult.Id, model.AccessLevel, transaction);

                return tagDbResult.IsInserted;
            }
        }
    }
}

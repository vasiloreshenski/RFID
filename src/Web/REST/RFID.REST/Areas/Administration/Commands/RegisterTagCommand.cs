namespace RFID.REST.Areas.Administration.Commands
{
    using RFID.REST.Areas.Administration.Models;
    using RFID.REST.Database;
    using RFID.REST.Models;
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
        public async Task<CommandResult> RegisterAsync(RegisterTagRequestModel model)
        {
            using (var connection = await this.sqlConnectionFactory.CreateConnectionAsync(true))
            using (var transaction = connection.BeginTransaction())
            {
                var userDbResult = await this.database.InsertAccessPointUserIfNotExistsAsync(model.UserName, transaction);
                var tagDbResult = await this.database.InsertTagIfNotExistsAsync(model.Number, userDbResult.Id, model.AccessLevel, transaction);

                if (tagDbResult.IsInserted)
                {
                    await this.database.DeleteUnknownTagAsync(model.Number, transaction);
                }

                transaction.Commit();

                return CommandResult.FromDbResult(tagDbResult);
            }
        }
    }
}

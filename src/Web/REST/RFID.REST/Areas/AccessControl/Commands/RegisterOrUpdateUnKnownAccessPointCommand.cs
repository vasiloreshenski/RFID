namespace RFID.REST.Areas.AccessControl.Commands
{
    using RFID.REST.Database;
    using RFID.REST.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    internal class RegisterOrUpdateUnKnownAccessPointCommand
    {
        private readonly Database database;
        private readonly SqlConnectionFactory sqlConnectionFactory;

        public RegisterOrUpdateUnKnownAccessPointCommand(Database database, SqlConnectionFactory sqlConnectionFactory)
        {
            this.database = database;
            this.sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task<CommandResult> RegisterOrUpdateAsync(String serialNumber)
        {
            using (var connection = await this.sqlConnectionFactory.CreateConnectionAsync(true))
            using (var transaction = connection.BeginTransaction())
            {
                var dbResult = await this.database.InsertOrUpdateUnknownAccessPointAsync(serialNumber, transaction);

                transaction.Commit();

                return CommandResult.FromDbResult(dbResult);
            }
        }
    }
}

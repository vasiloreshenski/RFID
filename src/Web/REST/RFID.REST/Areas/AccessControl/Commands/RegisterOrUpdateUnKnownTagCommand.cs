namespace RFID.REST.Areas.AccessControl.Commands
{
    using RFID.REST.Database;
    using RFID.REST.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;


    public class RegisterOrUpdateUnKnownTagCommand
    {
        private readonly SqlConnectionFactory sqlConnectionFactory;
        private readonly Database database;

        public RegisterOrUpdateUnKnownTagCommand(SqlConnectionFactory sqlConnectionFactory, Database database)
        {
            this.sqlConnectionFactory = sqlConnectionFactory;
            this.database = database;
        }

        public async Task<CommandResult> RegisterOrUpdateAsync(String number)
        {
            using (var connection = await this.sqlConnectionFactory.CreateConnectionAsync(true))
            using (var transaction = connection.BeginTransaction())
            {
                var dbResult = await this.database.InsertOrUpdateUnknownTagAsync(number, transaction);

                transaction.Commit();

                return CommandResult.FromDbResult(dbResult);
            }
        }
    }
}

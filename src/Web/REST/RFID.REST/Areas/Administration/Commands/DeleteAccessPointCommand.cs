namespace RFID.REST.Areas.Administration.Commands
{
    using RFID.REST.Database;
    using RFID.REST.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class DeleteAccessPointCommand
    {
        private readonly Database database;
        private readonly SqlConnectionFactory sqlConnection;

        public DeleteAccessPointCommand(Database database, SqlConnectionFactory sqlConnection)
        {
            this.database = database;
            this.sqlConnection = sqlConnection;
        }

        public async Task<CommandResult> DeleteAsync(Identity identity)
        {
            using (var connection = await this.sqlConnection.CreateConnectionAsync(true))
            using (var transaction = connection.BeginTransaction())
            {
                var serialNumber = await this.database.GetAccessPointSerialNumberAsync(identity.Id, transaction);
                var dbResult = await this.database.DeleteAccessPointAsync(serialNumber, transaction);

                transaction.Commit();

                return CommandResult.FromDbResult(dbResult);
            }
        }
    }
}

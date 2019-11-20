namespace RFID.REST.Areas.Administration.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using RFID.REST.Areas.Administration.Models;
    using RFID.REST.Database;
    using RFID.REST.Models;

    /// <summary>
    /// Command for registering or updating access points
    /// </summary>
    public class RegisterAccessPointCommand
    {
        private readonly SqlConnectionFactory connectionFactory;
        private readonly Database database;

        public RegisterAccessPointCommand(SqlConnectionFactory connectionFactory, Database database)
        {
            this.connectionFactory = connectionFactory;
            this.database = database;
        }

        /// <summary>
        /// Returns true if the access point is registered or updated and false if the access point was not found during update
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<CommandResult> RegisterAsync(RegisterAccessPointRequestModel model)
        {
            using (var connection = await this.connectionFactory.CreateConnectionAsync(true))
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    var dbResult = await this.database.InsertAccessPointIfNotExistsAsync(
                        serialNumber: model.SerialNumber,
                        transaction: transaction,
                        description: model.Description,
                        IsActive: model.IsActive,
                        accessLevel: model.AccessLevel,
                        direction: model.Direction
                    );

                    if (dbResult.IsInserted)
                    {
                        await this.database.DeleteUnknownAccessPointAsync(model.SerialNumber, transaction);
                    }

                    transaction.Commit();

                    return CommandResult.FromDbResult(dbResult);
                }
                catch
                {
                    transaction.Rollback();

                    throw;
                }
            }
        }
    }
}

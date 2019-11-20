using RFID.REST.Areas.Administration.Models;
using RFID.REST.Database;
using RFID.REST.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RFID.REST.Areas.Administration.Commands
{
    public class UpdateAccessPointCommand
    {
        private readonly Database.Database database;
        private readonly SqlConnectionFactory sqlConnectionFactory;

        public UpdateAccessPointCommand(Database.Database database, SqlConnectionFactory sqlConnectionFactory)
        {
            this.database = database;
            this.sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task<CommandResult> UpdateAsync(UpdateAccessPointRequestModel requestModel)
        {
            using (var connection = await this.sqlConnectionFactory.CreateConnectionAsync(true))
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    var dbResult = await database.UpdateAccessPointAsync(
                        id: requestModel.Id,
                        transaction: transaction,
                        description: requestModel.Description,
                        isActive: requestModel.IsActive,
                        accessLevel: requestModel.AccessLevel,
                        direction: requestModel.Direction
                    );

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

namespace RFID.REST.Areas.Administration.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using RFID.REST.Areas.Administration.Models;
    using RFID.REST.Database;

    /// <summary>
    /// Command for registering or updating access points
    /// </summary>
    public class RegisterOrUpdateAccessPointCommand
    {
        private readonly SqlConnectionFactory connectionFactory;
        private readonly Database database;

        public RegisterOrUpdateAccessPointCommand(SqlConnectionFactory connectionFactory, Database database)
        {
            this.connectionFactory = connectionFactory;
            this.database = database;
        }

        /// <summary>
        /// Returns true if the access point is registered or updated and false if the access point was not found during update
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool> RegisterOrUpdateAsync(RegisterUpdateAccessPointRequestModel model)
        {
            using (var transaction = await this.connectionFactory.CreateTransactionAsync())
            {
                try
                {
                    var dbResult = await this.database.InsertOrUpdateAccessPointAsync(
                        identifier: model.Identifier, 
                        transaction: transaction, 
                        description: model.Description, 
                        IsActive: model.IsActive, 
                        accessLevel: model.AccessLevel
                    );

                    return dbResult.IsInserted;
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

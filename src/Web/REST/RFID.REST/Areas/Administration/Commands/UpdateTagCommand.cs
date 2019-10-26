namespace RFID.REST.Areas.Administration.Commands
{
    using RFID.REST.Database;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using RFID.REST.Areas.Administration.Models;
    using RFID.REST.Models;

    /// <summary>
    /// Command for updating tags
    /// </summary>
    public class UpdateTagCommand
    {
        private readonly SqlConnectionFactory sqlConnectionFactory;
        private readonly Database database;

        public UpdateTagCommand(SqlConnectionFactory sqlConnectionFactory, Database database)
        {
            this.sqlConnectionFactory = sqlConnectionFactory;
            this.database = database;
        }

        /// <summary>
        /// Updates the specified tag
        /// </summary>
        /// <returns>
        /// True if the tag was found and updated or false if the tag was not found
        /// </returns>
        public async Task<CommandResult> UpdateAsync(UpdateTagRequestModel model)
        {
            using (var connection = await this.sqlConnectionFactory.CreateConnectionAsync(true))
            using (var transaction = connection.BeginTransaction())
            {
                var dbResult = await this.database.UpdateTagAsync(
                    tagId: model.TagId,
                    userId: model.UserId,
                    isActive: model.IsActive,
                    isDeleted: model.IsDeleted,
                    accessLevel: model.AccessLevel,
                    transaction: transaction
                );

                transaction.Commit();

                return CommandResult.FromDbResult(dbResult);
            }
        }
    }
}

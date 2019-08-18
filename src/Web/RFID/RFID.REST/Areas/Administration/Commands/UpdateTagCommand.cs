namespace RFID.REST.Areas.Administration.Commands
{
    using RFID.REST.Database;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Database;
    using RFID.REST.Areas.Administration.Models;

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
        /// <returns></returns>
        public async Task UpdateAsync(UpdateTagRequestModel model)
        {
            using (var transaction = await this.sqlConnectionFactory.CreateTransactionAsync())
            {
                await this.database.UpdateTagAsync(tagId: model.TagId, userId: model.UserId, isActive: model.IsActive, isDeleted: model.IsDeleted, accessLevel: model.AccessLevel, transaction: transaction);
            }
        }
    }
}

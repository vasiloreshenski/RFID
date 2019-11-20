namespace RFID.REST.Areas.AccessControl.Commands
{
    using RFID.REST.Areas.AccessControl.Models;
    using RFID.REST.Database;
    using RFID.REST.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class CheckAccessCommand
    {
        private readonly SqlConnectionFactory sqlConnectionFactory;
        private readonly Database database;

        public CheckAccessCommand(SqlConnectionFactory sqlConnectionFactory, Database database)
        {
            this.sqlConnectionFactory = sqlConnectionFactory;
            this.database = database;
        }

        public async Task<CommandResult> CheckAccessAsync(CheckTagAccessRequestModel model)
        {
            using (var connection = await this.sqlConnectionFactory.CreateConnectionAsync(true))
            using (var transaction = connection.BeginTransaction())
            {
                var tagAccessLevel = await this.database.GetAccessLevelForTagAsync(model.TagNumber);
                var accessPointLevel = await this.database.GetAccessLevelForAccessPointAsync(model.AccessPointSerialNumber);

                if (accessPointLevel == null)
                {
                    await this.database.InsertOrUpdateUnknownAccessPointAsync(model.AccessPointSerialNumber, transaction);
                }

                if (tagAccessLevel == null)
                {
                    await this.database.InsertOrUpdateUnknownTagAsync(model.TagNumber, transaction);
                }

                await this.database.InsertEventAsync(model.AccessPointSerialNumber, model.TagNumber, transaction);

                transaction.Commit();

                if (accessPointLevel == null || tagAccessLevel == null)
                {
                    return CommandResult.NotFound();
                }
                else if (accessPointLevel > tagAccessLevel)
                {
                    return CommandResult.UnAuthorized();
                }
                else
                {
                    return CommandResult.Ok();
                }
            }
        }
    }
}

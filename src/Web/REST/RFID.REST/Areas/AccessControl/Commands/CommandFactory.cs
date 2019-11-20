namespace RFID.REST.Areas.AccessControl.Commands
{
    using RFID.REST.Database;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class CommandFactory
    {
        private readonly SqlConnectionFactory sqlConnectionFactory;
        private readonly Database database;

        public CommandFactory(SqlConnectionFactory sqlConnectionFactory, Database database)
        {
            this.sqlConnectionFactory = sqlConnectionFactory;
            this.database = database;
        }

        internal RegisterOrUpdateUnKnownAccessPointCommand CreateRegisterOrUpdateUnKnownAccessPointCommand()
        {
            return new RegisterOrUpdateUnKnownAccessPointCommand(this.database, this.sqlConnectionFactory);
        }

        internal RegisterOrUpdateUnKnownTagCommand CreateRegisterOrUpdateUnKnownTagCommand()
        {
            return new RegisterOrUpdateUnKnownTagCommand(this.sqlConnectionFactory, this.database);
        }

        internal CheckAccessCommand CreateCheckAccessCommand()
        {
            return new CheckAccessCommand(this.sqlConnectionFactory, this.database);
        }
    }
}

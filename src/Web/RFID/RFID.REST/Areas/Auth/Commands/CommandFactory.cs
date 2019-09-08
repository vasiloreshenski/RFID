namespace RFID.REST.Areas.Auth.Commands
{
    using RFID.REST.Areas.Auth.Services;
    using RFID.REST.Database;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class CommandFactory
    {
        private readonly SqlConnectionFactory sqlConnectionFactory;
        private readonly Database database;
        private readonly Auth auth;

        public CommandFactory(SqlConnectionFactory sqlConnectionFactory, Database database, Auth auth)
        {
            this.sqlConnectionFactory = sqlConnectionFactory;
            this.database = database;
            this.auth = auth;
        }

        public GenerateAuthTokenCommand CreateGenerateAuthTokenCommand()
        {
            return new GenerateAuthTokenCommand(this.auth, this.database, this.sqlConnectionFactory);
        }

        public RefreshAuthTokenCommand CreateRefereshAuthTokenCommand()
        {
            return new RefreshAuthTokenCommand(this.auth, this.database, this.sqlConnectionFactory);
        }
    }
}

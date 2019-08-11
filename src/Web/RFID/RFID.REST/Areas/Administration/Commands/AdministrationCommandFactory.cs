namespace RFID.REST.Areas.Administration.Commands
{
    using RFID.REST.Database;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Factory for creating administration related commands
    /// </summary>
    public class AdministrationCommandFactory
    {
        private readonly SqlConnectionFactory sqlConnectionFactory;
        private readonly Database database;

        public AdministrationCommandFactory(SqlConnectionFactory sqlConnectionFactory, Database database)
        {
            this.sqlConnectionFactory = sqlConnectionFactory;
            this.database = database;
        }

        /// <summary>
        /// Creates new instance of the <see cref="RegisterTagCommand"/>
        /// </summary>
        /// <returns></returns>
        public RegisterTagCommand CreateRegisterTagCommand()
        {
            return new RegisterTagCommand(this.sqlConnectionFactory, this.database);
        }
    }
}

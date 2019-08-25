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
    public class CommandFactory
    {
        private readonly SqlConnectionFactory sqlConnectionFactory;
        private readonly Database database;

        public CommandFactory(SqlConnectionFactory sqlConnectionFactory, Database database)
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

        /// <summary>
        /// Creates new instance of the <see cref="UpdateTagCommand"/>
        /// </summary>
        /// <returns></returns>
        public UpdateTagCommand CreateUpdateTagCommand()
        {
            return new UpdateTagCommand(this.sqlConnectionFactory, this.database);
        }

        /// <summary>
        /// Creates new instance of the <see cref="RegisterOrUpdateAccessPointCommand"/>
        /// </summary>
        /// <returns></returns>
        public RegisterOrUpdateAccessPointCommand CreateRegisterOrUpdateAccessPoint()
        {
            return new RegisterOrUpdateAccessPointCommand(this.sqlConnectionFactory, this.database);
        }
    }
}

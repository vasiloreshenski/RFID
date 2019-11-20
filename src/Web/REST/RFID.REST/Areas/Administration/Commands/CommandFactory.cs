namespace RFID.REST.Areas.Administration.Commands
{
    using Microsoft.AspNetCore.Identity;
    using RFID.REST.Areas.Administration.Models;
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
        private readonly IPasswordHasher<AdministrationUser> passwordHasher;

        public CommandFactory(SqlConnectionFactory sqlConnectionFactory, Database database, IPasswordHasher<AdministrationUser> passwordHasher)
        {
            this.sqlConnectionFactory = sqlConnectionFactory;
            this.database = database;
            this.passwordHasher = passwordHasher;
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
        /// Creates new instance of the <see cref="RegisterAccessPointCommand"/>
        /// </summary>
        /// <returns></returns>
        public RegisterAccessPointCommand CreateRegisterAccessPointCommand()
        {
            return new RegisterAccessPointCommand(this.sqlConnectionFactory, this.database);
        }

        public UpdateAccessPointCommand CreateUpdateAccessPointCommand()
        {
            return new UpdateAccessPointCommand(this.database, this.sqlConnectionFactory);
        }
        
        public RegisterAdministrationUserCommand CreateRegisterAdministrationUserCommand()
        {
            return new RegisterAdministrationUserCommand(this.sqlConnectionFactory, this.database, this.passwordHasher);
        }

        public DeleteAccessPointCommand CreateDeleteAccessPointCommand()
        {
            return new DeleteAccessPointCommand(this.database, this.sqlConnectionFactory);
        }
    }
}

namespace RFID.REST.Areas.User.Commands
{
    using Microsoft.AspNetCore.Identity;
    using RFID.REST.Database;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class CommandFactory
    {
        private readonly SqlConnectionFactory sqlConnectionFactory;
        private readonly Database database;
        private readonly IPasswordHasher<User.Models.User> passwordHasher;

        public CommandFactory(SqlConnectionFactory sqlConnectionFactory, Database database, IPasswordHasher<User.Models.User> passwordHasher)
        {
            this.sqlConnectionFactory = sqlConnectionFactory;
            this.database = database;
            this.passwordHasher = passwordHasher;
        }

        public RegisterUserCommand CreateRegisterUserCommand()
        {
            return new RegisterUserCommand(this.sqlConnectionFactory, this.database, this.passwordHasher);
        }
    }
}

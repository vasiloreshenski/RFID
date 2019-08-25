namespace RFID.REST.Areas.User.Commands
{
    using Microsoft.AspNetCore.Identity;
    using RFID.REST.Areas.User.Models;
    using RFID.REST.Database;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Registers a new administration user
    /// </summary>
    public class RegisterUserCommand
    {
        private readonly SqlConnectionFactory sqlConnectionFactory;
        private readonly Database database;
        private readonly IPasswordHasher<User> passwordHasher;

        public RegisterUserCommand(SqlConnectionFactory sqlConnectionFactory, Database database, IPasswordHasher<User> passwordHasher)
        {
            this.sqlConnectionFactory = sqlConnectionFactory;
            this.database = database;
            this.passwordHasher = passwordHasher;
        }

        /// <summary>
        /// Executes the command
        /// </summary>
        /// <param name="model">user model to be registered</param>
        /// <returns></returns>
        public async Task RegisterAsync(RegisterUserRequestModel model)
        {
            using (var transaction = await this.sqlConnectionFactory.CreateTransactionAsync())
            {
                try
                {
                    var user = new User(model.Email, model.Roles.ToArray());
                    var passwordHash = this.passwordHasher.HashPassword(user, model.Password);

                    await this.database.InsertAdministrationUserAsync(email: model.Email, passwordHash: passwordHash, roles: model.Roles.ToArray(), transaction: transaction);

                    transaction.Commit();
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

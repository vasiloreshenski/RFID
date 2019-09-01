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
    /// Registers a new administration user
    /// </summary>
    public class RegisterAdministrationUserCommand
    {
        private readonly SqlConnectionFactory sqlConnectionFactory;
        private readonly Database database;
        private readonly IPasswordHasher<AdministrationUser> passwordHasher;

        public RegisterAdministrationUserCommand(SqlConnectionFactory sqlConnectionFactory, Database database, IPasswordHasher<AdministrationUser> passwordHasher)
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
        public async Task RegisterAsync(RegisterAdministrationUserRequestModel model)
        {
            using (var transaction = await this.sqlConnectionFactory.CreateTransactionAsync())
            {
                try
                {
                    var passwordHash = this.passwordHasher.HashPassword(null, model.Password);
                    
                    await this.database.InsertAdministrationUserAsync(email: model.Email, passwordHash: passwordHash, roles: model.Roles, transaction: transaction);

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

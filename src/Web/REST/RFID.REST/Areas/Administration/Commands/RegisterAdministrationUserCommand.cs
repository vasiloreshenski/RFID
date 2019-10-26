namespace RFID.REST.Areas.Administration.Commands
{
    using Microsoft.AspNetCore.Identity;
    using RFID.REST.Areas.Administration.Models;
    using RFID.REST.Database;
    using RFID.REST.Models;
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
        public async Task<CommandResult> RegisterAsync(RegisterAdministrationUserRequestModel model)
        {
            using (var connection = await this.sqlConnectionFactory.CreateConnectionAsync(true))
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    var passwordHash = this.passwordHasher.HashPassword(null, model.Password);

                    var dbResult = await this.database.InsertAdministrationUserAsync(email: model.Email, passwordHash: passwordHash, roles: model.Roles, transaction: transaction);

                    var refreshToken = Auth.Services.Auth.GenerateRefereshToken();
                    await this.database.ReplaceRefreshTokenAsync(model.Email, refreshToken, transaction);

                    transaction.Commit();

                    return CommandResult.FromDbResult(dbResult);
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

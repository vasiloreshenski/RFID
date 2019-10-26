namespace RFID.REST.Areas.Auth.Commands
{
    using RFID.REST.Areas.Auth.Models;
    using RFID.REST.Areas.Auth.Services;
    using RFID.REST.Database;
    using RFID.REST.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Generates new auth token
    /// </summary>
    public class GenerateAuthTokenCommand
    {
        private readonly Auth auth;
        private readonly Database database;
        private readonly SqlConnectionFactory sqlConnectionFactory;

        public GenerateAuthTokenCommand(Auth auth, Database database, SqlConnectionFactory sqlConnectionFactory)
        {
            this.auth = auth;
            this.database = database;
            this.sqlConnectionFactory = sqlConnectionFactory;
        }

        /// <summary>
        /// Executes the command for the specified user info
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<CommandResult<AuthToken>> GenerateTokenAsync(TokenGenerationRequestModel model)
        {
            var dbResult = InsertOrUpdDbResult.NotFound;

            var authToken = await this.auth.ValidatePasswordAndGenerateTokenAsync(model.Email, model.Password);
            if (authToken != null)
            {
                using (var connection = await this.sqlConnectionFactory.CreateConnectionAsync(true))
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        dbResult = await this.database.ReplaceRefreshTokenAsync(model.Email, authToken.RefreshToken, transaction);

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();

                        throw;
                    }
                }
            }

            if (dbResult.IsNotFound)
            {
                return CommandResult.NotFound<AuthToken>();
            }
            else
            {
                return new CommandResult<AuthToken>(authToken, CommandStatus.Created);
            }
        }
    }
}

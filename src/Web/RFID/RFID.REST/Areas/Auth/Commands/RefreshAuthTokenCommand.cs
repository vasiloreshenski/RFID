namespace RFID.REST.Areas.Auth.Commands
{
    using RFID.REST.Areas.Auth.Models;
    using RFID.REST.Areas.Auth.Services;
    using RFID.REST.Common;
    using RFID.REST.Database;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Generates new auth token by the specified referesh token
    /// </summary>
    public class RefreshAuthTokenCommand
    {
        private readonly Auth auth;
        private readonly Database database;
        private readonly SqlConnectionFactory sqlConnectionFactory;

        public RefreshAuthTokenCommand(Auth auth, Database database, SqlConnectionFactory sqlConnectionFactory)
        {
            this.auth = auth;
            this.database = database;
            this.sqlConnectionFactory = sqlConnectionFactory;
        }

        /// <summary>
        /// Executes the commnad
        /// </summary>
        /// <param name="model">Auth and referesh tokens</param>
        /// <returns></returns>
        public async Task<AuthToken> RefreshTokenAsync(TokenRefreshRequestModel model)
        {
            var claimsPrincipal = this.auth.GetClamsPrincipalFromExpiredToken(model.Token);
            var email = claimsPrincipal?.Email();
            var authUser = await this.database.GeAuthUserAsync(email);

            var isValidRefreshToken = authUser?.RefreshToken == model.RefreshToken;
            if (isValidRefreshToken)
            {
                var authToken = this.auth.GenerateToken(authUser.Email, authUser.Roles);
                var dbResult = InsertOrUpdDbResult.NotFound;
                using (var transaction = await this.sqlConnectionFactory.CreateTransactionAsync())
                {
                    try
                    {
                        dbResult = await this.database.ReplaceRefreshTokenAsync(authUser.Email, authUser.RefreshToken, transaction);

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();

                        throw;
                    }
                }

                if (dbResult.IsNotFound)
                {
                    return null;
                }

                return authToken;
            }

            return null;
        }
    }
}

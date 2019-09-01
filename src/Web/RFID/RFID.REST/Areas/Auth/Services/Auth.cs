namespace RFID.REST.Areas.Auth.Services
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.IdentityModel.Tokens;
    using RFID.REST.Areas.Administration.Models;
    using RFID.REST.Areas.Auth.Models;
    using RFID.REST.Database;
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Service for handling administration user authentication
    /// </summary>
    public class Auth
    {
        private readonly IPasswordHasher<AdministrationUser> passwordHasher;
        private readonly Database database;
        private readonly AuthSettings authSettings;

        public Auth(IPasswordHasher<AdministrationUser> passwordHasher, Database database, AuthSettings authSettings)
        {
            this.passwordHasher = passwordHasher;
            this.database = database;
            this.authSettings = authSettings;
        }

        /// <summary>
        /// Generates access token for the specified user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<AuthToken> IssueTokenForUserAsync(TokenGenerationRequestModel model)
        {
            var administrationUser = await this.database.GetAdministrationUserAsync(model.Email);
            if (administrationUser != null)
            {
                var verificationResult = passwordHasher.VerifyHashedPassword(administrationUser, administrationUser.PasswordHash, model.Password);
                if (verificationResult == PasswordVerificationResult.Success)
                {
                    var securityToken = GenerateToken(administrationUser);
                    return new AuthToken(securityToken);
                }
            }

            return null;
        }

        private SecurityToken GenerateToken(AdministrationUser user)
        {
            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(this.authSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Email.ToString()),
                    new Claim(ClaimTypes.Role, user.Roles.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return token;
        }
    }
}

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
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Service for handling administration user authentication
    /// </summary>
    public class Auth
    {
        public static readonly String SecurityAlg = SecurityAlgorithms.HmacSha256Signature;

        private readonly IPasswordHasher<AuthUser> passwordHasher;
        private readonly Database database;
        private readonly AuthSettings authSettings;

        public Auth(IPasswordHasher<AuthUser> passwordHasher, Database database, AuthSettings authSettings)
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
        public async Task<AuthToken> ValidatePasswordAndGenerateTokenAsync(String email, String password)
        {
            var authUser = await this.database.GeAuthUserAsync(email);
            if (authUser != null)
            {
                var verificationResult = passwordHasher.VerifyHashedPassword(authUser, authUser.PasswordHash, password);
                if (verificationResult == PasswordVerificationResult.Success)
                {
                    var authToken = this.GenerateToken(authUser.Email, authUser.Roles);
                    return authToken;
                }
            }

            return null;
        }

        /// <summary>
        /// Generates token with the email and role claims
        /// </summary>
        /// <param name="email">Email</param>
        /// <param name="roles">Role</param>
        /// <returns></returns>
        public AuthToken GenerateToken(String email, UserRoles roles)
        {
            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = null,
                Audience = null,
                IssuedAt = DateTime.UtcNow,
                Expires = DateTime.UtcNow.Add(AuthToken.DefaultExpire),
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, email),
                    new Claim(ClaimTypes.Role, roles.ToString())
                }),
                SigningCredentials = new SigningCredentials(this.authSettings.SecurityKey, SecurityAlg)
            };
            var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            var tokenStr = tokenHandler.WriteToken(token);

            var refreshToken = GenerateRefereshToken();

            return new AuthToken(tokenStr, refreshToken);
        }

        /// <summary>
        /// Returns claims info the specified expired token.
        /// </summary>
        /// <returns></returns>
        public ClaimsPrincipal GetClamsPrincipalFromExpiredToken(String value)
        {
            var validationParameters = this.authSettings.CreateTokenValidationParameters();

            // we don't need to validate the lifetime of an expired token
            validationParameters.ValidateLifetime = false;

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(value, validationParameters , out var securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;

            // if we don't validate the hash alg there is posability to be send token who is not using hash alg which will be automatically valid
            if (jwtSecurityToken == null || jwtSecurityToken.Header.Alg.Equals(SecurityAlg, StringComparison.InvariantCultureIgnoreCase) == false)
            {
                return null;
            }

            return principal;
        }

        /// <summary>
        /// Generates refresh token
        /// </summary>
        /// <returns></returns>
        public static String GenerateRefereshToken()
        {
            var size = 32;

            var buffer = new Byte[size];
            using (var rg = RandomNumberGenerator.Create())
            {
                rg.GetBytes(buffer, offset: 0, count: size);
            }

            var str = Convert.ToBase64String(buffer);

            return str;
        }
    }
}

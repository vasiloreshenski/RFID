﻿namespace RFID.REST.Areas.Auth.Models
{
    using Microsoft.IdentityModel.Tokens;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Auth settings
    /// </summary>
    public class AuthSettings
    {
        public AuthSettings(string secret, string issuer, string audience, TimeSpan accessExpiration, TimeSpan refreshExpiration)
        {
            this.Secret = secret;
            this.Issuer = issuer;
            this.Audience = audience;
            this.AccessExpiration = accessExpiration;
            this.RefreshExpiration = refreshExpiration;
        }

        /// <summary>
        /// Secret
        /// </summary>
        public String Secret { get;  }

        /// <summary>
        /// Issuer
        /// </summary>
        public String Issuer { get; }

        /// <summary>
        /// Audience
        /// </summary>
        public String Audience { get; }

        /// <summary>
        /// Access expiration
        /// </summary>
        public TimeSpan AccessExpiration { get; }

        /// <summary>
        /// Reference expiration
        /// </summary>
        public TimeSpan RefreshExpiration { get; }
        
        /// <summary>
        /// ASCII bytes representation of the secret
        /// </summary>
        public Byte[] SecretBytes => Encoding.ASCII.GetBytes(this.Secret);

        /// <summary>
        /// Security key
        /// </summary>
        public SecurityKey SecurityKey => new SymmetricSecurityKey(this.SecretBytes);

        /// <summary>
        /// Create token validation paramters
        /// </summary>
        /// <returns></returns>
        public TokenValidationParameters CreateTokenValidationParameters()
        {
            return new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = this.SecurityKey,
                ValidateIssuer = false,
                ValidateAudience = false
            };
        }
    }
}

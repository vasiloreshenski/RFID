namespace RFID.REST.Areas.Auth.Models
{
    using Microsoft.IdentityModel.Tokens;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    
    /// <summary>
    /// Authentication token info
    /// </summary>
    public class AuthToken
    {
        public static readonly TimeSpan DefaultExpire = TimeSpan.FromMinutes(3);

        public AuthToken(String token, String refereshToken)
            : this(token, (int)DefaultExpire.TotalSeconds, refereshToken)
        {

        }

        public AuthToken(String token, int expireInSeconds, String refereshToken)
        {
            this.Token = token;
            this.ExpireInSeconds = expireInSeconds;
            this.RefereshToken = refereshToken;
        }

        /// <summary>
        /// Value of generated token
        /// </summary>
        public String Token { get; }

        /// <summary>
        /// Token expiration in secods
        /// </summary>
        public int ExpireInSeconds { get; }

        /// <summary>
        /// Value for the referesh token
        /// </summary>
        public String RefereshToken { get; }
    }
}

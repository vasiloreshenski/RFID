namespace RFID.REST.Areas.Auth.Models
{
    using RFID.REST.Areas.Administration.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Class representing auth user
    /// </summary>
    public class AuthUser
    {
        public AuthUser(string email, string refreshToken, string passwordHash, UserRoles roles)
        {
            this.Email = email;
            this.RefreshToken = refreshToken;
            this.PasswordHash = passwordHash;
            this.Roles = roles;
        }

        /// <summary>
        /// Email
        /// </summary>
        public String Email { get; }

        /// <summary>
        /// Refresh token
        /// </summary>
        public String RefreshToken { get; }

        /// <summary>
        /// Password hash
        /// </summary>
        public String PasswordHash { get; }

        /// <summary>
        /// Roles
        /// </summary>
        public UserRoles Roles { get; }
    }
}

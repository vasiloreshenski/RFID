namespace RFID.REST.Areas.Administration.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Domain model for the user
    /// </summary>
    public class AuthUser
    {
        public AuthUser(String email, IReadOnlyCollection<AuthUserRole> roles)
        {
            this.Email = email;
            this.Roles = roles;
        }

        /// <summary>
        /// Email
        /// </summary>
        public String Email { get; private set; }
        
        /// <summary>
        /// Roles
        /// </summary>
        public IReadOnlyCollection<AuthUserRole> Roles { get; }
    }
}

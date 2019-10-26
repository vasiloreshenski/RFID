namespace RFID.REST.Areas.Administration.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Domain model for the user
    /// </summary>
    public class AdministrationUser
    {
        public AdministrationUser(String email, String passwordHash, UserRoles roles)
        {
            this.Email = email;
            this.Roles = roles;
            this.PasswordHash = passwordHash;
        }

        /// <summary>
        /// Email
        /// </summary>
        public String Email { get; }
        
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

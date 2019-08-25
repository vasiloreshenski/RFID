namespace RFID.REST.Areas.User.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Domain model for the user
    /// </summary>
    public class User
    {
        public User(string email, IReadOnlyCollection<UserRole> roles)
        {
            Email = email;
            Roles = roles;
        }

        /// <summary>
        /// Email
        /// </summary>
        public String Email { get; private set; }

        /// <summary>
        /// Roles
        /// </summary>
        public IReadOnlyCollection<UserRole> Roles { get; }
    }
}

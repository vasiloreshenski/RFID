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
        public AdministrationUser(String email, IReadOnlyCollection<AdministrationUserRole> roles)
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
        public IReadOnlyCollection<AdministrationUserRole> Roles { get; }
    }
}

namespace RFID.REST.Areas.Auth.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Model representing request info for token
    /// </summary>
    public class TokenGenerationRequestModel
    {
        /// <summary>
        /// Email
        /// </summary>
        [Required]
        public String Email { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        [Required]
        public String Password { get; set; }
    }
}

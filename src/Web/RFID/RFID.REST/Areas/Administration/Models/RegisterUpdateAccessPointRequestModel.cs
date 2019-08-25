namespace RFID.REST.Areas.Administration.Models
{
    using RFID.REST.Models.Common;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Model representing access point to be registered
    /// </summary>
    public class RegisterUpdateAccessPointRequestModel
    {
        /// <summary>
        /// Access's point identifier
        /// </summary>
        [Required]
        public Guid Identifier { get; set; }
        
        /// <summary>
        /// Description of the access point
        /// </summary>
        public String Description { get; set; }

        /// <summary>
        /// Is active state of the access point
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// Access level for the access point
        /// </summary>
        public AccessLevel? AccessLevel { get; set; }
    }
}

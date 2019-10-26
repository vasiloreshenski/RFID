namespace RFID.REST.Areas.Administration.Models
{
    using RFID.REST.Models;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Model representing access point to be registered
    /// </summary>
    public class RegisterAccessPointRequestModel
    {
        /// <summary>
        /// Access's point identifier
        /// </summary>
        [Required]
        public String SerialNumber { get; set; }
        
        /// <summary>
        /// Description of the access point
        /// </summary>
        [Required]
        public String Description { get; set; }

        /// <summary>
        /// Is active state of the access point
        /// </summary>
        [Required]
        public bool IsActive { get; set; }

        /// <summary>
        /// Access level for the access point
        /// </summary>
        [Required]
        [EnumDataType(typeof(AccessLevel))]
        public AccessLevel AccessLevel { get; set; }

        [Required]
        [EnumDataType(typeof(AccessPointDirectionType))]
        public AccessPointDirectionType Direction { get; set; }
    }
}

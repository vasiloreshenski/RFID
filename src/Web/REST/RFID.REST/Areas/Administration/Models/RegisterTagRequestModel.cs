namespace RFID.REST.Areas.Administration.Models
{
    using RFID.REST.Models;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Request model used for registering new tags
    /// </summary>
    public class RegisterTagRequestModel
    {
        /// <summary>
        /// Unique tag number
        /// </summary>
        [Required]
        public String Number { get; set; }

        /// <summary>
        /// Level of access for the current tag
        /// </summary>
        [Required]
        [EnumDataType(typeof(RFID.REST.Models.AccessLevel))]
        public AccessLevel AccessLevel { get; set; }

        /// <summary>
        /// User associated with the current tag
        /// </summary>
        [Required]
        public RegisterTagUserRequestModel User { get; set; }
    }
}

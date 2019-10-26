namespace RFID.REST.Areas.AccessControl.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Reauest model for checking the access of a tag for specific access point
    /// </summary>
    public class CheckTagAccessRequestModel
    {
        /// <summary>
        /// Tag of the id to be checked
        /// </summary>
        [Required]
        public String TagNumber { get; set; }

        /// <summary>
        /// Identifier of the access point against which the tag is checked
        /// </summary>
        [Required]
        public String AccessPointSerialNumber { get; set; }
    }
}

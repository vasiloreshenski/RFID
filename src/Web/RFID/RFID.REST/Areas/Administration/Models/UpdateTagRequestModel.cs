namespace RFID.REST.Areas.Administration.Models
{
    using RFID.REST.Models.Common;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Request model for updating tags
    /// </summary>
    public class UpdateTagRequestModel
    {
        /// <summary>
        /// Id of the tag to be updated
        /// </summary>
        [Required]
        public int TagId { get; set; }

        /// <summary>
        /// Is active status for the specified tag
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// Is deleted status for the specified tag
        /// </summary>
        public bool? IsDeleted { get; set; }

        /// <summary>
        /// User id for the specified tag
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// Access level for the specified tag
        /// </summary>
        public TagAccessLevel AccessLevel { get; set; }
    }
}

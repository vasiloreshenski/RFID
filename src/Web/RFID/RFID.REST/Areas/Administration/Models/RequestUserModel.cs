namespace RFID.REST.Areas.Administration.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading.Tasks;
    
    /// <summary>
    /// Represents user associated to specific tag
    /// </summary>
    public class RequestUserModel
    {
        /// <summary>
        /// User id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// User's name
        /// </summary>
        [Required]
        public String Name { get; set; }
    }
}

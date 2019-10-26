namespace RFID.REST.Areas.Administration.Models
{
    using RFID.REST.Models;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading.Tasks;
    
    public class ChangeTagAccessLevelRequestModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [EnumDataType(typeof(AccessLevel))]
        public AccessLevel AccessLevel { get; set; }
    }
}

namespace RFID.REST.Areas.Administration.Models
{
    using RFID.REST.Models;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading.Tasks;
    
    public class UpdateAccessPointRequestModel
    {
        [Required]
        public int AccessPointId { get; set; }

        public String Description { get; set; }

        public bool? IsActive { get; set; }
        
        public AccessLevel? AccessLevel { get; set; }
    }
}

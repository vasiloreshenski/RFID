namespace RFID.REST.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading.Tasks;


    public class Identity
    {
        [Required]
        public int Id { get; set; }
    }
}

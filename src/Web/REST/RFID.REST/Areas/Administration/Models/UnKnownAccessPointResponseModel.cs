namespace RFID.REST.Areas.Administration.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class UnKnownAccessPointResponseModel
    {
        public int Id { get; set; }
        public String SerialNumber { get; set; }
        public DateTime AccessDate { get; set; }
    }
}

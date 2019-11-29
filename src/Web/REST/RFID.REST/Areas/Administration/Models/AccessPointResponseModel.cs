namespace RFID.REST.Areas.Administration.Models
{
    using RFID.REST.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class AccessPointResponseModel
    {
        public int Id { get; set; }

        public String SerialNumber { get; set; }

        public String Description { get; set; }

        public AccessLevel AccessLevel { get; set; }

        public AccessPointDirectionType Direction { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? ModificationDate { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }
    }
}

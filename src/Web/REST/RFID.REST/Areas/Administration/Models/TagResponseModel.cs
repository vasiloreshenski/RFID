namespace RFID.REST.Areas.Administration.Models
{
    using RFID.REST.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class TagResponseModel
    {
        public int Id { get; set; }

        public bool IsActive { get; set; }

        public AccessLevel AccessLevel { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? ModificationDate { get; set; }

        public String Number { get; set; }

        public String UserName { get; set; }
    }
}

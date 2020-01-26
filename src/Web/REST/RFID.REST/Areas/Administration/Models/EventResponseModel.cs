namespace RFID.REST.Areas.Administration.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class EventResponseModel
    {
        public int Id { get; set; }
        public String TagNumber { get; set; }
        public int TagLevelId { get; set; }
        public bool TagIsActive { get; set; }
        public bool TagIsDeleted { get; set; }
        public bool TagIsUnknown { get; set; }
        public int UserId { get; set; }
        public String AccessPointSerialNumber { get; set; }
        public int AccessPointLevelId { get; set; }
        public int AccessPointDirectionId { get; set; }
        public bool AccessPointIsActive { get; set; }
        public bool AccessPointIsDeleted { get; set; }
        public bool AccessPointIsUnknown { get; set; }
        public DateTime CreateDate { get; set; }
    }
}

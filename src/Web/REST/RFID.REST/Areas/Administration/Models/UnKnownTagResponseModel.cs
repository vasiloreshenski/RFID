using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RFID.REST.Areas.Administration.Models
{
    public class UnKnownTagResponseModel
    {
        public int Id { get; set; }

        public DateTime AccessDate { get; set; }

        public String Number { get; set; }
    }
}

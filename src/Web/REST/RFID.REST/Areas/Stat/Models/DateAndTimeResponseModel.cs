using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RFID.REST.Areas.Stat.Models
{
    public class DateAndTimeResponseModel
    {
        public DateTime Day { get; set; }
        public TimeSpan Time { get; set; }
    }
}

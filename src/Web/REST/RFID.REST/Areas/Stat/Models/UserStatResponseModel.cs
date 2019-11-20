namespace RFID.REST.Areas.Stat.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class UserStatResponseModel
    {
        public int Id { get; set; }

        public String UserName { get; set; }

        public TimeSpan AvgEntranceTime { get; set; }

        public TimeSpan AvgExitTime { get; set; }

        public TimeSpan AvgWorkHourNorm { get; set; }
    }
}

namespace RFID.REST.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public enum CommandStatus
    {
        None = 0,
        Created = 1,
        Updated = 2,
        Deleted = 3,

        Dublicate = 100,
        NotFound = 101,
        Expired = 102
    }
}

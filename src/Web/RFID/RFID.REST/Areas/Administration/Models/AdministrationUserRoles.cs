namespace RFID.REST.Areas.Administration.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Administration user roles
    /// </summary>
    [Flags]
    public enum AdministrationUserRoles
    {
        Admin = 1
    }
}

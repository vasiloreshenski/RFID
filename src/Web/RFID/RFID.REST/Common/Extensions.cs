namespace RFID.REST.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    
    public static class Extensions
    {
        /// <summary>
        /// Returns Email claim or null
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        public static String Email(this ClaimsPrincipal cp)
        {
            return cp.FindFirstValue(ClaimTypes.Name);
        }
    }
}

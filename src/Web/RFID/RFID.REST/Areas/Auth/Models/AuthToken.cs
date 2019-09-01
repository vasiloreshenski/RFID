namespace RFID.REST.Areas.Auth.Models
{
    using Microsoft.IdentityModel.Tokens;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;


    /// <summary>
    /// Authentication token info
    /// </summary>
    public class AuthToken
    {
        public AuthToken(String value)
        {
            Value = value;
        }

        /// <summary>
        /// Value of generated token
        /// </summary>
        public String Value { get; }
    }
}

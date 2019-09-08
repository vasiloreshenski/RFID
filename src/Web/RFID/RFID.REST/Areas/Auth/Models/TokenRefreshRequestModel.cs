namespace RFID.REST.Areas.Auth.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Request model for auth token referesh
    /// </summary>
    public class TokenRefreshRequestModel
    {
        public TokenRefreshRequestModel(string token, string refreshToken)
        {
            this.Token = token;
            this.RefreshToken = refreshToken;
        }

        /// <summary>
        /// Auth token
        /// </summary>
        [Required]
        public String Token { get; set; }
        
        /// <summary>
        /// Refresh token
        /// </summary>
        [Required]
        public String RefreshToken { get; set; }
    }
}

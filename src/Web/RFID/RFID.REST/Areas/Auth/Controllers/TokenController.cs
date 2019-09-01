namespace RFID.REST.Areas.Auth.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using RFID.REST.Areas.Auth.Models;
    using RFID.REST.Areas.Auth.Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Controller for generating, refereshing auth tokens
    /// </summary>
    [Route("[area]/api/token")]
    [ApiController]
    [Area("Auth")]
    [Authorize]
    public class TokenController : ControllerBase
    {
        private readonly Auth auth;

        public TokenController(Auth auth)
        {
            this.auth = auth;
        }

        /// <summary>
        /// Generates token for the specified user
        /// </summary>
        /// <returns>
        /// 200 if the user is registered
        /// 404 if the user is not registered
        /// </returns>
        [HttpGet("generate")]
        [AllowAnonymous]
        public async Task<IActionResult> GenerateTokenAsync([FromQuery]TokenGenerationRequestModel model)
        {
            var token = await this.auth.IssueTokenForUserAsync(model);
            if (token != null)
            {
                return this.Ok(token);
            }
            else
            {
                return this.NotFound();
            }
        }

        [HttpGet("refresh")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RefereshAsync()
        {
            return this.Ok();
        }
    }
}

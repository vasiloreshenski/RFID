namespace RFID.REST.Areas.Auth.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using RFID.REST.Areas.Auth.Models;
    using RFID.REST.Areas.Auth.Services;
    using RFID.REST.Common;
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
        private readonly Commands.CommandFactory commandFactory;

        public TokenController(Commands.CommandFactory commandFactory)
        {
            this.commandFactory = commandFactory;
        }

        /// <summary>
        /// Generates token for the specified user
        /// </summary>
        /// <returns>
        /// 200 if the user is registered
        /// 404 if the user is not registered
        /// </returns>
        [HttpPost("generate")]
        [AllowAnonymous]
        public async Task<IActionResult> GenerateTokenAsync(TokenGenerationRequestModel model)
        {
            var command = this.commandFactory.CreateGenerateAuthTokenCommand();
            var token = await command.GenerateTokenAsync(model);
            if (token != null)
            {
                return this.Ok(token);
            }
            else
            {
                return this.NotFound();
            }
        }

        /// <summary>
        /// Generates new auth token and refresh token from the specified tokens
        /// </summary>
        /// <returns>
        /// 200 if the referesh token and the auth token are valid
        /// 400 if there is error with the tokens validation
        /// </returns>
        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> RefereshAsync(TokenRefreshRequestModel model)
        {
            var command = this.commandFactory.CreateRefereshAuthTokenCommand();
            var authToken = await command.RefreshTokenAsync(model);
            if (authToken != null)
            {
                return this.Ok(authToken);
            }
            else
            {
                return this.NotFound();
            }
        }
    }
}

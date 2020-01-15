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
        public async Task<IActionResult> GenerateTokenAsync([FromBody]TokenGenerationRequestModel model)
        {
            var command = this.commandFactory.CreateGenerateAuthTokenCommand();
            var commandResult = await command.GenerateTokenAsync(model);
            if (commandResult.Success)
            {
                return this.Ok(commandResult.Value);
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
            var commandResult = await command.RefreshTokenAsync(model);
            if (commandResult.Success)
            {
                return this.Ok(commandResult.Value);
            }
            else
            {
                return this.NotFound();
            }
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            return this.Ok();
        }
    }
}

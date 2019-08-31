namespace RFID.REST.Areas.Administration.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using RFID.REST.Areas.Administration.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    [ApiController]
    [Route("/api/auth_users")]
    [Area("Administration")]
    [Authorize]
    public class AuthUsersController : ControllerBase
    {
        private readonly Areas.Administration.Commands.CommandFactory commandFactory;

        public AuthUsersController(Areas.Administration.Commands.CommandFactory commandFactory)
        {
            this.commandFactory = commandFactory;
        }

        [HttpPost("/register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterAsync(RegisterAuthUserRequestModel model)
        {
            var command = this.commandFactory.CreateRegisterUserCommand();
            await command.RegisterAsync(model);

            return this.Ok();
        }
    }
}

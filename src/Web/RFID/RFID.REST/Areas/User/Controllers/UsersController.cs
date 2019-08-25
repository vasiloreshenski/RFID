namespace RFID.REST.Areas.Users.Controllers
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using RFID.REST.Areas.User.Commands;
    using RFID.REST.Areas.User.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    [ApiController]
    [Route("/api/users")]
    [Area("Users")]
    public class UsersController : ControllerBase
    {
        private readonly Areas.User.Commands.CommandFactory commandFactory;

        public UsersController(CommandFactory commandFactory)
        {
            this.commandFactory = commandFactory;
        }

        [HttpPost("/register")]
        public async Task<IActionResult> RegisterAsync(RegisterUserRequestModel model)
        {
            var command = this.commandFactory.CreateRegisterUserCommand();
            await command.RegisterAsync(model);

            return this.Ok();
        }
    }
}

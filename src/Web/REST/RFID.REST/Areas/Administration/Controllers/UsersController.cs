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
    [Route("[area]/api/users")]
    [Area("Administration")]
    public class UsersController : ControllerBase
    {
        private readonly Areas.Administration.Commands.CommandFactory commandFactory;

        public UsersController(Areas.Administration.Commands.CommandFactory commandFactory)
        {
            this.commandFactory = commandFactory;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterAsync([FromBody]RegisterAdministrationUserRequestModel model)
        {
            var command = this.commandFactory.CreateRegisterAdministrationUserCommand();
            var result = await command.RegisterAsync(model);

            if (result.Success)
            {
                return this.Ok();
            }
            else
            {
                return this.BadRequest(result);
            }
        }
    }
}

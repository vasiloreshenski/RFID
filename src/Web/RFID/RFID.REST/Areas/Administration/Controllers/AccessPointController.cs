namespace RFID.REST.Areas.Administration.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using RFID.REST.Areas.Administration.Commands;
    using RFID.REST.Areas.Administration.Models;
    using RFID.REST.Models.Common;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Controller for registering, activating and de-activating access points
    /// </summary>
    [ApiController]
    [Route("/api/accessPoint")]
    [Area("administration")]
    public class AccessPointController : ControllerBase
    {
        private readonly CommandFactory commandFactory;

        public AccessPointController(CommandFactory commandFactory)
        {
            this.commandFactory = commandFactory;
        }

        /// <summary>
        /// Register the specified access point
        /// </summary>
        /// <param name="model">access point model</param>
        /// <returns>
        /// 200 if the tag was registered successfully
        /// 400 if the model is not valid or the access point is already registered
        /// </returns>
        [HttpPost("/register")]
        public async Task<IActionResult> RegisterAsync(RegisterUpdateAccessPointRequestModel model)
        {
            var command = this.commandFactory.CreateRegisterOrUpdateAccessPoint();
            await command.RegisterOrUpdateAsync(model);

            return this.Ok();
        }
        
        /// <summary>
        /// Activates the specified access point
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns>
        /// 200 if the access point is activated
        /// 404 if there is no such access point
        /// </returns>
        [HttpPatch("/activate")]
        public async Task<IActionResult> ActivateAsync(Guid identifier)
        {
            var command = this.commandFactory.CreateRegisterOrUpdateAccessPoint();
            var found = await command.RegisterOrUpdateAsync(new RegisterUpdateAccessPointRequestModel { Identifier = identifier, IsActive = true });
            if (found)
            {
                return this.Ok();
            }
            else
            {
                return this.NotFound();
            }
        }

        /// <summary>
        /// De-activates the specified access point.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns>
        /// 200 if the access point is deactivated
        /// 404 if there is no such access point
        /// </returns>
        [HttpPatch("/deactivate")]
        public async Task<IActionResult> DeActivateAsync(Guid identifier)
        {
            var command = this.commandFactory.CreateRegisterOrUpdateAccessPoint();
            var found = await command.RegisterOrUpdateAsync(new RegisterUpdateAccessPointRequestModel {  Identifier = identifier, IsActive = false });
            if (found)
            {
                return this.Ok();
            }
            else
            {
                return this.NotFound();
            }
        }

        /// <summary>
        /// Changes the access level of the specified access point
        /// </summary>
        /// <returns>
        /// 200 if the access point's access level was changed
        /// 404 if there is no such access point
        /// </returns>
        [HttpPatch("accessLevel")]
        public async Task<IActionResult> ChangeAccessLevelAsync(Guid identifier, AccessLevel accessLevel)
        {
            var command = this.commandFactory.CreateRegisterOrUpdateAccessPoint();
            var found = await command.RegisterOrUpdateAsync(new RegisterUpdateAccessPointRequestModel { Identifier = identifier, AccessLevel = accessLevel });
            if (found)
            {
                return this.Ok();
            }
            else
            {
                return this.NotFound();
            }
        }
    }
}

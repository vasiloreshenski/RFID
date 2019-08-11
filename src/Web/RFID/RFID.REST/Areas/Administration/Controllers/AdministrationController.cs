namespace RFID.REST.Areas.Administration.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using RFID.REST.Areas.Administration.Commands;
    using RFID.REST.Areas.Administration.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Controller used for administration purposes like registering, deleting, deactivating tags
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AdministrationController : ControllerBase
    {
        private readonly AdministrationCommandFactory commandFactory;

        public AdministrationController(AdministrationCommandFactory commandFactory)
        {
            this.commandFactory = commandFactory;
        }

        /// <summary>
        /// Registers new tag
        /// </summary>
        /// <param name="model">Reqister model</param>
        /// <returns>
        /// 200 OK if the model was registered successfully. 
        /// 400 if the request model is not valid or the tag is already registered.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> RegisterTagAsync(RegisterTagRequestModel model)
        {
            var command = this.commandFactory.CreateRegisterTagCommand();
            var registered = await command.RegisterAsync(model);
            if (registered)
            {
                return this.Ok();
            }
            else
            {
                return this.BadRequest();
            }
        }
    }
}

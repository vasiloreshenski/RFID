namespace RFID.REST.Areas.Administration.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using RFID.REST.Areas.Administration.Commands;
    using RFID.REST.Areas.Administration.Models;
    using RFID.REST.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Controller used for administration purposes like registering, deleting, deactivating tags
    /// </summary>
    [Route("api/tags")]
    [Area("administration")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly CommandFactory commandFactory;

        public TagsController(CommandFactory commandFactory)
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
        [HttpPost("/register")]
        public async Task<IActionResult> RegisterAsync(RegisterTagRequestModel model)
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
          
        /// <summary>
        /// Activates the tag specified tag
        /// </summary>
        /// <param name="tagId">id of the tag</param>
        /// <returns>
        /// 200 OK if the model was updated
        /// 400 if the request model is invalid
        /// 404 if the tag with the specified id was not found
        /// </returns>
        [HttpPatch("/activate")]
        public async Task<IActionResult> ActivateAsync(int tagId)
        {
            var command = this.commandFactory.CreateUpdateTagCommand();
            var found = await command.UpdateAsync(new UpdateTagRequestModel { TagId = tagId, IsActive = true });
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
        /// Deactivates the specified tag
        /// </summary>
        /// <returns></returns>
        [HttpPatch("/deactivate")]
        public async Task<IActionResult> DeActivateAsync(int tagId)
        {
            var command = this.commandFactory.CreateUpdateTagCommand();
            var found = await command.UpdateAsync(new UpdateTagRequestModel { TagId = tagId, IsActive = false });
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
        /// Deletes the specified tag
        /// </summary>
        /// <param name="tagId">id of the tag</param>
        /// <returns></returns>
        [HttpPatch("/delete")]
        public async Task<IActionResult> DeleteAsync(int tagId)
        {
            var command = this.commandFactory.CreateUpdateTagCommand();
            var found = await command.UpdateAsync(new UpdateTagRequestModel { TagId = tagId, IsDeleted = true });

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
        /// Changes the access level of the specified tag
        /// </summary>
        /// <returns></returns>
        [HttpPatch("/accessLevel")]
        public async Task<IActionResult> ChangeAccessLevelAsync(int tagId, AccessLevel accessLevel)
        {
            var command = this.commandFactory.CreateUpdateTagCommand();
            var found = await command.UpdateAsync(new UpdateTagRequestModel { TagId = tagId, AccessLevel = accessLevel });

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

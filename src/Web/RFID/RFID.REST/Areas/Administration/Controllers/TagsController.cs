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

        /// <summary>
        /// Activates the tag specified tag
        /// </summary>
        /// <param name="tagId">id of the tag</param>
        /// <returns></returns>
        [HttpPatch("/activate")]
        public async Task<IActionResult> ActivateTagAsync(int tagId)
        {
            var command = this.commandFactory.CreateUpdateTagCommand();
            await command.UpdateAsync(new UpdateTagRequestModel { TagId = tagId, IsActive = true });

            return this.Ok();
        }

        /// <summary>
        /// Deactivates the specified tag
        /// </summary>
        /// <returns></returns>
        [HttpPatch("/deactivate")]
        public async Task<IActionResult> DeActivateTagAsync(int tagId)
        {
            var command = this.commandFactory.CreateUpdateTagCommand();
            await command.UpdateAsync(new UpdateTagRequestModel { TagId = tagId, IsActive = false });

            return this.Ok();
        }

        /// <summary>
        /// Deletes the specified tag
        /// </summary>
        /// <param name="tagId">id of the tag</param>
        /// <returns></returns>
        [HttpPatch("delete")]
        public async Task<IActionResult> DeleteTagAsync(int tagId)
        {
            var command = this.commandFactory.CreateUpdateTagCommand();
            await command.UpdateAsync(new UpdateTagRequestModel { TagId = tagId, IsDeleted = true });

            return this.Ok();
        }
    }
}

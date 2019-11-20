﻿namespace RFID.REST.Areas.Administration.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using RFID.REST.Areas.Administration.Commands;
    using RFID.REST.Areas.Administration.Models;
    using RFID.REST.Common;
    using RFID.REST.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Controller used for administration purposes like registering, deleting, deactivating tags
    /// </summary>
    [Route("[area]/api/tags")]
    [Area("administration")]
    [ApiController]
    [Authorize(PolicyNames.AdminPolicy)]
    public class TagsController : ControllerBase
    {
        private readonly CommandFactory commandFactory;
        private readonly Database.Database database;

        public TagsController(CommandFactory commandFactory, Database.Database database)
        {
            this.commandFactory = commandFactory;
            this.database = database;
        }

        /// <summary>
        /// Registers new tag
        /// </summary>
        /// <param name="model">Reqister model</param>
        /// <returns>
        /// 200 OK if the model was registered successfully. 
        /// 400 if the request model is not valid or the tag is already registered.
        /// </returns>
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(RegisterTagRequestModel model)
        {
            var command = this.commandFactory.CreateRegisterTagCommand();
            var commandResult = await command.RegisterAsync(model);
            if (commandResult.Success)
            {
                return this.Ok();
            }
            else
            {
                return this.BadRequest(commandResult);
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
        [HttpPatch("activate")]
        public async Task<IActionResult> ActivateAsync(Identity identity)
        {
            var command = this.commandFactory.CreateUpdateTagCommand();
            var commandResult = await command.UpdateAsync(new UpdateTagRequestModel { Id = identity.Id, IsActive = true });
            if (commandResult.Success)
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
        [HttpPatch("deactivate")]
        public async Task<IActionResult> DeActivateAsync(Identity identity)
        {
            var command = this.commandFactory.CreateUpdateTagCommand();
            var commandResult = await command.UpdateAsync(new UpdateTagRequestModel { Id = identity.Id, IsActive = false });
            if (commandResult.Success)
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
        [HttpPatch("delete")]
        public async Task<IActionResult> DeleteAsync(Identity identity)
        {
            var command = this.commandFactory.CreateUpdateTagCommand();
            var commandResult = await command.UpdateAsync(new UpdateTagRequestModel { Id = identity.Id, IsDeleted = true });

            if (commandResult.Success)
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
        [HttpPatch("accessLevel")]
        public async Task<IActionResult> ChangeAccessLevelAsync(ChangeTagAccessLevelRequestModel requestModel)
        {
            var command = this.commandFactory.CreateUpdateTagCommand();
            var commandResult = await command.UpdateAsync(new UpdateTagRequestModel { Id = requestModel.Id, AccessLevel = requestModel.AccessLevel });

            if (commandResult.Success)
            {
                return this.Ok();
            }
            else
            {
                return this.NotFound();
            }
        }

        [HttpPatch("update")]
        public async Task<IActionResult> UpdateTagAsync(UpdateTagRequestModel requestModel)
        {
            var command = this.commandFactory.CreateUpdateTagCommand();
            var commandResult = await command.UpdateAsync(requestModel);

            if (commandResult.Success)
            {
                return this.Ok();
            }
            else
            {
                return this.NotFound();
            }
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetAllActiveAsync()
        {
            return this.Ok(await this.database.GetAllActiveTagsAsync());
        }

        [HttpGet("inactive")]
        public async Task<IActionResult> GetAllInActiveAsync()
        {
            return this.Ok(await this.database.GetAllInActiveTagsAsync());
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsersAsync()
        {
            var users = await this.database.GetAllTagsUsersAsync();
            return this.Ok(users);
        }

        [HttpGet("unknown")]
        public async Task<IActionResult> GetAllUnKnownTagsAsync()
        {
            var unknown = await this.database.GetAllUnKnownActiveTagsAsync();
            return this.Ok(unknown);
        }
    }
}

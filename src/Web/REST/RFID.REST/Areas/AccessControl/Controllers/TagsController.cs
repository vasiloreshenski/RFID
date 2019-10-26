namespace RFID.REST.Areas.AccessControl.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using RFID.REST.Areas.AccessControl.Models;
    using RFID.REST.Database;

    /// <summary>
    /// Controller used for access control checks
    /// </summary>
    [Route("[area]/api/[controller]")]
    [Area("accessControl")]
    [ApiController]
    [AllowAnonymous]
    public class TagsController : ControllerBase
    {
        private readonly Database database;

        public TagsController(Database database)
        {
            this.database = database;
        }

        /// <summary>
        /// Check if the specified specified tag has access to the proved access point
        /// </summary>
        /// <param name="model">request model</param>
        /// <returns>200 with json response with the access result. example -> { "has_access": "true" } or 404 when the tag or the access point does not exists</returns>
        [HttpGet("checkAccess")]
        public async Task<IActionResult> CheckAccessAsync([FromQuery]CheckTagAccessRequestModel model)
        {
            var tagAccessLevel = await this.database.GetAccessLevelForTagAsync(model.TagNumber);
            var accessPointLevel = await this.database.GetAccessLevelForAccessPointAsync(model.AccessPointSerialNumber);
            if (tagAccessLevel == null || accessPointLevel == null)
            {
                return this.NotFound();
            }
            else
            {
                var hasAccess = tagAccessLevel >= accessPointLevel;
                if (hasAccess)
                {
                    return this.Ok();
                }
                else
                {
                    return this.Unauthorized();
                }
            }
        }
    }
}
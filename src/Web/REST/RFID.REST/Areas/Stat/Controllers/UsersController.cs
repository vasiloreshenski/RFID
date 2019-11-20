namespace RFID.REST.Areas.Stat.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using RFID.REST.Areas.Stat.Models;
    using RFID.REST.Common;
    using RFID.REST.Database;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;


    [Area("stat")]
    [Route("[area]/api/[controller]")]
    [Authorize(PolicyNames.AdminPolicy)]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly Database database;

        public UsersController(Database database)
        {
            this.database = database;
        }

        [HttpGet("overview")]
        public async Task<IActionResult> GetUserOverviewAsync()
        {
            var avgEntranceTime = await this.database.GetUsersAvgEntranceTimeAsync();
            var avgExitTime = await this.database.GetUsersAvgExitTimeAsync();
            var avgWorkHourNorm = await this.database.GetUsersAvgWorkHourNormAsync();

            return this.Ok(new UserOverviewResponeModel
            {
                AvgEntranceTime = avgEntranceTime,
                AvgExitTime = avgExitTime,
                AvgWorkHourNorm = avgWorkHourNorm
            });
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsersAsync()
        {
            return this.Ok(await this.database.GetUsersStatAsync());
        }

        [HttpGet("norm")]
        public async Task<IActionResult> GetWorkHourNormAsync([FromQuery]UserEventsInRangeRequestModel requestModel)
        {
            return this.Ok(await this.database.GetUserWorkHourNormForRangeAsync(requestModel.UserId, requestModel.StartDate, requestModel.EndDate));
        }

        [HttpGet("entrance")]
        public async Task<IActionResult> GetEntranceTimeAsync([FromQuery]UserEventsInRangeRequestModel requestModel)
        {
            return this.Ok(await this.database.GetUserEntranceTimeForRangeAsync(requestModel.UserId, requestModel.StartDate, requestModel.EndDate));
        }

        [HttpGet("exit")]
        public async Task<IActionResult> GetExitTimeAsync([FromQuery]UserEventsInRangeRequestModel requestModel)
        {
            return this.Ok(await this.database.GetUserExitTimeForRangeAsync(requestModel.UserId, requestModel.StartDate, requestModel.EndDate));
        }
    }
}

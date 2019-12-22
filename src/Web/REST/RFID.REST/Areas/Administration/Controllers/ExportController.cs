namespace RFID.REST.Areas.Administration.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using RFID.REST.Common;
    using RFID.REST.Database;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    [ApiController]
    [Route("[area]/api/export")]
    [Area("administration")]
    [Authorize(Policy = PolicyNames.AdminPolicy)]
    public class ExportController : Controller
    {
        private readonly Database database;

        public ExportController(Database database)
        {
            this.database = database;
        }

        [HttpGet("")]
        public async Task<IActionResult> ExportAsync()
        {
            var jsonStr = await this.database.ExportAsJsonAsync();
            return this.Content(jsonStr, "application/json");
        }
    }
}

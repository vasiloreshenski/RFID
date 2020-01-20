namespace RFID.Seed.Internal
{
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Configuration;
    using RFID.REST;
    using RFID.REST.Areas.Administration.Controllers;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using RFID.REST.Models;
    using System.Linq;
    using RFID.REST.Areas.Administration.Models;
    using RFID.REST.Areas.AccessControl.Controllers;
    using RFID.REST.Database;
    using TagsController = REST.Areas.Administration.Controllers.TagsController;
    using System.IO;
    using System.Data.SqlClient;
    using Dapper;
    using RFID.REST.Areas.AccessControl.Models;

    internal static class SeedClient
    {
        private static Random random = new Random();

        public static async Task SeedAsync()
        {
            using (var webhost = Configure())
            {
                await RfidDockerHttpClient.RestMssqlAsync(webhost.Services.GetService<SqlConnectionFactory>());

                await InsertAdministratorAsync(webhost.Services);
                await InsertAccessPoints(webhost.Services);
                await InsertUnknownAccessPointsAsync(webhost.Services);
                await InsertTagsAsync(webhost.Services);
                await GenerateEventsAsync(webhost.Services);
            }
        }

        private static async Task InsertAdministratorAsync(IServiceProvider services)
        {
            var controller = services.GetService<UsersController>();
            await controller.RegisterAsync(new REST.Areas.Administration.Models.RegisterAdministrationUserRequestModel { Email = "test@test.com", Password = "123", Roles = REST.Areas.Administration.Models.UserRoles.Admin });
        }

        private static async Task InsertAccessPoints(IServiceProvider services)
        {
            var controller = services.GetService<AccessPointController>();
            var entrances = new[] { "A", "B", "C", "D", "E" };
            var sides = new[] { "south", "east", "west", "north" };
            var bools = new[] { true, false };
            var accessLevels = new[] { AccessLevel.Low, AccessLevel.Mid, AccessLevel.High };
            var diretions = new[] { AccessPointDirectionType.Entrance, AccessPointDirectionType.Exit };

            for (int i = 0; i < random.Next(15, 30); i++)
            {
                var direction = Shuffle(diretions);
                var entranceExitText = direction == AccessPointDirectionType.Entrance ? "entrance" : "exit";

                await controller.RegisterAsync(new REST.Areas.Administration.Models.RegisterAccessPointRequestModel
                {
                    AccessLevel = Shuffle(accessLevels),
                    Description = $"Building {random.Next(1, 10)}, {entranceExitText} {Shuffle(entrances)} floor {random.Next(1, 10)}, {Shuffle(sides)} side",
                    Direction = direction,
                    IsActive = Shuffle(bools),
                    SerialNumber = $"{Guid.NewGuid()}"
                });
            }
        }

        private static async Task InsertUnknownAccessPointsAsync(IServiceProvider services)
        {
            var controller = services.GetService<REST.Areas.AccessControl.Controllers.TagsController>();

            for (int i = 0; i < random.Next(1, 10); i++)
            {
                await controller.CheckAccessAsync(new REST.Areas.AccessControl.Models.CheckTagAccessRequestModel
                {
                    TagNumber = $"{Guid.NewGuid()}",
                    AccessPointSerialNumber = $"{Guid.NewGuid()}"
                });
            }
        }

        private static async Task InsertTagsAsync(IServiceProvider services)
        {
            var controller = services.GetService<TagsController>();
            var entrances = new[] { "A", "B", "C", "D", "E" };
            var sides = new[] { "south", "east", "west", "north" };
            var bools = new[] { true, false };
            var fnames = new[] { "john", "bratt", "goerge", "harry", "ron", "scarlet", "marry", "francesca" };
            var lnames = new[] { "bit", "malkovic", "davidson", "pike", "wood", "round" };

            for (int i = 0; i < random.Next(15, 30); i++)
            {
                await controller.RegisterAsync(new REST.Areas.Administration.Models.RegisterTagRequestModel
                {
                    AccessLevel = (AccessLevel)random.Next(1, 3),
                    Number = $"{Guid.NewGuid()}",
                    UserName = $"{Shuffle(fnames)} {Shuffle(lnames)}"
                });
            }
        }

        private static async Task GenerateEventsAsync(IServiceProvider services)
        {
            var configuration = services.GetService<IConfiguration>();
            var tagController = services.GetService<REST.Areas.AccessControl.Controllers.TagsController>();

            var connectionString = configuration.GetConnectionString("Rfid");

            var accessPointInfo = await GetAccessPointSerialNumbersAsync(connectionString);
            var tagInfo = await GetTagNumbersAsync(connectionString);

            foreach (var (tagNumber, isActive, isUnknown) in tagInfo)
            {
                var date = DateTime.Now.AddMonths(-1);
                // simulate for fulll month
                while((date.Month < DateTime.Now.Month || date.Year < DateTime.Now.Year) || (date.Month == DateTime.Now.Month && date.Day <= DateTime.Now.Day))
                {
                    // get in
                    var entranceAccessPoint = Shuffle(accessPointInfo.Where(x => x.isActive && x.direction == AccessPointDirectionType.Entrance));
                    await tagController.CheckAccessAsync(new CheckTagAccessRequestModel { TagNumber = tagNumber, AccessPointSerialNumber = entranceAccessPoint.serialNumber });

                    var enterHour = random.Next(6, 11);
                    if (isActive)
                    {
                        var enterDate = new DateTime(date.Year, date.Month, day: date.Day, hour: enterHour, minute: random.Next(0, 59), second: random.Next(0, 59));
                        await SetLastEventCreateDate(enterDate, connectionString);
                    }

                    // lunch break
                    if (isActive)
                    {
                        // get out for lunch break
                        var lunchBreakExitAccessPoint = Shuffle(accessPointInfo.Where(x => x.isActive && x.direction == AccessPointDirectionType.Exit));
                        await tagController.CheckAccessAsync(new CheckTagAccessRequestModel { TagNumber = tagNumber, AccessPointSerialNumber = lunchBreakExitAccessPoint.serialNumber });
                        var exitHour = random.Next(enterHour + 2, 17 - 2);
                        var exitDate = new DateTime(date.Year, date.Month, day: date.Day, hour: exitHour, minute: random.Next(0, 59), second: random.Next(0, 59));
                        await SetLastEventCreateDate(exitDate, connectionString);


                        // get back from lunch break
                        entranceAccessPoint = Shuffle(accessPointInfo.Where(x => x.isActive && x.direction == AccessPointDirectionType.Entrance));
                        await tagController.CheckAccessAsync(new CheckTagAccessRequestModel { TagNumber = tagNumber, AccessPointSerialNumber = entranceAccessPoint.serialNumber });
                        enterHour = random.Next(exitHour + 1, 17); // +1 to avoid muniutes ignoring
                        var enterDate = new DateTime(date.Year, date.Month, day: date.Day, hour: enterHour, minute: random.Next(0, 59), second: random.Next(0, 59));
                        await SetLastEventCreateDate(enterDate, connectionString);
                    }


                    // get out
                    var exitAccessPoint = Shuffle(accessPointInfo.Where(x => x.isActive && x.direction == AccessPointDirectionType.Exit));
                    await tagController.CheckAccessAsync(new CheckTagAccessRequestModel { TagNumber = tagNumber, AccessPointSerialNumber = exitAccessPoint.serialNumber });
                    if (isActive)
                    {
                        var exitHour = random.Next(enterHour + 1, 23);
                        var exitDate = new DateTime(date.Year, date.Month, day: date.Day, hour: exitHour, minute: random.Next(0, 59), second: random.Next(0, 59));
                        await SetLastEventCreateDate(exitDate, connectionString);
                    }

                    date = date.AddDays(1);
                }
            }

            // todo: simualte unknown access points
        }

        private static IWebHost Configure()
        {
            var webHost = WebHost
                .CreateDefaultBuilder()
                .UseStartup<Startup>()
                .ConfigureServices(s =>
                    s.AddSingleton<UsersController>()
                    .AddSingleton<AccessPointController>()
                    .AddSingleton<TagsController>()
                    .AddSingleton<REST.Areas.AccessControl.Controllers.TagsController>()
                )
                .ConfigureAppConfiguration(b => b.AddJsonFile("appsettings.json"))
                .Build();

            return webHost;
        }

        private static T Shuffle<T>(IEnumerable<T> collection)
        {
            return Enumerable.Range(1, 10).Select(x => collection).Aggregate((f, s) => f.Concat(s)).OrderBy(_ => random.Next(1, 1000)).First();
        }

        private static async Task<IReadOnlyCollection<(String serialNumber, AccessPointDirectionType? direction, bool isActive)>> GetAccessPointSerialNumbersAsync(String connectionString)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var dbResult = await connection.QueryAsync<(String, AccessPointDirectionType? direction, bool isActive)>(
                   @"select x.SerialNumber, x.DirectionId, x.IsActive from access_control.AccessPoints as x " +
                   "union " +
                   "select x.SerialNumber, null as DirectionId, cast(0 as bit) as [IsActive] from access_control.UnKnownAccessPoints as x"
                );

                return dbResult.ToList();
            }
        }

        private static async Task<IReadOnlyCollection<(String number, bool isActive, bool isUnknown)>> GetTagNumbersAsync(String connectionString)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var dbResult = await connection.QueryAsync<(String, bool, bool)>(
                    "select x.Number, x.IsActive, cast(0 as bit) as IsUnKnown from access_control.Tags as x union select x.Number, cast(0 as bit) IsActive, cast(1 as bit) IsUnKnown from access_control.UnKnownTags as x"
                );

                return dbResult.ToList();
            }
        }

        private static async Task SetLastEventCreateDate(DateTime date, String connectionString)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.ExecuteAsync(@"
                    update stat.[Events]
                    set CreateDate = @date
                    where Id = (select top 1 x.Id from stat.[Events] as x order by x.Id desc)
                    ", param: new { @date = date }
                );
            }
        }
    }
}

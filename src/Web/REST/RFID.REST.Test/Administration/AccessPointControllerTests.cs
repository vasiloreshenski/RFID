namespace RFID.REST.Test.Administration
{
    using RFID.REST.Models;
    using RFID.REST.Test.Common;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    [Collection("Integration")]
    public class AccessPointControllerTests
    {
        public AccessPointControllerTests()
        {
            RfidDockerHttpClient.RestMssqlAsync().Wait();
        }

        #region Register

        [Fact]
        public async Task Register_When_New_Access_Point()
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();
            var userRM = Examples.Administrator();
            var accessPointRM = Examples.AccessPoint();

            await RfidHttpClient.RegisterUserAsync(userRM);
            using (var authTokenResponseMessage = await RfidHttpClient.GenerateAuthTokenAsync(userRM))
            {
                var authToken = await AuthTokenHelper.FromHttpResponseMessageAsync(authTokenResponseMessage);
                var token = await authToken.GetTokenAsync();

                using (var httpResponse = await RfidHttpClient.RegisterAccessPointAsync(accessPointRM, token))
                {
                    RfidAssert.AssertHttpResponse(httpResponse, System.Net.HttpStatusCode.OK);
                }
            }

            await assertDatabase.AssertCntAsync(userRM, accessPointRM);
        }

        [Fact]
        public async Task Register_When_Several_Access_Points()
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();
            var userRM = Examples.Administrator();
            var faccessPointRM = Examples.AccessPoint("test1");
            var saccessPointRM = Examples.AccessPoint("test2");

            await RfidHttpClient.RegisterUserAsync(userRM);
            using (var authTokenResponseMessage = await RfidHttpClient.GenerateAuthTokenAsync(userRM))
            {
                var authToken = await AuthTokenHelper.FromHttpResponseMessageAsync(authTokenResponseMessage);
                var token = await authToken.GetTokenAsync();

                using (var fhttpResponse = await RfidHttpClient.RegisterAccessPointAsync(faccessPointRM, token))
                using (var shttpResponse = await RfidHttpClient.RegisterAccessPointAsync(saccessPointRM, token))
                {
                    RfidAssert.AssertHttpResponse(fhttpResponse, System.Net.HttpStatusCode.OK);
                    RfidAssert.AssertHttpResponse(shttpResponse, System.Net.HttpStatusCode.OK);
                }
            }

            await assertDatabase.AssertCntAsync(userRM, faccessPointRM, saccessPointRM);
        }

        [Fact]
        public async Task Register_When_Access_Point_Already_Exists()
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();
            var userRM = Examples.Administrator();
            var accessPointRM = Examples.AccessPoint("test1");

            await RfidHttpClient.RegisterUserAsync(userRM);
            using (var authTokenResponseMessage = await RfidHttpClient.GenerateAuthTokenAsync(userRM))
            {
                var authToken = await AuthTokenHelper.FromHttpResponseMessageAsync(authTokenResponseMessage);
                var token = await authToken.GetTokenAsync();

                using (var fhttpResponse = await RfidHttpClient.RegisterAccessPointAsync(accessPointRM, token))
                using (var shttpResponse = await RfidHttpClient.RegisterAccessPointAsync(accessPointRM, token))
                {
                    RfidAssert.AssertHttpResponse(fhttpResponse, System.Net.HttpStatusCode.OK);
                    await RfidAssert.AssertHttpResponseAsync(shttpResponse, System.Net.HttpStatusCode.BadRequest, (false, CommandStatus.Dublicate));
                }
            }

            await assertDatabase.AssertCntAsync(userRM, accessPointRM);
        }

        [Fact]
        public async Task Register_When_Not_Authorized()
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();
            var userRM = Examples.Administrator();
            var accessPointRM = Examples.AccessPoint();

            await RfidHttpClient.RegisterUserAsync(userRM);
            using (var authTokenResponseMessage = await RfidHttpClient.GenerateAuthTokenAsync(userRM))
            {
                using (var httpResponse = await RfidHttpClient.RegisterAccessPointAsync(accessPointRM, "invalid-access-token"))
                {
                    RfidAssert.AssertHttpResponse(httpResponse, System.Net.HttpStatusCode.Unauthorized);
                }
            }

            await assertDatabase.AssertCntAsync(userRM);
        }

        [Theory]
        [InlineData("", "", 0)]
        [InlineData(null, "", 0)]
        [InlineData("", null, 0)]
        [InlineData("", "", 1)]
        [InlineData(null, null, 2)]
        public async Task Register_When_Invalid_Data(String serialNumber, String description, int accessLevel)
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();
            var userRM = Examples.Administrator();
            var accessPointRM = Examples.AccessPoint(serialNumber, description, (AccessLevel)accessLevel);

            await RfidHttpClient.RegisterUserAsync(userRM);
            using (var authTokenResponseMessage = await RfidHttpClient.GenerateAuthTokenAsync(userRM))
            {
                var authToken = await AuthTokenHelper.FromHttpResponseMessageAsync(authTokenResponseMessage);
                var token = await authToken.GetTokenAsync();

                using (var httpResponse = await RfidHttpClient.RegisterAccessPointAsync(accessPointRM, token))
                {
                    RfidAssert.AssertHttpResponse(httpResponse, System.Net.HttpStatusCode.BadRequest);
                }
            }

            await assertDatabase.AssertCntAsync(userRM);
        }

        #endregion Register

        #region Activate

        [Fact]
        public async Task Activate_When_Already_Activated()
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();
            var userRM = Examples.Administrator();
            var accessPointRM = Examples.AccessPoint();
            var accessPointId = 0;

            await RfidHttpClient.RegisterUserAsync(userRM);

            using (var authTokenResponse = await RfidHttpClient.GenerateAuthTokenAsync(userRM))
            {
                var authToken = await AuthTokenHelper.FromHttpResponseMessageAsync(authTokenResponse);
                var token = await authToken.GetTokenAsync();

                using (var registerAccessPointResponse = await RfidHttpClient.RegisterAccessPointAsync(accessPointRM, token))
                using (var activateAccessPointResponse = await RfidHttpClient.ActivateAccessPointAsync(accessPointRM.SerialNumber, token))
                {
                    RfidAssert.AssertHttpResponse(activateAccessPointResponse, System.Net.HttpStatusCode.OK);

                    accessPointId = await RfidDatabase.GetAccessPointIdBySerialNumberAsync(accessPointRM.SerialNumber);
                }
            }

            await assertDatabase.AssertCntAsync(userRM, accessPointRM);
            await assertDatabase.AssertStateAsync("access_control.AccessPoints", accessPointId, new { Id = accessPointId, IsActive = true });
        }

        [Fact]
        public async Task Activate_When_Deactivated()
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();
            var userRM = Examples.Administrator();
            var accessPointRM = Examples.AccessPoint();
            var accessPointId = 0;

            await RfidHttpClient.RegisterUserAsync(userRM);

            using (var authTokenResponse = await RfidHttpClient.GenerateAuthTokenAsync(userRM))
            {
                var authToken = await AuthTokenHelper.FromHttpResponseMessageAsync(authTokenResponse);
                var token = await authToken.GetTokenAsync();

                using (var registerAccessPointResponse = await RfidHttpClient.RegisterAccessPointAsync(accessPointRM, token))
                {
                    accessPointId = await RfidDatabase.GetAccessPointIdBySerialNumberAsync(accessPointRM.SerialNumber);

                    await RfidDatabase.DeActivateAccessPointAsync(accessPointId);

                    using (var activateAccessPointResponse = await RfidHttpClient.ActivateAccessPointAsync(accessPointRM.SerialNumber, token))
                    {
                        RfidAssert.AssertHttpResponse(activateAccessPointResponse, System.Net.HttpStatusCode.OK);
                    }
                }
            }

            await assertDatabase.AssertCntAsync(userRM, accessPointRM);
            await assertDatabase.AssertStateAsync("access_control.AccessPoints", accessPointId, new { Id = accessPointId, IsActive = true });
        }

        [Fact]
        public async Task Activate_When_Several()
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();
            var userRM = Examples.Administrator();
            var faccessPointRM = Examples.AccessPoint("test1");
            var saccessPointRM = Examples.AccessPoint("test2");

            var faccessPointId = 0;
            var saccessPointId = 0;

            await RfidHttpClient.RegisterUserAsync(userRM);

            using (var authTokenResponse = await RfidHttpClient.GenerateAuthTokenAsync(userRM))
            {
                var authToken = await AuthTokenHelper.FromHttpResponseMessageAsync(authTokenResponse);
                var token = await authToken.GetTokenAsync();

                using (var fregisterAccessPointResponse = await RfidHttpClient.RegisterAccessPointAsync(faccessPointRM, token))
                using (var sregisterAccessPointResponse = await RfidHttpClient.RegisterAccessPointAsync(saccessPointRM, token))
                {
                    faccessPointId = await RfidDatabase.GetAccessPointIdBySerialNumberAsync(faccessPointRM.SerialNumber);
                    saccessPointId = await RfidDatabase.GetAccessPointIdBySerialNumberAsync(saccessPointRM.SerialNumber);

                    await RfidDatabase.DeActivateAccessPointAsync(faccessPointId);
                    await RfidDatabase.DeActivateAccessPointAsync(saccessPointId);

                    using (var activateAccessPointResponse = await RfidHttpClient.ActivateAccessPointAsync(faccessPointRM.SerialNumber, token))
                    {
                        RfidAssert.AssertHttpResponse(activateAccessPointResponse, System.Net.HttpStatusCode.OK);
                    }
                }
            }

            await assertDatabase.AssertCntAsync(userRM, faccessPointRM, saccessPointRM);
            await assertDatabase.AssertStateAsync("access_control.AccessPoints", faccessPointId, new { Id = faccessPointId, IsActive = true });
            await assertDatabase.AssertStateAsync("access_control.AccessPoints", saccessPointId, new { Id = saccessPointId, IsActive = false });
        }

        [Fact]
        public async Task Activate_When_Does_Not_Exists()
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();
            var userRM = Examples.Administrator();
            var accessPointRM = Examples.AccessPoint();
            var accessPointId = 0;

            await RfidHttpClient.RegisterUserAsync(userRM);

            using (var authTokenResponse = await RfidHttpClient.GenerateAuthTokenAsync(userRM))
            {
                var authToken = await AuthTokenHelper.FromHttpResponseMessageAsync(authTokenResponse);
                var token = await authToken.GetTokenAsync();

                using (var registerAccessPointResponse = await RfidHttpClient.RegisterAccessPointAsync(accessPointRM, token))
                {
                    accessPointId = await RfidDatabase.GetAccessPointIdBySerialNumberAsync(accessPointRM.SerialNumber);

                    await RfidDatabase.DeActivateAccessPointAsync(accessPointId);

                    using (var activateAccessPointResponse = await RfidHttpClient.ActivateAccessPointAsync("unknown", token))
                    {
                        RfidAssert.AssertHttpResponse(activateAccessPointResponse, System.Net.HttpStatusCode.NotFound);
                    }
                }
            }

            await assertDatabase.AssertCntAsync(userRM, accessPointRM);
            await assertDatabase.AssertStateAsync("access_control.AccessPoints", accessPointId, new { Id = accessPointId, IsActive = false });
        }

        [Fact]
        public async Task Activate_When_Not_Authorized()
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();
            var userRM = Examples.Administrator();
            var accessPointRM = Examples.AccessPoint();
            var accessPointId = 0;

            await RfidHttpClient.RegisterUserAsync(userRM);

            using (var authTokenResponse = await RfidHttpClient.GenerateAuthTokenAsync(userRM))
            {
                var authToken = await AuthTokenHelper.FromHttpResponseMessageAsync(authTokenResponse);
                var token = await authToken.GetTokenAsync();

                using (var registerAccessPointResponse = await RfidHttpClient.RegisterAccessPointAsync(accessPointRM, token))
                {
                    accessPointId = await RfidDatabase.GetAccessPointIdBySerialNumberAsync(accessPointRM.SerialNumber);

                    await RfidDatabase.DeActivateAccessPointAsync(accessPointId);

                    using (var activateAccessPointResponse = await RfidHttpClient.ActivateAccessPointAsync(accessPointRM.SerialNumber, "unknown"))
                    {
                        RfidAssert.AssertHttpResponse(activateAccessPointResponse, System.Net.HttpStatusCode.Unauthorized);
                    }
                }
            }

            await assertDatabase.AssertCntAsync(userRM, accessPointRM);
            await assertDatabase.AssertStateAsync("access_control.AccessPoints", accessPointId, new { Id = accessPointId, IsActive = false });
        }

        #endregion Activate


        #region De-activate

        [Fact]
        public async Task DeActivate_When_Activated()
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();
            var userRM = Examples.Administrator();
            var accessPointRM = Examples.AccessPoint();
            var accessPointId = 0;

            await RfidHttpClient.RegisterUserAsync(userRM);

            using (var authTokenResponse = await RfidHttpClient.GenerateAuthTokenAsync(userRM))
            {
                var authToken = await AuthTokenHelper.FromHttpResponseMessageAsync(authTokenResponse);
                var token = await authToken.GetTokenAsync();

                using (var registerAccessPointResponse = await RfidHttpClient.RegisterAccessPointAsync(accessPointRM, token))
                using (var activateAccessPointResponse = await RfidHttpClient.DeActivateAccessPointAsync(accessPointRM.SerialNumber, token))
                {
                    RfidAssert.AssertHttpResponse(activateAccessPointResponse, System.Net.HttpStatusCode.OK);

                    accessPointId = await RfidDatabase.GetAccessPointIdBySerialNumberAsync(accessPointRM.SerialNumber);
                }
            }

            await assertDatabase.AssertCntAsync(userRM, accessPointRM);
            await assertDatabase.AssertStateAsync("access_control.AccessPoints", accessPointId, new { Id = accessPointId, IsActive = false });
        }

        [Fact]
        public async Task DeActivate_When_Already_Deactivated()
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();
            var userRM = Examples.Administrator();
            var accessPointRM = Examples.AccessPoint();
            var accessPointId = 0;

            await RfidHttpClient.RegisterUserAsync(userRM);

            using (var authTokenResponse = await RfidHttpClient.GenerateAuthTokenAsync(userRM))
            {
                var authToken = await AuthTokenHelper.FromHttpResponseMessageAsync(authTokenResponse);
                var token = await authToken.GetTokenAsync();

                using (var registerAccessPointResponse = await RfidHttpClient.RegisterAccessPointAsync(accessPointRM, token))
                {
                    accessPointId = await RfidDatabase.GetAccessPointIdBySerialNumberAsync(accessPointRM.SerialNumber);

                    await RfidDatabase.DeActivateAccessPointAsync(accessPointId);

                    using (var activateAccessPointResponse = await RfidHttpClient.DeActivateAccessPointAsync(accessPointRM.SerialNumber, token))
                    {
                        RfidAssert.AssertHttpResponse(activateAccessPointResponse, System.Net.HttpStatusCode.OK);
                    }
                }
            }

            await assertDatabase.AssertCntAsync(userRM, accessPointRM);
            await assertDatabase.AssertStateAsync("access_control.AccessPoints", accessPointId, new { Id = accessPointId, IsActive = false });
        }

        [Fact]
        public async Task DeActivate_When_Several()
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();
            var userRM = Examples.Administrator();
            var faccessPointRM = Examples.AccessPoint("test1");
            var saccessPointRM = Examples.AccessPoint("test2");

            var faccessPointId = 0;
            var saccessPointId = 0;

            await RfidHttpClient.RegisterUserAsync(userRM);

            using (var authTokenResponse = await RfidHttpClient.GenerateAuthTokenAsync(userRM))
            {
                var authToken = await AuthTokenHelper.FromHttpResponseMessageAsync(authTokenResponse);
                var token = await authToken.GetTokenAsync();

                using (var fregisterAccessPointResponse = await RfidHttpClient.RegisterAccessPointAsync(faccessPointRM, token))
                using (var sregisterAccessPointResponse = await RfidHttpClient.RegisterAccessPointAsync(saccessPointRM, token))
                {
                    faccessPointId = await RfidDatabase.GetAccessPointIdBySerialNumberAsync(faccessPointRM.SerialNumber);
                    saccessPointId = await RfidDatabase.GetAccessPointIdBySerialNumberAsync(saccessPointRM.SerialNumber);

                    using (var activateAccessPointResponse = await RfidHttpClient.DeActivateAccessPointAsync(faccessPointRM.SerialNumber, token))
                    {
                        RfidAssert.AssertHttpResponse(activateAccessPointResponse, System.Net.HttpStatusCode.OK);
                    }
                }
            }

            await assertDatabase.AssertCntAsync(userRM, faccessPointRM, saccessPointRM);
            await assertDatabase.AssertStateAsync("access_control.AccessPoints", faccessPointId, new { Id = faccessPointId, IsActive = false });
            await assertDatabase.AssertStateAsync("access_control.AccessPoints", saccessPointId, new { Id = saccessPointId, IsActive = true });
        }

        [Fact]
        public async Task DeActivate_When_Does_Not_Exists()
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();
            var userRM = Examples.Administrator();
            var accessPointRM = Examples.AccessPoint();
            var accessPointId = 0;

            await RfidHttpClient.RegisterUserAsync(userRM);

            using (var authTokenResponse = await RfidHttpClient.GenerateAuthTokenAsync(userRM))
            {
                var authToken = await AuthTokenHelper.FromHttpResponseMessageAsync(authTokenResponse);
                var token = await authToken.GetTokenAsync();

                using (var registerAccessPointResponse = await RfidHttpClient.RegisterAccessPointAsync(accessPointRM, token))
                {
                    accessPointId = await RfidDatabase.GetAccessPointIdBySerialNumberAsync(accessPointRM.SerialNumber);

                    using (var activateAccessPointResponse = await RfidHttpClient.DeActivateAccessPointAsync("unknown", token))
                    {
                        RfidAssert.AssertHttpResponse(activateAccessPointResponse, System.Net.HttpStatusCode.NotFound);
                    }
                }
            }

            await assertDatabase.AssertCntAsync(userRM, accessPointRM);
            await assertDatabase.AssertStateAsync("access_control.AccessPoints", accessPointId, new { Id = accessPointId, IsActive = true });
        }

        [Fact]
        public async Task DeActivate_When_Not_Authorized()
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();
            var userRM = Examples.Administrator();
            var accessPointRM = Examples.AccessPoint();
            var accessPointId = 0;

            await RfidHttpClient.RegisterUserAsync(userRM);

            using (var authTokenResponse = await RfidHttpClient.GenerateAuthTokenAsync(userRM))
            {
                var authToken = await AuthTokenHelper.FromHttpResponseMessageAsync(authTokenResponse);
                var token = await authToken.GetTokenAsync();

                using (var registerAccessPointResponse = await RfidHttpClient.RegisterAccessPointAsync(accessPointRM, token))
                {
                    accessPointId = await RfidDatabase.GetAccessPointIdBySerialNumberAsync(accessPointRM.SerialNumber);

                    using (var activateAccessPointResponse = await RfidHttpClient.DeActivateAccessPointAsync(accessPointRM.SerialNumber, "unknown"))
                    {
                        RfidAssert.AssertHttpResponse(activateAccessPointResponse, System.Net.HttpStatusCode.Unauthorized);
                    }
                }
            }

            await assertDatabase.AssertCntAsync(userRM, accessPointRM);
            await assertDatabase.AssertStateAsync("access_control.AccessPoints", accessPointId, new { Id = accessPointId, IsActive = true });
        }

        #endregion De-activate


        #region De-activate

        [Fact]
        public async Task ChangeAccessLevel_With_Same_Access_Level()
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();
            var userRM = Examples.Administrator();
            var accessPointRM = Examples.AccessPoint("test", "test", AccessLevel.Mid);
            var accessPointId = 0;

            await RfidHttpClient.RegisterUserAsync(userRM);

            using (var authTokenResponse = await RfidHttpClient.GenerateAuthTokenAsync(userRM))
            {
                var authToken = await AuthTokenHelper.FromHttpResponseMessageAsync(authTokenResponse);
                var token = await authToken.GetTokenAsync();

                using (var registerAccessPointResponse = await RfidHttpClient.RegisterAccessPointAsync(accessPointRM, token))
                using (var activateAccessPointResponse = await RfidHttpClient.ChangeAccessPointAccessLevelAsync(accessPointRM.SerialNumber, AccessLevel.Mid, token))
                {
                    RfidAssert.AssertHttpResponse(activateAccessPointResponse, System.Net.HttpStatusCode.OK);

                    accessPointId = await RfidDatabase.GetAccessPointIdBySerialNumberAsync(accessPointRM.SerialNumber);
                }
            }

            await assertDatabase.AssertCntAsync(userRM, accessPointRM);
            await assertDatabase.AssertStateAsync("access_control.AccessPoints", accessPointId, new { Id = accessPointId, LevelId = (int)AccessLevel.Mid });
        }

        [Fact]
        public async Task ChangeAccessLevel_With_Diff_Access_Level()
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();
            var userRM = Examples.Administrator();
            var accessPointRM = Examples.AccessPoint("test", "test", AccessLevel.Mid);
            var accessPointId = 0;

            await RfidHttpClient.RegisterUserAsync(userRM);

            using (var authTokenResponse = await RfidHttpClient.GenerateAuthTokenAsync(userRM))
            {
                var authToken = await AuthTokenHelper.FromHttpResponseMessageAsync(authTokenResponse);
                var token = await authToken.GetTokenAsync();

                using (var registerAccessPointResponse = await RfidHttpClient.RegisterAccessPointAsync(accessPointRM, token))
                using (var activateAccessPointResponse = await RfidHttpClient.ChangeAccessPointAccessLevelAsync(accessPointRM.SerialNumber, AccessLevel.High, token))
                {
                    RfidAssert.AssertHttpResponse(activateAccessPointResponse, System.Net.HttpStatusCode.OK);

                    accessPointId = await RfidDatabase.GetAccessPointIdBySerialNumberAsync(accessPointRM.SerialNumber);
                }
            }

            await assertDatabase.AssertCntAsync(userRM, accessPointRM);
            await assertDatabase.AssertStateAsync("access_control.AccessPoints", accessPointId, new { Id = accessPointId, LevelId = (int)AccessLevel.High });
        }

        [Fact]
        public async Task ChangeAccessLevel_When_Several()
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();
            var userRM = Examples.Administrator();
            var faccessPointRM = Examples.AccessPoint("test1", "test1", AccessLevel.Low);
            var saccessPointRM = Examples.AccessPoint("test2", "test2", AccessLevel.High);

            var faccessPointId = 0;
            var saccessPointId = 0;

            await RfidHttpClient.RegisterUserAsync(userRM);

            using (var authTokenResponse = await RfidHttpClient.GenerateAuthTokenAsync(userRM))
            {
                var authToken = await AuthTokenHelper.FromHttpResponseMessageAsync(authTokenResponse);
                var token = await authToken.GetTokenAsync();

                using (var fregisterAccessPointResponse = await RfidHttpClient.RegisterAccessPointAsync(faccessPointRM, token))
                using (var sregisterAccessPointResponse = await RfidHttpClient.RegisterAccessPointAsync(saccessPointRM, token))
                {
                    faccessPointId = await RfidDatabase.GetAccessPointIdBySerialNumberAsync(faccessPointRM.SerialNumber);
                    saccessPointId = await RfidDatabase.GetAccessPointIdBySerialNumberAsync(saccessPointRM.SerialNumber);

                    using (var factivateAccessPointResponse = await RfidHttpClient.ChangeAccessPointAccessLevelAsync(faccessPointRM.SerialNumber, AccessLevel.Mid, token))
                    using (var sactivateAccessPointResponse = await RfidHttpClient.ChangeAccessPointAccessLevelAsync(saccessPointRM.SerialNumber, AccessLevel.Low, token))
                    {
                        RfidAssert.AssertHttpResponse(factivateAccessPointResponse, System.Net.HttpStatusCode.OK);
                        RfidAssert.AssertHttpResponse(sactivateAccessPointResponse, System.Net.HttpStatusCode.OK);
                    }
                }
            }

            await assertDatabase.AssertCntAsync(userRM, faccessPointRM, saccessPointRM);
            await assertDatabase.AssertStateAsync("access_control.AccessPoints", faccessPointId, new { Id = faccessPointId, LevelId = (int)AccessLevel.Mid });
            await assertDatabase.AssertStateAsync("access_control.AccessPoints", saccessPointId, new { Id = saccessPointId, LevelId = (int)AccessLevel.Low });
        }

        [Fact]
        public async Task ChangeAccessLevel_When_Does_Not_Exists()
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();
            var userRM = Examples.Administrator();
            var accessPointRM = Examples.AccessPoint("test", "test", AccessLevel.Mid);
            var accessPointId = 0;

            await RfidHttpClient.RegisterUserAsync(userRM);

            using (var authTokenResponse = await RfidHttpClient.GenerateAuthTokenAsync(userRM))
            {
                var authToken = await AuthTokenHelper.FromHttpResponseMessageAsync(authTokenResponse);
                var token = await authToken.GetTokenAsync();

                using (var registerAccessPointResponse = await RfidHttpClient.RegisterAccessPointAsync(accessPointRM, token))
                {
                    accessPointId = await RfidDatabase.GetAccessPointIdBySerialNumberAsync(accessPointRM.SerialNumber);

                    using (var activateAccessPointResponse = await RfidHttpClient.ChangeAccessPointAccessLevelAsync("unknown", AccessLevel.Low, token))
                    {
                        RfidAssert.AssertHttpResponse(activateAccessPointResponse, System.Net.HttpStatusCode.NotFound);
                    }
                }
            }

            await assertDatabase.AssertCntAsync(userRM, accessPointRM);
            await assertDatabase.AssertStateAsync("access_control.AccessPoints", accessPointId, new { Id = accessPointId, LevelId = (int)AccessLevel.Mid });
        }

        [Fact]
        public async Task ChangeAccessLevel_When_Not_Authorized()
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();
            var userRM = Examples.Administrator();
            var accessPointRM = Examples.AccessPoint("test", "test", AccessLevel.Low);
            var accessPointId = 0;

            await RfidHttpClient.RegisterUserAsync(userRM);

            using (var authTokenResponse = await RfidHttpClient.GenerateAuthTokenAsync(userRM))
            {
                var authToken = await AuthTokenHelper.FromHttpResponseMessageAsync(authTokenResponse);
                var token = await authToken.GetTokenAsync();

                using (var registerAccessPointResponse = await RfidHttpClient.RegisterAccessPointAsync(accessPointRM, token))
                {
                    accessPointId = await RfidDatabase.GetAccessPointIdBySerialNumberAsync(accessPointRM.SerialNumber);

                    using (var activateAccessPointResponse = await RfidHttpClient.ChangeAccessPointAccessLevelAsync(accessPointRM.SerialNumber, AccessLevel.High, "unknown"))
                    {
                        RfidAssert.AssertHttpResponse(activateAccessPointResponse, System.Net.HttpStatusCode.Unauthorized);
                    }
                }
            }

            await assertDatabase.AssertCntAsync(userRM, accessPointRM);
            await assertDatabase.AssertStateAsync("access_control.AccessPoints", accessPointId, new { Id = accessPointId, LevelId = (int)AccessLevel.Low });
        }

        #endregion De-activate
    }
}

namespace RFID.REST.Test.AccessControl
{
    using RFID.REST.Models;
    using RFID.REST.Test.Common;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    [Collection("Integration")]
    public class TagsControllerTests
    {
        public TagsControllerTests()
        {
            RfidDockerHttpClient.RestMssqlAsync().Wait();
        }

        [Fact]
        public async Task Check_Access_Level_Sufficient_1()
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();
            var userRM = Examples.Administrator();
            var tagRM = Examples.Tag("test", (int)AccessLevel.Mid, "test");
            var accessPoint = Examples.AccessPoint("test", "test", AccessLevel.Mid);

            await RfidHttpClient.RegisterUserAsync(userRM);
            using (var tokenResponse = await RfidHttpClient.GenerateAuthTokenAsync(userRM))
            {
                var authToken = await AuthTokenHelper.FromHttpResponseMessageAsync(tokenResponse);
                var token = await authToken.GetTokenAsync();

                using (var registerTagResponse = await RfidHttpClient.RegisterTagAsync(tagRM, token))
                using (var registerAccessPointResponse = await RfidHttpClient.RegisterAccessPointAsync(accessPoint, token))
                using (var checkAccessResponse = await RfidHttpClient.CheckAccessAsync(tagRM.Number, accessPoint.SerialNumber))
                {
                    RfidAssert.AssertHttpResponse(checkAccessResponse, System.Net.HttpStatusCode.OK);
                }
            }

            await assertDatabase.AssertCntAsync(userRM, tagRM, accessPoint);
        }

        [Fact]
        public async Task Check_Access_Level_Sufficient_2()
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();
            var userRM = Examples.Administrator();
            var tagRM = Examples.Tag("test", (int)AccessLevel.High, "test");
            var accessPoint = Examples.AccessPoint("test", "test", AccessLevel.Mid);

            await RfidHttpClient.RegisterUserAsync(userRM);
            using (var tokenResponse = await RfidHttpClient.GenerateAuthTokenAsync(userRM))
            {
                var authToken = await AuthTokenHelper.FromHttpResponseMessageAsync(tokenResponse);
                var token = await authToken.GetTokenAsync();

                using (var registerTagResponse = await RfidHttpClient.RegisterTagAsync(tagRM, token))
                using (var registerAccessPointResponse = await RfidHttpClient.RegisterAccessPointAsync(accessPoint, token))
                using (var checkAccessResponse = await RfidHttpClient.CheckAccessAsync(tagRM.Number, accessPoint.SerialNumber))
                {
                    RfidAssert.AssertHttpResponse(checkAccessResponse, System.Net.HttpStatusCode.OK);
                }
            }

            await assertDatabase.AssertCntAsync(userRM, tagRM, accessPoint);
        }

        [Fact]
        public async Task Check_Access_Level_Sufficient_3()
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();
            var userRM = Examples.Administrator();
            var tagRM = Examples.Tag("test", (int)AccessLevel.Mid, "test");
            var accessPoint = Examples.AccessPoint("test", "test", AccessLevel.Low);

            await RfidHttpClient.RegisterUserAsync(userRM);
            using (var tokenResponse = await RfidHttpClient.GenerateAuthTokenAsync(userRM))
            {
                var authToken = await AuthTokenHelper.FromHttpResponseMessageAsync(tokenResponse);
                var token = await authToken.GetTokenAsync();

                using (var registerTagResponse = await RfidHttpClient.RegisterTagAsync(tagRM, token))
                using (var registerAccessPointResponse = await RfidHttpClient.RegisterAccessPointAsync(accessPoint, token))
                using (var checkAccessResponse = await RfidHttpClient.CheckAccessAsync(tagRM.Number, accessPoint.SerialNumber))
                {
                    RfidAssert.AssertHttpResponse(checkAccessResponse, System.Net.HttpStatusCode.OK);
                }
            }

            await assertDatabase.AssertCntAsync(userRM, tagRM, accessPoint);
        }

        [Fact]
        public async Task Check_Access_Level_When_Not_Sufficient()
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();
            var userRM = Examples.Administrator();
            var tagRM = Examples.Tag("test", (int)AccessLevel.Low, "test");
            var accessPoint = Examples.AccessPoint("test", "test", AccessLevel.Mid);

            await RfidHttpClient.RegisterUserAsync(userRM);
            using (var tokenResponse = await RfidHttpClient.GenerateAuthTokenAsync(userRM))
            {
                var authToken = await AuthTokenHelper.FromHttpResponseMessageAsync(tokenResponse);
                var token = await authToken.GetTokenAsync();

                using (var registerTagResponse = await RfidHttpClient.RegisterTagAsync(tagRM, token))
                using (var registerAccessPointResponse = await RfidHttpClient.RegisterAccessPointAsync(accessPoint, token))
                using (var checkAccessResponse = await RfidHttpClient.CheckAccessAsync(tagRM.Number, accessPoint.SerialNumber))
                {
                    RfidAssert.AssertHttpResponse(checkAccessResponse, System.Net.HttpStatusCode.Unauthorized);
                }
            }

            await assertDatabase.AssertCntAsync(userRM, tagRM, accessPoint);
        }

        [Fact]
        public async Task Check_Access_Level_When_Tag_Number_Does_Not_Exists()
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();
            var userRM = Examples.Administrator();
            var tagRM = Examples.Tag("test", (int)AccessLevel.Mid, "test");
            var accessPoint = Examples.AccessPoint("test", "test", AccessLevel.Mid);

            await RfidHttpClient.RegisterUserAsync(userRM);
            using (var tokenResponse = await RfidHttpClient.GenerateAuthTokenAsync(userRM))
            {
                var authToken = await AuthTokenHelper.FromHttpResponseMessageAsync(tokenResponse);
                var token = await authToken.GetTokenAsync();

                using (var registerTagResponse = await RfidHttpClient.RegisterTagAsync(tagRM, token))
                using (var registerAccessPointResponse = await RfidHttpClient.RegisterAccessPointAsync(accessPoint, token))
                using (var checkAccessResponse = await RfidHttpClient.CheckAccessAsync("unknown", accessPoint.SerialNumber))
                {
                    RfidAssert.AssertHttpResponse(checkAccessResponse, System.Net.HttpStatusCode.NotFound);
                }
            }

            await assertDatabase.AssertCntAsync(userRM, tagRM, accessPoint, Examples.UnKnownTag("unknown"));
        }

        [Fact]
        public async Task Check_Access_Level_When_Access_Point_Serial_Number_Does_Not_Exists()
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();
            var userRM = Examples.Administrator();
            var tagRM = Examples.Tag("test", (int)AccessLevel.Mid, "test");
            var accessPoint = Examples.AccessPoint("test", "test", AccessLevel.Mid);

            await RfidHttpClient.RegisterUserAsync(userRM);
            using (var tokenResponse = await RfidHttpClient.GenerateAuthTokenAsync(userRM))
            {
                var authToken = await AuthTokenHelper.FromHttpResponseMessageAsync(tokenResponse);
                var token = await authToken.GetTokenAsync();

                using (var registerTagResponse = await RfidHttpClient.RegisterTagAsync(tagRM, token))
                using (var registerAccessPointResponse = await RfidHttpClient.RegisterAccessPointAsync(accessPoint, token))
                using (var checkAccessResponse = await RfidHttpClient.CheckAccessAsync(tagRM.Number, "unknown"))
                {
                    RfidAssert.AssertHttpResponse(checkAccessResponse, System.Net.HttpStatusCode.NotFound);
                }
            }

            await assertDatabase.AssertCntAsync(userRM, tagRM, accessPoint, Examples.UnKnownAccessPoint("unknown"));
        }

        [Theory]
        [InlineData("", "")]
        [InlineData("", null)]
        [InlineData(null, "")]
        [InlineData(null, null)]
        public async Task Check_Access_Level_When_Invalid_Data(String tagNumber, String accessPointSerialNumber)
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();
            
            using (var checkAccessResponse = await RfidHttpClient.CheckAccessAsync(tagNumber, accessPointSerialNumber))
            {
                RfidAssert.AssertHttpResponse(checkAccessResponse, System.Net.HttpStatusCode.BadRequest);
            }

            await assertDatabase.AssertCntAsync();
        }
    }
}

namespace RFID.REST.Test.Auth
{
    using Microsoft.IdentityModel.Tokens;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using RFID.REST.Areas.Administration.Models;
    using RFID.REST.Test.Common;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    using static Common.RfidAssert;

    [Collection("Integration")]
    public class TokenControllerTests
    {
        public TokenControllerTests()
        {
            RfidDockerHttpClient.RestMssqlAsync().Wait();
        }

        [Fact]
        public async Task GenerateToken_When_User_Exists()
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();
            var registerUserRM = Examples.Administrator();

            await RfidHttpClient.RegisterUserAsync(registerUserRM);
            using (var httpResponse = await RfidHttpClient.GenerateAuthTokenAsync(registerUserRM.Email, registerUserRM.Password))
            {
                await AssertAuthTokenResponseAsync(httpResponse, System.Net.HttpStatusCode.OK);
            }

            await assertDatabase.AssertCntAsync(registerUserRM);
        }

        [Fact]
        public async Task GenerateToken_When_Several_Users_Exists()
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();
            var fregisterUserRM = Examples.Administrator();
            var sregisterUserRM = Examples.Administrator("test@test2.com", "123");

            await RfidHttpClient.RegisterUserAsync(fregisterUserRM);
            await RfidHttpClient.RegisterUserAsync(sregisterUserRM);

            using (var fhttpResponse = await RfidHttpClient.GenerateAuthTokenAsync(fregisterUserRM.Email, fregisterUserRM.Password))
            using (var shttpResponse = await RfidHttpClient.GenerateAuthTokenAsync(sregisterUserRM.Email, sregisterUserRM.Password))
            {
                await AssertAuthTokenResponseAsync(fhttpResponse, System.Net.HttpStatusCode.OK);
                await AssertAuthTokenResponseAsync(shttpResponse, System.Net.HttpStatusCode.OK);
            }

            await assertDatabase.AssertCntAsync(fregisterUserRM, sregisterUserRM);
        }

        [Fact]
        public async Task GenerateToken_When_User_Does_Not_Exists()
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();

            using (var httpResponse = await RfidHttpClient.GenerateAuthTokenAsync("unknown@unknown.com", "123"))
            {
                await AssertAuthTokenResponseAsync(httpResponse, System.Net.HttpStatusCode.NotFound);
            }

            await assertDatabase.AssertCntAsync();
        }

        [Fact]
        public async Task GenerateToken_When_User_Exists_But_Password_Incorect()
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();
            var requestModel = Examples.Administrator();

            await RfidHttpClient.RegisterUserAsync(requestModel);
            using (var httpResponse = await RfidHttpClient.GenerateAuthTokenAsync(requestModel.Email, Path.GetRandomFileName()))
            {
                await AssertAuthTokenResponseAsync(httpResponse, System.Net.HttpStatusCode.NotFound);
            }

            await assertDatabase.AssertCntAsync(requestModel);
        }

        [Fact]
        public async Task RefereshToken_When_Refresh_Token_Is_Valid()
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();
            var requestModel = Examples.Administrator();

            await RfidHttpClient.RegisterUserAsync(requestModel);

            using (var genResponse = await RfidHttpClient.GenerateAuthTokenAsync(requestModel.Email, requestModel.Password))
            {
                var jtoken = await genResponse.Content.AsJObjectAsync();

                using (var refreshResponse = await RfidHttpClient.RefreshAuthTokenAsync(jtoken.Token(), jtoken.RefreshToken()))
                {
                    await AssertAuthTokenResponseAsync(refreshResponse, System.Net.HttpStatusCode.OK);

                    var rtoken = await refreshResponse.Content.AsJObjectAsync();
                    Assert.NotEqual(expected: jtoken.Token(), actual: rtoken.Token());
                    Assert.NotEqual(expected: jtoken.RefreshToken(), actual: rtoken.RefreshToken());
                }
            }

            await assertDatabase.AssertCntAsync(requestModel);
        }

        [Fact]
        public async Task RefreshToken_When_User_Refresh_Token_Is_Invalid()
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();
            var requestModel = Examples.Administrator();

            await RfidHttpClient.RegisterUserAsync(requestModel);

            using (var genResponse = await RfidHttpClient.GenerateAuthTokenAsync(requestModel.Email, requestModel.Password))
            {
                var jtoken = await genResponse.Content.AsJObjectAsync();

                using (var refreshResponse = await RfidHttpClient.RefreshAuthTokenAsync(jtoken.Token(), Path.GetRandomFileName()))
                {
                    await AssertAuthTokenResponseAsync(refreshResponse, System.Net.HttpStatusCode.NotFound);
                }
            }

            await assertDatabase.AssertCntAsync(requestModel);
        }

        [Fact]
        public async Task RefreshToken_When_Auth_Token_Generated_With_Different_Alg()
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();
            var requestModel = Examples.Administrator();

            await RfidHttpClient.RegisterUserAsync(requestModel);

            var auth = new RFID.REST.Areas.Auth.Services.Auth(null, null, Settings.GetDevelopmentAuthSettings());
            var fakeToken = auth.GenerateToken(requestModel.Email, UserRoles.Admin, SecurityAlgorithms.HmacSha384);

            using (var httpResponse = await RfidHttpClient.RefreshAuthTokenAsync(fakeToken.Token, fakeToken.RefreshToken))
            {
                await AssertAuthTokenResponseAsync(httpResponse, System.Net.HttpStatusCode.NotFound);
            }

            await assertDatabase.AssertCntAsync(requestModel);
        }

        [Fact]
        public async Task RefreshToken_When_Auth_Token_Generated_With_Correct_Alg_But_User_Does_Not_Exists()
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();

            var auth = new RFID.REST.Areas.Auth.Services.Auth(null, null, Settings.GetDevelopmentAuthSettings());
            var fakeToken = auth.GenerateToken("unknown@unknown.com", UserRoles.Admin);

            using (var httpResponse = await RfidHttpClient.RefreshAuthTokenAsync(fakeToken.Token, fakeToken.RefreshToken))
            {
                await AssertAuthTokenResponseAsync(httpResponse, System.Net.HttpStatusCode.NotFound);
            }

            await assertDatabase.AssertCntAsync();
        }

        [Theory]
        [InlineData("", "")]
        [InlineData(null, null)]
        [InlineData("", null)]
        [InlineData(null, "")]
        public async Task RefreshToken_When_Invalid_Data(String token, String refreshToken)
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();

            using (var httpResponse = await RfidHttpClient.RefreshAuthTokenAsync(token, refreshToken))
            {
                await AssertAuthTokenResponseAsync(httpResponse, System.Net.HttpStatusCode.BadRequest);
            }

            await assertDatabase.AssertCntAsync();
        }
    }
}

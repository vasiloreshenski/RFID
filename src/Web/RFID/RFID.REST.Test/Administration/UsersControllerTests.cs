namespace RFID.REST.Test.Administration
{
    using Microsoft.AspNetCore.Http;
    using RFID.REST.Areas.Administration.Models;
    using RFID.REST.Models;
    using RFID.REST.Test.Common;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    using static Common.RfidAssert;

    [Collection("Integration")]
    public class UsersControllerTests
    {
        private static readonly String areaSubPath = "/administration/api/users";
        private static readonly String registerUserSubPath = $"{areaSubPath}/register";

        public UsersControllerTests()
        {
            RfidDockerHttpClient.RestMssqlAsync().Wait();
        }

        [Fact]
        public async Task Register_User()
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();
            var requestModel = Examples.Administrator();

            using (var response = await RfidHttpClient.PostAsync(registerUserSubPath, requestModel))
            {
                AssertHttpResponse(response, HttpStatusCode.OK);
            }

            await assertDatabase.AssertCntAsync(requestModel);
        }

        [Fact]
        public async Task Register_Several_Users()
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();
            var fRequestModel = Examples.Administrator();
            var sRequestModel = Examples.Administrator("test@test2.com", "123");

            using (var response = await RfidHttpClient.PostAsync(registerUserSubPath, fRequestModel))
            {
                AssertHttpResponse(response, HttpStatusCode.OK);
            }

            using (var response = await RfidHttpClient.PostAsync(registerUserSubPath, sRequestModel))
            {
                AssertHttpResponse(response, HttpStatusCode.OK);
            }

            await assertDatabase.AssertCntAsync(fRequestModel, sRequestModel);
        }

        [Fact]
        public async Task Register_User_When_Already_Registered()
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();
            var requestModel = Examples.Administrator();

            using (var response = await RfidHttpClient.PostAsync(registerUserSubPath, requestModel))
            {
                AssertHttpResponse(response, HttpStatusCode.OK);
            }

            using (var response = await RfidHttpClient.PostAsync(registerUserSubPath, requestModel))
            {
               await AssertHttpResponseAsync(response, HttpStatusCode.BadRequest, (false, CommandStatus.Dublicate));
            }

            await assertDatabase.AssertCntAsync(requestModel);
        }

        [Theory]
        [InlineData("", "password", 1)]
        [InlineData(null, null, 1)]
        [InlineData("email@email.com", "", 1)]
        [InlineData("email", "password", 1)]
        [InlineData("email@email2.com", "password", -1)]
        [InlineData("email@email3.com", "", 1)]
        [InlineData("email@email3.com", null, 1)]
        public async Task Register_User_When_Invalid(String email, String password, int role)
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();

            using (var response = await RfidHttpClient.PostAsync(registerUserSubPath, CreateUser(email, password, (UserRoles)role)))
            {
                AssertHttpResponse(response, HttpStatusCode.BadRequest);
            }

            await assertDatabase.AssertCntAsync();
        }

        private static RegisterAdministrationUserRequestModel CreateUser(String email, String password, UserRoles roles)
        {
            return new RegisterAdministrationUserRequestModel { Email = email, Password = password, Roles = roles };
        }
    }
}

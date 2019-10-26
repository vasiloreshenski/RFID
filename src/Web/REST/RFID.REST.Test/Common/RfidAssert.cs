namespace RFID.REST.Test.Common
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using RFID.REST.Areas.Administration.Models;
    using RFID.REST.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    internal static class RfidAssert
    {
        public static void AssertHttpResponse(HttpResponseMessage httpResponse, HttpStatusCode statusCode)
        {
            Assert.Equal(actual: httpResponse.StatusCode, expected: statusCode);
        }

        public static async Task AssertHttpResponseAsync(HttpResponseMessage httpResponse, HttpStatusCode statusCode, (bool success, CommandStatus status) commandInfo)
        {
            var json = (JObject)JsonConvert.DeserializeObject(await httpResponse.Content.ReadAsStringAsync());

            Assert.Equal(actual: httpResponse.StatusCode, expected: statusCode);

            var actual = (json["success"].Value<bool>(), (CommandStatus)json["status"].Value<int>());
            Assert.Equal(expected: commandInfo, actual: actual);
        }

        public static async Task AssertAuthTokenResponseAsync(HttpResponseMessage httpResponse, HttpStatusCode expectedStatusCode)
        {
            AssertHttpResponse(httpResponse, expectedStatusCode);

            if (expectedStatusCode == HttpStatusCode.OK)
            {
                var jobject = await httpResponse.Content.AsJObjectAsync();

                Assert.NotNull(jobject.Token());
                Assert.True(jobject.ExpireInSeconds() > 0);
                Assert.NotNull(jobject.RefreshToken());
            }
        }

        public static async Task AssertRegisterTagAsync(
            Func<Task<HttpResponseMessage>> httpResponseFunc, 
            HttpStatusCode expectedStatusCode, 
            RegisterTagRequestModel requestModel,
            int expectedTagsCntIncrease,
            int expectedUserCntIncrease)
        {
            var tagsCntBefore = await RfidDatabase.GetAccessControlTagsCountAsync();
            var tagsUsersCntBefore = await RfidDatabase.GetAccessControlUsersCountAsync();

            using (var httpResponse = await httpResponseFunc())
            {
                var tagsCntAfter = await RfidDatabase.GetAccessControlTagsCountAsync();
                var tagsUsersCntAfter = await RfidDatabase.GetAccessControlUsersCountAsync();

                AssertHttpResponse(httpResponse, expectedStatusCode);

                if (expectedStatusCode == HttpStatusCode.OK)
                {
                    Assert.Equal(expected: tagsCntBefore + expectedTagsCntIncrease, actual: tagsCntAfter);
                    Assert.Equal(expected: tagsUsersCntBefore + expectedUserCntIncrease, actual: tagsUsersCntAfter);
                }
                else
                {
                    Assert.Equal(expected: tagsCntBefore, actual: tagsCntAfter);
                    Assert.Equal(expected: tagsUsersCntBefore, actual: tagsUsersCntAfter);
                }
            }
        }
    }
}

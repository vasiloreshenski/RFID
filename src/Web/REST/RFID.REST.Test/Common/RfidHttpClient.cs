namespace RFID.REST.Test.Common
{
    using Newtonsoft.Json;
    using RFID.REST.Areas.Administration.Models;
    using RFID.REST.Areas.Auth.Models;
    using RFID.REST.Common;
    using RFID.REST.Models;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    internal static class RfidHttpClient
    {
        public static readonly String EndPointUrl = "http://192.168.0.105:8080";

        public static Task<HttpResponseMessage> GetAsync(String subpath, String authToken)
        {
            return ExecuteAsync<Object>(subpath, null, authToken, (client, path, _) => client.GetAsync(path)); 
        }

        public static Task<HttpResponseMessage> PostAsync<T>(String subpath, T data)
        {
            return PostAsync(subpath, data, null);
        }

        public static Task<HttpResponseMessage> PostAsync<T>(String subpath, T data, String authToken)
        {
            return ExecuteAsync(subpath, data, authToken, (x, path, content) => x.PostAsync(path, content));
        }

        public static Task<HttpResponseMessage> PatchAsync<T>(String subpath, T data, String authToken)
        {
            return ExecuteAsync(subpath, data, authToken, (x, path, content) => x.PatchAsync(path, content));
        }

        public static async Task RegisterUserAsync(RegisterAdministrationUserRequestModel requestModel)
        {
            using (await RfidHttpClient.PostAsync("/administration/api/users/register", requestModel))
            {
            }
        }

        public static Task<HttpResponseMessage> GenerateAuthTokenAsync(RegisterAdministrationUserRequestModel requestModel)
        {
            return GenerateAuthTokenAsync(requestModel.Email, requestModel.Password);
        }

        public static async Task<HttpResponseMessage> GenerateAuthTokenAsync(String email, String password)
        {
            var result = await PostAsync("/auth/api/token/generate", new TokenGenerationRequestModel { Email = email, Password = password });
            
            return result;

        }

        public static Task<HttpResponseMessage> RefreshAuthTokenAsync(String token, String refreshToken)
        {
            return PostAsync("/auth/api/token/refresh", new TokenRefreshRequestModel { Token = token, RefreshToken = refreshToken });
        }

        public static Task<HttpResponseMessage> RegisterTagAsync(RegisterTagRequestModel requestModel, String authToken)
        {
            return PostAsync("/administration/api/tags/register", requestModel, authToken);
        }

        public static async Task<HttpResponseMessage> ActivateTagAsync(String number, String authToken)
        {
            var tagId = await RfidDatabase.GetTagIdByNumberAsync(number);

            return await PatchAsync($"/administration/api/tags/activate", new { Id = tagId }, authToken);
        }

        public static async Task<HttpResponseMessage> DeActivateTagAsync(String number, String authToken)
        {
            var tagId = await RfidDatabase.GetTagIdByNumberAsync(number);

            return await PatchAsync($"/administration/api/tags/deactivate", new { Id = tagId }, authToken);
        }

        public static async Task<HttpResponseMessage> DeleteTagAsync(String number, String authToken)
        {
            var tagId = await RfidDatabase.GetTagIdByNumberAsync(number);

            return await PatchAsync($"/administration/api/tags/delete", new { Id = tagId }, authToken);
        }

        public static async Task<HttpResponseMessage> UpdateTagAsync(UpdateTagRequestModel model, String authToken)
        {
            return await PatchAsync($"/administration/api/tags/update", model, authToken);
        }

        public static async Task<HttpResponseMessage> ChangeTagAccessLevelAsync(ChangeTagAccessLevelRequestModel requestModel, String authToken)
        {
            return await PatchAsync($"/administration/api/tags/accesslevel", requestModel, authToken);
        }

        public static Task<HttpResponseMessage> RegisterAccessPointAsync(RegisterAccessPointRequestModel requestModel, String authToken)
        {
            return PostAsync("/administration/api/accessPoint/register", requestModel, authToken);
        }

        public static async Task<HttpResponseMessage> ActivateAccessPointAsync(String serialNumber, String authToken)
        {
            var id = await RfidDatabase.GetAccessPointIdBySerialNumberAsync(serialNumber);

            return await PatchAsync("/administration/api/accessPoint/activate", new { Id = id }, authToken);
        }

        public static async Task<HttpResponseMessage> DeActivateAccessPointAsync(String serialNumber, String authToken)
        {
            var id = await RfidDatabase.GetAccessPointIdBySerialNumberAsync(serialNumber);

            return await PatchAsync("/administration/api/accessPoint/deactivate", new { Id = id }, authToken);
        }

        public static async Task<HttpResponseMessage> ChangeAccessPointAccessLevelAsync(String serialNumber, AccessLevel accessLevel, String authToken)
        {
            var id = await RfidDatabase.GetAccessPointIdBySerialNumberAsync(serialNumber);

            return await PatchAsync("/administration/api/accessPoint/accesslevel", new ChangeAccessPointAccessLevelRequestModel { AccessPointId = id, AccessLevel = accessLevel }, authToken);
        }

        public static Task<HttpResponseMessage> CheckAccessAsync(String tagNumber, String accessPontSerialNumber)
        {
            return GetAsync($"/accesscontrol/api/tags/checkaccess?TagNumber={tagNumber}&AccessPointSerialNumber={accessPontSerialNumber}", null);
        }

        public static Task<HttpResponseMessage> UpdateAccessPointAsync(UpdateAccessPointRequestModel requestModel, String authToken)
        {
            return PatchAsync($"/administration/api/accessPoint/update", requestModel, authToken);
        }

        public static async Task<HttpResponseMessage> DeleteAccessPointAsync(String serialNumber, String authToken)
        {
            var id = await RfidDatabase.GetAccessPointIdBySerialNumberAsync(serialNumber);

            return await PatchAsync($"/administration/api/accesspoint/delete", new { Id = id }, authToken);
        }

        public static async Task<IReadOnlyCollection<AccessPointResponseModel>> GetAllActiveAccessPointsAsync(String authToken)
        {
            using (var response = await GetAsync("/administration/api/accesspoint/active", authToken))
            {
                using (var content = response.Content)
                {
                    var str = await content.ReadAsStringAsync();

                    return JsonConvert.DeserializeObject<List<AccessPointResponseModel>>(str);
                }
            }
        }

        private static async Task<HttpResponseMessage> ExecuteAsync<T>(String subpath, T data, String authToken, Func<HttpClient, String, HttpContent, Task<HttpResponseMessage>> action)
        {
            var httpContent = (HttpContent)null;

            try
            {
                using (var httpClient = new HttpClient())
                {
                    if (String.IsNullOrEmpty(authToken) == false)
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);
                    }

                    if (data?.Equals(default(T)) == false)
                    {
                        httpContent = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                    }

                    var path = EndPointUrl;
                    if (subpath.StartsWith("/") == false)
                    {
                        subpath = $"/{subpath}";
                    }

                    path = $"{path}{subpath}";

                    return await action(httpClient, path, httpContent);
                }
            }
            finally
            {
                httpContent?.Dispose();
            }
        }
    }
}

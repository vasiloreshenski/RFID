namespace RFID.REST.Test.Common
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    public static class Extensions
    {
        public static async Task<JObject> AsJObjectAsync(this HttpContent content)
        {
            var str = await content.ReadAsStringAsync();

            return (JObject)JsonConvert.DeserializeObject(str);
        }

        public static String Token(this JObject jObject) => jObject["token"].Value<string>();

        public static int ExpireInSeconds(this JObject jObject) => jObject["expireInSeconds"].Value<int>();

        public static String RefreshToken(this JObject jObject) => jObject["refreshToken"].Value<string>();
    }
}

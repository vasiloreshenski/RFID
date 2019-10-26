namespace RFID.REST.Test.Common
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    internal class AuthTokenHelper
    {
        public static async Task<AuthTokenHelper> FromHttpResponseMessageAsync(HttpResponseMessage httpResponse)
        {
            var jobj = await httpResponse.Content.AsJObjectAsync();
            return new AuthTokenHelper(jobj.Token(), jobj.RefreshToken());
        }

        private readonly String token;
        private readonly String refreshToken;

        public AuthTokenHelper(string token, string refreshToken)
        {
            this.token = token;
            this.refreshToken = refreshToken;
        }

        public Task<String> GetTokenAsync()
        {
            // later here we can implement referesh logic

            return Task.FromResult(this.token);
        }
    }
}

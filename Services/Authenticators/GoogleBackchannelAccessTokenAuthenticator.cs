using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Text.Json;
using AspNet.Security.OAuth.KakaoTalk;

namespace AuthenticationServer.API.Services.Authenticators
{
    public class GoogleBackchannelAccessTokenAuthenticator
    {
        private readonly HttpClient Backchannel;
        private readonly HttpContext Context;

        public GoogleBackchannelAccessTokenAuthenticator(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            this.Backchannel = httpClientFactory.CreateClient("google");
            this.Context = httpContextAccessor.HttpContext;
        }

        public async Task<JsonDocument> GetUserProfileAsync(
        [NotNull] string accessToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"?id_token={accessToken}");

            using var response = await Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, Context.RequestAborted);
            if (!response.IsSuccessStatusCode)
            {
                //await Log.UserProfileErrorAsync(Logger, response, Context.RequestAborted);
                throw new HttpRequestException("An error occurred while retrieving the user profile.");
            }

            return JsonDocument.Parse(await response.Content.ReadAsStringAsync(Context.RequestAborted));
        }
    }
}

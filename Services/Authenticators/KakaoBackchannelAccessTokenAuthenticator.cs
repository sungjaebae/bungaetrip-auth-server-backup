using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Text.Json;
using AspNet.Security.OAuth.KakaoTalk;

namespace AuthenticationServer.API.Services.Authenticators
{
    public class KakaoBackchannelAccessTokenAuthenticator
    {
        private readonly HttpClient Backchannel;
        private readonly HttpContext Context;

        public KakaoBackchannelAccessTokenAuthenticator(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            this.Backchannel = httpClientFactory.CreateClient("kakao");
            this.Context = httpContextAccessor.HttpContext;
        }

        public string UserInformationEndpoint { get; set; }

        public async Task<JsonDocument> GetUserProfileAsync(
        [NotNull] string accessToken)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, UserInformationEndpoint);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

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

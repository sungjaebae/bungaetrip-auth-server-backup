using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata;
using System.Text.Json;

namespace AuthenticationServer.API.Services.Authenticators
{
    public class AppleBackchannelAccessTokenAuthenticator
    { 
        private readonly HttpClient Backchannel;
        private readonly HttpContext Context;

        public AppleBackchannelAccessTokenAuthenticator(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            this.Backchannel = httpClientFactory.CreateClient("apple");
            this.Context = httpContextAccessor.HttpContext;
        }
        
        public async Task<string> GetPublicKeyAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, (string)null);
            using var response = await Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, Context.RequestAborted);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException("An error occurred while retrieving the public key.");
            }
            return await response.Content.ReadAsStringAsync();
        }
    }       
}

using System.Diagnostics.CodeAnalysis;
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

    public async Task<JsonDocument> GetUserProfileAsync(
    [NotNull] string accessToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, (string)null);

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

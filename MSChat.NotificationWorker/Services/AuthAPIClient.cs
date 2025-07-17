using Microsoft.Extensions.Options;
using MSChat.NotificationWorker.Configuration;
using MSChat.Shared.Auth.Models.Dtos;

namespace MSChat.NotificationWorker.Services;

public class AuthAPIClient : IAuthAPIClient
{
    private readonly IAccessTokenProvider _tokenProvider;
    private readonly IHttpClientFactory _clientFactory;
    private readonly ServicesSettings _servicesSettings;

    public AuthAPIClient(IAccessTokenProvider tokenProvider, IHttpClientFactory clientFactory, IOptions<ServicesSettings> servicesSettings)
    {
        _tokenProvider = tokenProvider;
        _clientFactory = clientFactory;
        _servicesSettings = servicesSettings.Value;
    }

    public async Task<UserInfoDto?> GetUserInfoAsync(string userId, CancellationToken cancellationToken = default)
    {
        using var client = _clientFactory.CreateClient();

        var accessToken = await _tokenProvider.GetAccessTokenAsync(cancellationToken);

        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

        var url = $"{_servicesSettings.AuthApiUri}/api/users/{userId}/info";

        try
        {
            return await client.GetFromJsonAsync<UserInfoDto>(url);
        }
        catch
        {
            return null;
        }
    }
}

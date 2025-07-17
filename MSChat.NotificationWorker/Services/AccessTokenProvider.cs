using Duende.IdentityModel.Client;
using Microsoft.Extensions.Options;
using MSChat.Shared.Configuration.Models;

namespace MSChat.NotificationWorker.Services;

public class AccessTokenProvider : IAccessTokenProvider
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly M2MAuthSettings _m2mAuthSettings;

    private string _accessToken = "";
    private DateTime _expiresAt = default;

    public AccessTokenProvider(IHttpClientFactory clientFactory, IOptions<M2MAuthSettings> m2mAuthSettings)
    {
        _clientFactory = clientFactory;
        _m2mAuthSettings = m2mAuthSettings.Value;
    }

    public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        if (_expiresAt > DateTime.Now && !string.IsNullOrEmpty(_accessToken))
        {
            return _accessToken;
        }

        using var client = _clientFactory.CreateClient();

        var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest()
        {
            Address = _m2mAuthSettings.Authority + "/connect/token",
            ClientId = _m2mAuthSettings.ClientId,
            ClientSecret = _m2mAuthSettings.ClientSecret,
            Scope = "chatapi presenceapi"
        });

        if (tokenResponse.IsError)
        {
            throw new InvalidOperationException("Getting authorization token failed.", tokenResponse.Exception);
        }

        _accessToken = tokenResponse.AccessToken!;
        _expiresAt = DateTime.Now + TimeSpan.FromSeconds(tokenResponse.ExpiresIn - 60);

        return _accessToken;
    }
}

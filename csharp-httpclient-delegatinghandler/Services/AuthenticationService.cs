using csharp_httpclient_delegatinghandler.Configurations;
using csharp_httpclient_delegatinghandler.Models;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace csharp_httpclient_delegatinghandler.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly AuthenticationConfiguration authenticationConfiguration;
        private readonly HttpClient httpClient;
        private readonly IMemoryCache memoryCache;
        private readonly string remoteServiceBaseUrl;
        private readonly string tokenCacheKey = "token_cache";
        public AuthenticationService(HttpClient httpClient, AuthenticationConfiguration authenticationConfiguration, IMemoryCache memoryCache)
        {
            this.httpClient = httpClient;
            this.memoryCache = memoryCache;
            this.authenticationConfiguration = authenticationConfiguration;
            this.remoteServiceBaseUrl = $"{authenticationConfiguration.Url}/";
        }

        public async Task<TokenResponse> GetToken()
        {
            var token = this.memoryCache.Get(tokenCacheKey) as TokenResponse;
            if (token == null)
            {
                token = await NewToken();
                this.memoryCache.Set(tokenCacheKey, token, TimeSpan.FromSeconds(token.ExpiresIn));
            }
            return token;
        }

        public async Task<TokenResponse> RefreshToken()
        {
            var token = await NewToken();
            this.memoryCache.Set(tokenCacheKey, token, TimeSpan.FromSeconds(token.ExpiresIn));
            return token;
        }

        private async Task<TokenResponse> NewToken()
        {
            try
            {
                var uri = this.remoteServiceBaseUrl + "version";
                var httpRequestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(uri),
                    Content = new FormUrlEncodedContent(new Dictionary<string, string>()
                    {
                        { "grant_type", $"{authenticationConfiguration.GrantType}" },
                        { "client_id", $"{authenticationConfiguration.ClientId}" },
                        { "client_secret", $"{authenticationConfiguration.ClientSecret}" }
                    })
                };

                var response = await this.httpClient.SendAsync(httpRequestMessage);
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TokenResponse>(responseString) ?? throw new Exception("response is null.");
            }
            catch (HttpRequestException e)
            {
                throw e;
            }
        }
    }
}

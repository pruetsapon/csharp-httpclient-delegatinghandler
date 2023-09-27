using csharp_httpclient_delegatinghandler.Services;
using System.Net;
using System.Net.Http.Headers;

namespace csharp_httpclient_delegatinghandler.Handlers
{
    public class AuthenticationDelegatingHandler : DelegatingHandler
    {
        private readonly IAuthenticationService authenticationService;
        public AuthenticationDelegatingHandler(IAuthenticationService authenticationService)
        {
            this.authenticationService = authenticationService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage, CancellationToken cancellationToken)
        {
            var token = await this.authenticationService.GetToken();
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue($"{token.TokenType}", token.AccessToken);

            var response = await base.SendAsync(requestMessage, cancellationToken);
            if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
            {
                token = await this.authenticationService.RefreshToken();
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue($"{token.TokenType}", token.AccessToken);
                response = await base.SendAsync(requestMessage, cancellationToken);
            }
            return response;
        }
    }
}

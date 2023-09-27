using csharp_httpclient_delegatinghandler.Models;

namespace csharp_httpclient_delegatinghandler.Services
{
    public interface IAuthenticationService
    {
        Task<TokenResponse> GetToken();
        Task<TokenResponse> RefreshToken();
    }
}

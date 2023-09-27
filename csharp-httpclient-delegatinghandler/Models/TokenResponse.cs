namespace csharp_httpclient_delegatinghandler.Models
{
    public class TokenResponse
    {
        public string? AccessToken { get; set; }
        public int ExpiresIn { get; set; }
        public string? TokenType { get; set; }
        public string? Scope { get; set; }
    }
}

namespace csharp_httpclient_delegatinghandler.Services
{
    public interface IDataService
    {
        Task<string> GetVersion();
    }
}

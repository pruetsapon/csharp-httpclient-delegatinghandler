using csharp_httpclient_delegatinghandler.Configurations;

namespace csharp_httpclient_delegatinghandler.Services
{
    public class DataService : IDataService
    {
        private readonly HttpClient httpClient;
        private readonly string remoteServiceBaseUrl;
        public DataService(HttpClient httpClient, DataConfiguration dataConfiguration)
        {
            this.httpClient = httpClient;
            this.remoteServiceBaseUrl = $"{dataConfiguration.Url}/";
        }

        public async Task<string> GetVersion()
        {
            try
            {
                var uri = this.remoteServiceBaseUrl + "version";
                var httpRequestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(uri)
                };

                var response = await this.httpClient.SendAsync(httpRequestMessage);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException e)
            {
                throw e;
            }
        }
    }
}

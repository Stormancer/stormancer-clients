using System;
using System.Net.Http;

namespace Stormancer
{
    internal class DefaultHttpClientFactory : IHttpClientFactory, IDisposable
    {
        public DefaultHttpClientFactory()
        {
        }

        private HttpClient _httpClient = new HttpClient();
        public HttpClient CreateClient(string id)
        {
            return _httpClient;
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
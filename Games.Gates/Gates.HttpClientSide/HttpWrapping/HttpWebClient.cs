using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Gates.HttpClientSide.HttpWrapping
{
    internal class HttpWebClient : IHttpWebClient
    {
        private HttpClient Client { get; }

        public HttpWebClient(HttpClient httpClient)
        {
            Client = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public Task<ResponseContainer<T>> GetAsync<T>(string requestName, IEnumerable<(string, string)> parameters, CancellationToken cancellationToken) where T : class
        {
            throw new NotImplementedException();
        }

        public Task<ResponseContainer<T>> PostAsync<T>(string requestName, IEnumerable<(string, string)> parameters, object body, CancellationToken cancellationToken) where T : class
        {
            throw new NotImplementedException();
        }

        public Task<ResponseContainer<T>> PutAsync<T>(string requestName, IEnumerable<(string, string)> parameters, object body, CancellationToken cancellationToken) where T : class
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            Client.Dispose();
        }
    }
}
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Gates.HttpClientSide.HttpWrapping
{
    internal class ResponseContainer<T> : IDisposable where T : class
    {
        public bool IsSuccessStatusCode { get; }

        public T Content { get; }

        [NotNull] private IDisposable Resources { get; }

        private ResponseContainer(T data, bool isSuccessStatusCode, IDisposable resources)
        {
            Content = data;
            IsSuccessStatusCode = isSuccessStatusCode;
            Resources = resources ?? throw new ArgumentNullException(nameof(resources));
        }

        public void Dispose()
        {
            Resources.Dispose();
        }

        public static async Task<ResponseContainer<T>> FromHttpResponseAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken) where T : class
        {
            var contentString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var data = JsonConvert.DeserializeObject<T>(contentString);
            return new ResponseContainer<T>(data, response.IsSuccessStatusCode, response);
        }
    }
}

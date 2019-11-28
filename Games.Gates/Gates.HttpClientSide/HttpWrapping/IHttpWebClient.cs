using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Gates.HttpClientSide.HttpWrapping
{
    internal interface IHttpWebClient : IDisposable
    {
        Task<ResponseContainer<T>> GetAsync<T>(string requestName, IEnumerable<(string, string)> parameters, CancellationToken cancellationToken) where T : class;

        Task<ResponseContainer<T>> PostAsync<T>(string requestName, IEnumerable<(string, string)> parameters, object body, CancellationToken cancellationToken) where T : class;

        Task<ResponseContainer<T>> PutAsync<T>(string requestName, IEnumerable<(string, string)> parameters, object body, CancellationToken cancellationToken) where T : class;
    }
}

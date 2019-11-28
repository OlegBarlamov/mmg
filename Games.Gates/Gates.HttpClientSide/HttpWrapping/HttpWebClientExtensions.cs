using System.Threading;
using System.Threading.Tasks;

namespace Gates.HttpClientSide.HttpWrapping
{
    internal static class HttpWebClientExtensions
    {
        public static Task<ResponseContainer<T>> GetAsync<T>(this IHttpWebClient client, string request, CancellationToken cancellationToken) where T : class 
        {
            return client.GetAsync<T>(request, new (string, string)[0], cancellationToken);
        }

        public static Task<ResponseContainer<T>> PostAsync<T>(this IHttpWebClient client, string request, object body, CancellationToken cancellationToken) where T : class
        {
            return client.PostAsync<T>(request, new (string, string)[0], body, cancellationToken);
        }

        public static Task<ResponseContainer<T>> PutAsync<T>(this IHttpWebClient client, string request, object body, CancellationToken cancellationToken) where T : class
        {
            return client.PutAsync<T>(request, new (string, string)[0], body, cancellationToken);
        }

        public static Task<ResponseContainer<T>> GetAsync<T>(this IHttpWebClient client, string request, CancellationToken cancellationToken, params (string,string)[] parameters) where T : class
        {
            return client.GetAsync<T>(request, parameters, cancellationToken);
        }

        public static Task<ResponseContainer<T>> PostAsync<T>(this IHttpWebClient client, string request, object body, CancellationToken cancellationToken, params (string, string)[] parameters) where T : class
        {
            return client.PostAsync<T>(request, parameters, body, cancellationToken);
        }

        public static Task<ResponseContainer<T>> PutAsync<T>(this IHttpWebClient client, string request, object body, CancellationToken cancellationToken, params (string, string)[] parameters) where T : class
        {
            return client.PutAsync<T>(request, parameters, body, cancellationToken);
        }
    }
}

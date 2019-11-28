using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Gates.Core.ServerApi;
using Gates.HttpClientSide.HttpWrapping;
using JetBrains.Annotations;

namespace Gates.HttpClientSide
{
    [UsedImplicitly]
    public class HttpServerConnector : IServerConnector
    {
        public async Task<IServer> ConnectAsync(string url, CancellationToken cancellationToken)
        {
            var uri = new UriBuilder(url).Uri;
            var client = new HttpClient
            {
                BaseAddress = uri,
                Timeout = TimeSpan.FromSeconds(10),
            };

            try
            {
                using (var pingResponse = await client.GetAsync("ping", cancellationToken).ConfigureAwait(false))
                {
                    if (!pingResponse.IsSuccessStatusCode)
                        throw new GatesHttpClientSideException($"Server {url} not respond.");

                    var httpWebClient = new HttpWebClient(client);
                    return new HttpServer(httpWebClient);
                }
            }
            catch (Exception e)
            {
                client.Dispose();
                throw new GatesHttpClientSideException($"Can't connect to server {url}.", e);
            }
        }
    }
}

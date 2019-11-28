using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Gates.Core.Models;
using Gates.Core.ServerApi;
using Gates.HttpClientSide.HttpWrapping;

namespace Gates.HttpClientSide
{
    internal class HttpServer : IServer, IServerApi, IServerRoomApi, IServerGatesApi
    {
        private IHttpWebClient Client { get; }

        private string _userToken;
        private string _roomToken;

        public HttpServer(IHttpWebClient client)
        {
            Client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<IServerApi> AuthorizeAsync(string name, string password, CancellationToken cancellationToken)
        {
            var authData = new AuthData
            {
                User = name,
                Password = password
            };

            using (var response = await Client.PostAsync<AuthResponse>("auth", authData, cancellationToken)
                .ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                    throw new GatesHttpClientSideException("Bad auth");

                _userToken = response.Content.Token;

                return this;
            }
        }

        public async Task<IServerRoomApi> CreateRoomAsync(string name, string password,
            CancellationToken cancellationToken)
        {
            var roomData = new RoomData
            {
                Name = name,
                Password = password
            };

            using (var response = await Client
                .PutAsync<RoomResponse>("room", roomData, cancellationToken, ("token", _userToken))
                .ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                    throw new GatesHttpClientSideException("Creating room failed");

                _roomToken = response.Content.Token;

                return this;
            }
        }

        public async Task<IServerRoomApi> EnterRoomAsync(string name, string password, CancellationToken cancellationToken)
        {
            var roomData = new RoomData
            {
                Name = name,
                Password = password
            };

            using (var response = await Client
                .PostAsync<RoomResponse>("room/enter", roomData, cancellationToken, ("token", _userToken))
                .ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                    throw new GatesHttpClientSideException("Entering room failed");

                _roomToken = response.Content.Token;

                return this;
            }
        }

        public async Task<IReadOnlyCollection<RoomDescription>> GetRoomsAsync(CancellationToken cancellationToken)
        {
            using (var response = await Client
                .GetAsync<RoomDescription[]>("room/list", cancellationToken, ("token", _userToken))
                .ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                    throw new GatesHttpClientSideException("Getting rooms failed");

                return response.Content.ToArray();
            }
        }

        public async Task LeaveRoomAsync(CancellationToken cancellationToken)
        {
            using (var response = await Client
                .PostAsync<string>("room/leave", new object(), cancellationToken, ("token", _roomToken))
                .ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                    throw new GatesHttpClientSideException("Leaving room failed");
            }
        }

        public async Task<RoomState> GetStateAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task RunGatesGameAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<IServerGatesApi> ConnectToRunnedGameAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task PostUserReadyAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<GameStartInfo> GetStartInfoAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<GameDataIteration> GemeStateAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
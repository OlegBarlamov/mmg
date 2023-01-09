using System;

namespace BoardPlatform.Server.Controllers.ResponseData
{
    [Serializable]
    public class BoardInfoResponse
    {
        public string Id { get; }
        public BoardInfoResponse(string id)
        {
            Id = id;
        }
    }
}
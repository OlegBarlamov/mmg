using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Epic.Core.ClientMessages;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace Epic.Core.Services.Connection
{
    public interface IClientMessagesParserService
    {
        bool ParseSafe(string message, out IClientBattleMessage parsedMessage);
    }

    [UsedImplicitly]
    public class ClientMessagesParserService : IClientMessagesParserService
    {
        private ILogger Logger { get; }
        
        public ClientMessagesParserService([NotNull] ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null) throw new ArgumentNullException(nameof(loggerFactory));
            Logger = loggerFactory.CreateLogger<ClientMessagesParserService>();
        }
        public bool ParseSafe(string message, out IClientBattleMessage parsedMessage)
        {
            parsedMessage = null;
            try
            {
                using var document = JsonDocument.Parse(message);
                var root = document.RootElement;

                var command = root.GetProperty("command").GetString();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                options.Converters.Add(new JsonStringEnumConverter());
                parsedMessage = command switch
                {
                    "UNIT_MOVE" => JsonSerializer.Deserialize<UnitMoveClientBattleMessage>(message, options),
                    "UNIT_ATTACK" => JsonSerializer.Deserialize<UnitAttackClientBattleMessage>(message, options),
                    _ => throw new InvalidOperationException("Unknown command type")
                };
                return true;
            }
            catch (Exception e)
            {
                Logger.LogError($"Error while parsing client message {message}, error: {e.Message}");
                return false;
            }
        }
    }
}
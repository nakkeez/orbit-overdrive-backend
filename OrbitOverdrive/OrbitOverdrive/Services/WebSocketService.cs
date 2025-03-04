using System.Net.WebSockets;
using System.Text;
using OrbitOverdrive.Models;

namespace OrbitOverdrive.Services;

/// <summary>
/// Service for handling WebSocket connections and communication.
/// </summary>
public class WebSocketService
{
    private static readonly Dictionary<string, WebSocket> Sockets = [];
    private static readonly Room Room = new();

    /// <summary>
    /// Manages the WebSocket lifecycle for a client, including message handling
    /// </summary>
    /// <param name="context">The HTTP context of the request.</param>
    /// <param name="logger">The logger to log events and errors.</param>
    public async Task HandleWebSocketAsync(HttpContext context, ILogger logger)
    {
        using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        var player = Room.JoinRoom();
        lock (Sockets)
        {
            Sockets.Add(player.Id, webSocket);
        }

        var joinMessage = new ServerConnectMessage(player);
        var serializedJoinMessage = JsonSerializationService.Serialize(joinMessage);
        await SendMessageAsync(serializedJoinMessage, webSocket);

        await ProcessMessagesAsync(webSocket, logger);

        Room.LeaveRoom(player.Id);
        lock (Sockets)
        {
            Sockets.Remove(player.Id);
        }

        var disconnectMessage = new ServerDisconnectMessage(player.Id);
        var serializedDisconnectMessage = JsonSerializationService.Serialize(disconnectMessage);
        await BroadcastMessageAsync(serializedDisconnectMessage);
    }

    /// <summary>
    /// Processes messages received from a WebSocket.
    /// </summary>
    /// <param name="webSocket">The WebSocket to receive messages from.</param>
    /// <param name="logger">The logger to log events and errors.</param>
    private async Task ProcessMessagesAsync(WebSocket webSocket, ILogger logger)
    {
        var buffer = new byte[1024 * 4];
        try
        {
            var receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!receiveResult.CloseStatus.HasValue)
            {
                var result = new ArraySegment<byte>(buffer, 0, receiveResult.Count);
                var message = Encoding.UTF8.GetString(result);
                logger.LogInformation("Received message: {Message}", message);

                var clientMessage = JsonSerializationService.Deserialize<ClientMessage>(message);

                if (clientMessage is not null)
                {
                    switch (clientMessage.Type)
                    {
                        case ClientMessageType.Move:
                            Room.MovePlayer(clientMessage.PlayerId, (PlayerAction)clientMessage.Action);
                            break;
                    }
                }

                var players = Room.GetAllPlayers();
                var gameUpdate = new ServerUpdateMessage(players);
                var serializedGameUpdate = JsonSerializationService.Serialize(gameUpdate);
                await BroadcastMessageAsync(serializedGameUpdate);

                receiveResult = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await webSocket.CloseAsync(
                receiveResult.CloseStatus.Value,
                receiveResult.CloseStatusDescription,
                CancellationToken.None);
        }
        catch (WebSocketException ex)
        {
            logger.LogError("WebSocketException: {Message}", ex.Message);
        }
    }

    /// <summary>
    /// Sends a message to a WebSocket.
    /// </summary>
    /// <param name="message">The message to send.</param>
    /// <param name="socket">The WebSocket to send the message to.</param>
    private async Task SendMessageAsync(string message, WebSocket socket)
    {
        var buffer = Encoding.UTF8.GetBytes(message);
        await socket.SendAsync(
            new ArraySegment<byte>(buffer),
            WebSocketMessageType.Text,
            true,
            CancellationToken.None);
    }
   
    /// <summary>
    /// Broadcasts a message to all connected clients.
    /// </summary>
    /// <param name="message">The message to broadcast.</param>
    private async Task BroadcastMessageAsync(string message)
    {
        // Copy the clients to dictionary so lock can be released before broadcasting the messages
        Dictionary<string, WebSocket> socketsCopy;
        lock (Sockets)
        {
            socketsCopy = new Dictionary<string, WebSocket>(Sockets);
        }

        foreach (var socket in socketsCopy.Where(socket => socket.Value.State == WebSocketState.Open))
        {
            await SendMessageAsync(message, socket.Value);
        }
    }
}
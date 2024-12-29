using System.Net.WebSockets;

namespace OrbitOverdriveServer.Services
{
    /// <summary>
    /// Service for handling WebSocket connections and communication.
    /// </summary>
    public class WebSocketService
    {
        private static readonly List<WebSocket> Sockets = [];

        /// <summary>
        /// Manages the WebSocket lifecycle for a client, including message handling and
        /// broadcasting received messages.
        /// </summary>
        /// <param name="context">The HTTP context of the request.</param>
        /// <param name="logger">The logger to log events and errors.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task HandleWebSocketAsync(HttpContext context, ILogger logger)
        {
            using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            lock (Sockets)
            {
                Sockets.Add(webSocket);
            }

            await Echo(webSocket, logger);
            lock (Sockets)
            {
                Sockets.Remove(webSocket);
            }
        }

        /// <summary>
        /// Echoes messages received from a WebSocket to all connected clients.
        /// </summary>
        /// <param name="webSocket">The WebSocket to receive messages from.</param>
        /// <param name="logger">The logger to log events and errors.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private static async Task Echo(WebSocket webSocket, ILogger logger)
        {
            var buffer = new byte[1024 * 4];
            try
            {
                var receiveResult = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer), CancellationToken.None);

                while (!receiveResult.CloseStatus.HasValue)
                {
                    var message = new ArraySegment<byte>(buffer, 0, receiveResult.Count);

                    // Copy the clients to list so lock can be released before broadcasting the messages
                    List<WebSocket> socketsCopy;
                    lock (Sockets)
                    {
                        socketsCopy = [.. Sockets];
                    }

                    foreach (var socket in socketsCopy.Where(socket => socket.State == WebSocketState.Open))
                    {
                        await socket.SendAsync(
                            message,
                            receiveResult.MessageType,
                            receiveResult.EndOfMessage,
                            CancellationToken.None);
                    }

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
    }
}
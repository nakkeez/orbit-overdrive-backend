using Microsoft.AspNetCore.Mvc;
using OrbitOverdrive.Services;

namespace OrbitOverdrive.Controllers
{
    /// <summary>
    /// Controller for handling WebSocket connections.
    /// </summary>
    /// <param name="logger">The logger to log information and errors.</param>
    [Route("/ws")]
    [ApiController]
    public class WebSocketController(ILogger<WebSocketController> logger) : ControllerBase
    {
        private readonly WebSocketService _webSocketService = new();

        /// <summary>
        /// Handles the WebSocket GET request.
        /// If the request is a WebSocket request, delegates handling to the WebSocketService.
        /// Otherwise, returns a 400 Bad Request status code.
        /// </summary>
        [HttpGet(Name = "WebSocket")]
        public async Task Get()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                await _webSocketService.HandleWebSocketAsync(HttpContext, logger);
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }
    }
}

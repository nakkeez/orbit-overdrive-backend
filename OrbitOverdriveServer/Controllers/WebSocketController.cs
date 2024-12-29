using Microsoft.AspNetCore.Mvc;
using OrbitOverdriveServer.Services;

namespace OrbitOverdriveServer.Controllers
{
    /// <summary>
    /// Controller for handling WebSocket connections.
    /// </summary>
    [Route("/ws")]
    [ApiController]
    public class WebSocketController : ControllerBase
    {
        private readonly ILogger<WebSocketController> _logger;
        private readonly WebSocketService _webSocketService = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="WebSocketController"/> class.
        /// </summary>
        /// <param name="logger">The logger to log information and errors.</param>
        public WebSocketController(ILogger<WebSocketController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Handles the WebSocket GET request.
        /// If the request is a WebSocket request, delegates handling to the WebSocketService.
        /// Otherwise, returns a 400 Bad Request status code.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [HttpGet(Name = "WebSocket")]
        public async Task Get()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                await _webSocketService.HandleWebSocketAsync(HttpContext, _logger);
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }
    }
}
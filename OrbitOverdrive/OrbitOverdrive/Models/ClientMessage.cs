namespace OrbitOverdrive.Models;

/// <summary>
/// Enum for the type of message sent by the client.
/// </summary>
public enum ClientMessageType
{
    Move
}


/// <summary>
/// Class that represents a message sent by a client to the server.
/// </summary>
/// <param name="type">The type of message.</param>
/// <param name="action">The action to take.</param>
/// <param name="playerId">The ID of the player sending the message. Defaults to null.</param>
public class ClientMessage(ClientMessageType type, int action, string playerId)
{
    public ClientMessageType Type { get; init; } = type;
    public int Action { get; init; } = action;
    public string PlayerId { get; init; } = playerId;
}

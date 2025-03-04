namespace OrbitOverdrive.Models;

/// <summary>
/// Enum for the type of message sent by the server.
/// </summary>
public enum ServerMessageType
{
    Connect,
    Disconnect,
    Update
}

/// <summary>
/// Abstract class that represents a message sent by the server to the client.
/// </summary>
/// <param name="type">The type of message.</param>
public abstract class ServerMessage(ServerMessageType type)
{
    public ServerMessageType Type { get; init; } = type;
}

/// <summary>
/// Class that represents message sent by the server after successful connection.
/// </summary>
/// <param name="player">The player that connected.</param>
public class ServerConnectMessage(SpaceShip player) : ServerMessage(ServerMessageType.Connect)
{
    public SpaceShip Player { get; init; } = player;
}

/// <summary>
/// Class that represents message sent by the server after a player disconnects.
/// </summary>
/// <param name="playerId">The ID of the player that disconnected.</param>
public class ServerDisconnectMessage(string playerId) : ServerMessage(ServerMessageType.Disconnect)
{
    public string PlayerId { get; init; } = playerId;
}

/// <summary>
/// Class that represents message sent by the server to update the game state.
/// </summary>
/// <param name="players">The list of players in the game state.</param>
public class ServerUpdateMessage(IEnumerable<SpaceShip> players) : ServerMessage(ServerMessageType.Update)
{
    public List<SpaceShip> Players { get; init; } = players.ToList();
}
namespace OrbitOverdrive.Models;

/// <summary>
/// Class for managing the game state and player interactions.
/// </summary>
public class Room
{
    private readonly GameState _gameState = new();

    /// <summary>
    /// Adds a new player to the game state.
    /// </summary>
    public SpaceShip JoinRoom()
    {
        var newPlayer = new SpaceShip(0, 0);
        _gameState.AddPlayer(newPlayer);
        return newPlayer;
    }
   
    /// <summary>
    /// Removes a player from the game state.
    /// </summary>
    /// <param name="playerId">The ID of the player to remove.</param>
    public void LeaveRoom(string playerId)
    {
        _gameState.RemovePlayer(playerId);
    }

    /// <summary>
    /// Moves a player in the game state.
    /// </summary>
    /// <param name="playerId">The ID of the player to move.</param>
    /// <param name="action">The action to take.</param>
    public void MovePlayer(string playerId, PlayerAction action)
    {
        _gameState.MovePlayer(playerId, action);
    }
    
    /// <summary>
    /// Gets all players in the game state.
    /// </summary>
    /// <returns>An array of all players in the game state.</returns>
    public SpaceShip[] GetAllPlayers()
    {
        return _gameState.Players.ToArray();
    }
}

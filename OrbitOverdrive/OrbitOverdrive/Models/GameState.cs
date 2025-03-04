namespace OrbitOverdrive.Models;

/// <summary>
/// Enum for the actions a player can take.
/// </summary>
public enum PlayerAction
{
    Forward,
    Right,
    Left,
    Backward
}

/// <summary>
/// Class that represents the game state.
/// </summary>
public class GameState
{
    public List<SpaceShip> Players { get; } = [];

    /// <summary>
    /// Adds a player to the game state.
    /// </summary>
    /// <param name="spaceShip">The player to add.</param>
    public void AddPlayer(SpaceShip spaceShip)
    {
        Players.Add(spaceShip);
    }

    /// <summary>
    /// Removes a player from the game state.
    /// </summary>
    /// <param name="id">The ID of the player to remove.</param>
    public void RemovePlayer(string id)
    {
        Players.RemoveAll(player => player.Id == id);
    }

    /// <summary>
    /// Moves player's location in the game state based on the action taken.
    /// </summary>
    /// <param name="id">The ID of the player to move.</param>
    /// <param name="action">The action to take.</param>
    public void MovePlayer(string id, PlayerAction action)
    {
        var player = Players.Find(player => player.Id == id);
        if (player is null)
        {
            return;
        }

        switch (action)
        {
            case PlayerAction.Forward:
                player.Move(player.X, player.Y + 0.01);
                break;
            case PlayerAction.Right:
                player.Move(player.X + 0.01, player.Y);
                break;
            case PlayerAction.Left:
                player.Move(player.X - 0.01, player.Y);
                break;
            case PlayerAction.Backward:
                player.Move(player.X, player.Y - 0.01);
                break;
        }
    }
}

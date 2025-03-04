namespace OrbitOverdrive.Models;

/// <summary>
/// Class that represents a spaceship in the game.
/// </summary>
/// <param name="x">The X coordinate of the spaceship.</param>
/// <param name="y">The Y coordinate of the spaceship.</param>
public class SpaceShip(double x, double y)
{
    public string Id { get; private set; } = Guid.NewGuid().ToString();
    public double X { get; private set; } = x;
    public double Y { get; private set; } = y;
    
    /// <summary>
    /// Moves the spaceship to a new location.
    /// </summary>
    /// <param name="x">The new X coordinate.</param>
    /// <param name="y">The new Y coordinate.</param>
    public void Move(double x, double y)
    {
        X = x;
        Y = y;
    }
}

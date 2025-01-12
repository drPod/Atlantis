using Raylib_cs;
using System.Numerics;

/* Components for the ECS */
public record struct Position(float X, float Y) // Position: pixels
{
    public Vector2 Vector2 { get => new Vector2(X, Y); }
}
public record struct Velocity(float Dx, float Dy) // Velocity: pixels/sec
{
    public Vector2 Vector2 { get => new Vector2(Dx, Dy); }
}
public record struct HitboxRectangle(Rectangle rect); // Rectangular hitbox, relative to the top left corner of the texture, in pixels
public record struct HitboxCircle(Vector2 center, float radius); // Circular hitbox, relative to the center of the texture, in pixels

public record struct SourceRects(Rectangle[] rects, int frame)
{
    public Rectangle CurrentRect { get => rects[frame]; }
}

public record struct Player(); // an empty component, used as a boolean value for whether the entity is a player

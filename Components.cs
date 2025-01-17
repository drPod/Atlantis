using Raylib_cs;
using System.Numerics;

/* Components for the ECS */
public record struct Position(float X, float Y) // Position: pixels
{
    public Vector2 Vector2 { get => new Vector2(X, Y); }
}
public record struct UIPos(float X, float Y)
{
    public Vector2 Vector2 { get => new Vector2(X, Y); }
}
public record struct Velocity(float Dx, float Dy) // Velocity: pixels/sec
{
    public Vector2 Vector2 { get => new Vector2(Dx, Dy); }
}
public record struct Speed(float Dx); // Speed: pixels/sec
public record struct HitboxRectangle(Rectangle rect); // Rectangular hitbox, relative to the top left corner of the texture, in pixels
public record struct HitboxCircle(Vector2 center, float radius); // Circular hitbox, relative to the center of the texture, in pixels

// rects is a set of key-value pairs, with the keys being string names for sprite sets and the values being arrays of source rectangles for sprites
public record struct SourceRects(Dictionary<String,Rectangle[]> rects, String set, int frame)
{
    public Rectangle CurrentRect { get => rects[set][frame]; }
    public Rectangle[] CurrentSet { get => rects[set]; }
}
public record struct AnimationData(double delay, double lastFrameTime, bool isFlippedHorizontal); // Delay between animation frames in seconds
public record struct Player(); // an empty component, used as a boolean value for whether the entity is a player
public record struct Fish(); // an empty component, used as a boolean value for whether the entity is a Fish
public record struct Gravity(); // enables gravity for this entity

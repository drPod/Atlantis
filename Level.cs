using Raylib_cs;
using Arch.Core;
using System.Numerics;

namespace Atlantis;

abstract class Level : ILevel
{
    // structs for the ECS
    public record struct Position(float X, float Y); // Position: pixels
    public record struct Velocity(float Dx, float Dy); // Velocity: pixels/sec
    public record struct Texture(Texture2D texture);
    public record struct HitboxRectangle(Vector2 center, float radius); // Rectangular hitbox, relative to the top left corner of the texture
    public record struct HitboxCircle(Vector2 center, float radius); // Circular hitbox, relative to the center of the texture

    public World World { get; }
    public Camera2D Camera { get => camera; }
    protected Camera2D camera;

    public Level(int windowWidth, int windowHeight)
    {
        World = World.Create();
        camera = new Camera2D();
        camera.Target = new Vector2(0, 0);
        camera.Offset = new Vector2(windowWidth / 2, windowHeight / 2);
        camera.Rotation = 0.0f;
        camera.Zoom = 1.0f;
    }

    public abstract void UpdateLevel();

    public abstract void DrawLevel();
}

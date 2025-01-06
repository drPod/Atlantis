using Raylib_cs;
using Arch.Core;
using System.Numerics;

namespace Atlantis;

abstract class Level : ILevel,IDisposable
{
    // structs for the ECS
    public record struct Position(float X, float Y); // Position: pixels
    public record struct Velocity(float Dx, float Dy); // Velocity: pixels/sec
    public record struct Texture(Texture2D texture);
    public record struct HitboxRectangle(Rectangle rec); // Rectangular hitbox, relative to the top left corner of the texture
    public record struct HitboxCircle(Vector2 center, float radius); // Circular hitbox, relative to the center of the texture

    public World LevelWorld { get; }
    public Camera2D Camera { get => camera; }
    protected Camera2D camera;

    public Level(int windowWidth, int windowHeight)
    {
        LevelWorld = World.Create();
        camera = new Camera2D();
        camera.Target = new Vector2(0, 0);
        camera.Offset = new Vector2(windowWidth / 2, windowHeight / 2);
        camera.Rotation = 0.0f;
        camera.Zoom = 1.0f;
    }

    // Dispose of world
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    // From https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose
    private bool _disposed;

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing) {
            LevelWorld.TrimExcess();
            LevelWorld.Dispose();
        }

        World.Destroy(LevelWorld);

        _disposed = true;
    }

    // Destructor to unload world
    ~Level()
    {
        Dispose();
    }

    public abstract void UpdateLevel();

    public abstract void DrawLevel();
}

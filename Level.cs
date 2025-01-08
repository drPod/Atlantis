using Raylib_cs;
using Arch.Core;
using System.Numerics;

namespace Atlantis;

/* Levels contain the logic to interact with and view seperate worlds.
 * They accomplish this by having their own cameras and ECSs to provide
 * an API to simulate and draw virtual environments.
 * Using inheritance, child classes of levels can apply pre or post-processing
 * to the world/ECS of the parent levels.
 */
abstract class Level : ILevel,IDisposable
{
    public World LevelWorld { get => world; }
    protected World world;
    public Camera2D Camera { get => camera; }
    protected Camera2D camera;

    public Level(int windowWidth, int windowHeight)
    {
        world = World.Create();
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
    private bool _disposed = false;

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

    ~Level()
    {
        Dispose();
    }

    public abstract void UpdateLevel(Vector2 virtualMousePos);

    public abstract void DrawLevel();
}

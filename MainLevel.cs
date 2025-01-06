using Raylib_cs;
using static Raylib_cs.Raylib;
using Arch.Core;
using System.Numerics;

namespace Atlantis;

class MainLevel : Level,ILevel
{
    public MainLevel(int windowWidth, int windowHeight) : base(windowWidth, windowHeight)
    {
        // Change the camera here... or not, I won't force you
    }

    public override void UpdateLevel()
    {
        if (IsKeyDown(KeyboardKey.Left)) camera.Target.X -= 5;
        if (IsKeyDown(KeyboardKey.Right)) camera.Target.X += 5;
        if (IsKeyDown(KeyboardKey.Up)) camera.Target.Y -= 5;
        if (IsKeyDown(KeyboardKey.Down)) camera.Target.Y += 5;
    }

    public override void DrawLevel()
    {
        // We draw to a camera in order to transform world space into screen space
        // Draw here for things that are relative to the world (ie enemies, objects in the game world)
        BeginMode2D(camera);

        DrawRectangle(5, 5, 100, 100, Color.Black);

        EndMode2D();
    }
}

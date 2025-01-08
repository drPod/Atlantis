using Raylib_cs;
using static Raylib_cs.Raylib;
using Arch.Core;
using Arch.Core.Extensions;
using System.Numerics;

namespace Atlantis;

class LevelEditor : MainLevel, ILevel, IDisposable
{
    public record struct Dragging(Vector2 cursorOffset); // Component placed on object when it is being dragged

    protected bool running = false;

    public LevelEditor(int windowWidth, int windowHeight, IContentLoader contentLoader) : base(windowWidth, windowHeight, contentLoader)
    {
        SetWindowTitle("Level Editor");
    }

    private bool RectangleContains(Rectangle r, Vector2 point)
    {
        return point.X > r.X &&
               point.X < r.X + r.Width &&
               point.Y > r.Y &&
               point.Y < r.Y + r.Height;
    }

    private Rectangle RectangleFromTexture(Position pos, Texture2D texture)
    {
        return new Rectangle(pos.X, pos.Y, texture.Width, texture.Height);
    }

    public override void UpdateLevel(Vector2 virtualMousePos)
    {
        // Test out the level
        if (running) {
            base.UpdateLevel(virtualMousePos);
            return;
        }

        // Navigate around the editor
        camera.Zoom += ((float)GetMouseWheelMove() * 0.05f);
        if (camera.Zoom < 0) camera.Zoom = 0;

        if (IsMouseButtonDown(MouseButton.Right)) {
            camera.Target += GetMouseDelta() / camera.Zoom * 0.5f;
        }

        // Drag items around the editor
        Vector2 cursorPositionInWorld = GetScreenToWorld2D(virtualMousePos, camera);
        var queryForObjects = new QueryDescription().WithAll<Position, Texture2D>().WithNone<Dragging>();
        world.Query(in queryForObjects, (Entity entity, ref Position pos, ref Texture2D texture) => {
            if (IsMouseButtonDown(MouseButton.Left) &&
                RectangleContains(RectangleFromTexture(pos, texture),
                                  cursorPositionInWorld))
                entity.Add(new Dragging(Vector2.Subtract(cursorPositionInWorld, pos.Vector2)));
        });
        var queryBeingDragged = new QueryDescription().WithAll<Dragging, Position>();
        world.Query(in queryBeingDragged, (Entity entity, ref Position pos, ref Dragging dragging) => {
            if (IsMouseButtonDown(MouseButton.Left)) {
                pos.X = cursorPositionInWorld.X - dragging.cursorOffset.X;
                pos.Y = cursorPositionInWorld.Y - dragging.cursorOffset.Y;
            } else
                entity.Remove<Dragging>();
        });

        /* editor keybindings */
        if (IsKeyPressed(KeyboardKey.R)) running = !running;

    }

    public override void DrawLevel()
    {
        base.DrawLevel();
        /* editor gui */
    }
}

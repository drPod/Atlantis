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

    private Rectangle RectangleFromTexture(Position pos, Texture2D texture)
    {
        return new Rectangle(pos.X, pos.Y, texture.Width, texture.Height);
    }

    private Rectangle RectangleFromSourceRect(Position pos, Rectangle sourceRect)
    {
        return new Rectangle(pos.X, pos.Y, sourceRect.Width, sourceRect.Height);
    }

    public override void UpdateLevel(Vector2 virtualMousePos)
    {
        /* editor keybindings */
        if (IsKeyPressed(KeyboardKey.R)) running = !running;

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

        Vector2 cursorPositionInWorld = GetScreenToWorld2D(virtualMousePos, camera);

        // Add items to editor
        var queryForObjectsInUI = new QueryDescription().WithAll<Texture2D, UIPos>();
        world.Query(in queryForObjectsInUI, (Entity entity, ref Texture2D texture, ref UIPos pos) => {
            if (IsMouseButtonPressed(MouseButton.Left) &&
                CheckCollisionPointRec(virtualMousePos, new Rectangle(pos.X, pos.Y, texture.Width, texture.Height))) {
                Entity entityCopy = world.Create(entity.GetArchetype().Types);
                Vector2 entityCopyPosInWorld = GetScreenToWorld2D(pos.Vector2, camera);
                entityCopy.Add(new Position(entityCopyPosInWorld.X, entityCopyPosInWorld.Y));
                entityCopy.Add(new Dragging(Vector2.Subtract(cursorPositionInWorld, entityCopyPosInWorld)));
                foreach (var c in entity.GetAllComponents()) {
                    if (c != null && c.GetType() != typeof(UIPos))
                        entityCopy.Set(c);
                }
            }
        });

        // Drag items around the editor
        var queryForObjects = new QueryDescription().WithAll<Position, Texture2D>().WithNone<Dragging>();
        world.Query(in queryForObjects, (Entity entity, ref Position pos, ref Texture2D texture) => {

            Rectangle spriteRect;
            if (entity.Has<SourceRects>())
                spriteRect = RectangleFromSourceRect(pos, entity.Get<SourceRects>().CurrentRect);
            else
                spriteRect = RectangleFromTexture(pos, texture);

            if (IsMouseButtonPressed(MouseButton.Left) &&
                CheckCollisionPointRec(cursorPositionInWorld, spriteRect)) {
                entity.Add(new Dragging(Vector2.Subtract(cursorPositionInWorld, pos.Vector2)));
            }
        });
        var queryBeingDragged = new QueryDescription().WithAll<Dragging, Position>();
        world.Query(in queryBeingDragged, (Entity entity, ref Position pos, ref Dragging dragging) => {
            if (IsMouseButtonDown(MouseButton.Left)) {
                pos.X = cursorPositionInWorld.X - dragging.cursorOffset.X;
                pos.Y = cursorPositionInWorld.Y - dragging.cursorOffset.Y;
            } else
                entity.Remove<Dragging>();
        });
    }

    public override void DrawLevel()
    {
        base.DrawLevel();
        /* editor gui */
        var queryToDrawSpritesheetUI = new QueryDescription().WithAll<UIPos, Texture2D, SourceRects>();
        world.Query(in queryToDrawSpritesheetUI, (Entity entity, ref UIPos pos, ref Texture2D texture, ref SourceRects source) => {
            Rectangle sourceRect = source.CurrentRect;
            // apply AnimationData.isFlippedHorizontal
            if (entity.Has<AnimationData>() && entity.Get<AnimationData>().isFlippedHorizontal)
                sourceRect.Width = -sourceRect.Width;

            DrawTextureRec(texture, sourceRect, pos.Vector2, Color.White);
        });

    }
}

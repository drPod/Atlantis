using Raylib_cs;
using static Raylib_cs.Raylib;
using Arch.Core;
using System.Numerics;

namespace Atlantis;

class MainLevel : Level,ILevel,IDisposable
{
    private bool drawHitboxes = true;
    private int gamepad = -1;

    public MainLevel(int windowWidth, int windowHeight, IContentLoader contentLoader) : base(windowWidth, windowHeight)
    {
        contentLoader.LoadContentIntoWorld(world);
    }

    public override void UpdateLevel(Vector2 virtualMousePos)
    {
        /* keybindings */
        /* debug */
        if (IsKeyPressed(KeyboardKey.H)) drawHitboxes = !drawHitboxes;

        if (IsGamepadAvailable(0))
            gamepad = 0;
        else
            gamepad = -1;

        // player movement
        var queryForPlayer = new QueryDescription().WithAll<Player, Position, HitboxRectangle, Velocity, SourceRects>();
        world.Query(in queryForPlayer, (ref Velocity vel, ref SourceRects source) => {
            // TODO: add controller movement

            /* collision */
            // animation
            if (IsKeyPressed(KeyboardKey.Space))
                source.frame = (source.frame + 1) % source.rects.Length;
        });

        // Apply velocities
        var queryForVelocities = new QueryDescription().WithAll<Position, Velocity>();
        world.Query(in queryForVelocities, (ref Position pos, ref Velocity vel) => {
            pos.X += vel.Dx * GetFrameTime();
            pos.Y += vel.Dy * GetFrameTime();
        });
    }

    public override void DrawLevel()
    {
        // We draw to a camera in order to transform world space into screen space
        // Draw here for things that are relative to the world (ie enemies, objects in the game world)
        BeginMode2D(camera);

        var queryToDraw = new QueryDescription().WithAll<Position, Texture2D>().WithNone<SourceRects>();
        world.Query(in queryToDraw, (ref Position pos, ref Texture2D texture) => {
            DrawTextureV(texture, pos.Vector2, Color.White);
        });
        var queryToDrawSpritesheet = new QueryDescription().WithAll<Position, Texture2D, SourceRects>();
        world.Query(in queryToDrawSpritesheet, (ref Position pos, ref Texture2D texture, ref SourceRects source) => {
            DrawTextureRec(texture, source.rects[source.frame], pos.Vector2, Color.White);
        });

        if (drawHitboxes) {
            var queryToDrawHitboxes = new QueryDescription().WithAll<Position, HitboxRectangle>();
            world.Query(in queryToDrawHitboxes, (ref Position pos, ref HitboxRectangle hitbox) => {
                DrawRectangleLinesEx(new Rectangle(pos.X + hitbox.rect.X,
                                                   pos.Y + hitbox.rect.Y,
                                                   hitbox.rect.Width,
                                                   hitbox.rect.Height), 5, Color.Gray);
            });
        }

        EndMode2D();
    }
}

using Raylib_cs;
using static Raylib_cs.Raylib;
using Arch.Core;
using System.Numerics;

namespace Atlantis;

class MainLevel : Level,ILevel,IDisposable
{
    private bool drawHitboxes = true;

    public MainLevel(int windowWidth, int windowHeight, IContentLoader contentLoader) : base(windowWidth, windowHeight)
    {
        contentLoader.LoadContentIntoWorld(world);
    }

    public override void UpdateLevel(Vector2 virtualMousePos)
    {
        /* keybindings */
        /* debug */
        if (IsKeyPressed(KeyboardKey.H)) drawHitboxes = !drawHitboxes;

        // player movement
        var queryForPlayer = new QueryDescription().WithAll<Player, Velocity>();
        world.Query(in queryForPlayer, (ref Velocity vel) => {
            // TODO: add controller movement
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

        var queryToDraw = new QueryDescription().WithAll<Position, Texture2D>();
        world.Query(in queryToDraw, (ref Position pos, ref Texture2D texture) => {
            DrawTexture(texture, (int)MathF.Round(pos.X), (int)MathF.Round(pos.Y), Color.White);
        });

        if (drawHitboxes) {
            var queryToDrawHitboxes = new QueryDescription().WithAll<Position, HitboxRectangle>();
            world.Query(in queryToDrawHitboxes, (ref Position pos, ref HitboxRectangle hitbox) => {
                DrawRectangle((int)MathF.Round(pos.X + hitbox.rect.X),
                              (int)MathF.Round(pos.Y + hitbox.rect.Y),
                              (int)MathF.Round(hitbox.rect.Width),
                              (int)MathF.Round(hitbox.rect.Height), Color.Green);
            });
        }

        EndMode2D();
    }
}

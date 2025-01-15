using Raylib_cs;
using static Raylib_cs.Raylib;
using Arch.Core;
using Arch.Core.Extensions;
using System.Numerics;

namespace Atlantis;

class MainLevel : Level,ILevel,IDisposable
{
    private bool drawHitboxes = Program.Debug;
    private Vector2 gravity = new Vector2(0, 5f);
    protected double gameTime;

    public MainLevel(int windowWidth, int windowHeight, IContentLoader contentLoader) : base(windowWidth, windowHeight)
    {
        contentLoader.LoadContentIntoWorld(world);
        gameTime = 0;
    }

    public override void UpdateLevel(Vector2 virtualMousePos)
    {
        float deltaTime = GetFrameTime();

        /* keybindings */
        /* debug */
        if (IsKeyPressed(KeyboardKey.H)) drawHitboxes = !drawHitboxes;

        var queryForPlayer = new QueryDescription().WithAll<Player, Velocity, Speed, AnimationData>();
        world.Query(in queryForPlayer, (ref Velocity vel, ref Speed speed, ref AnimationData anim) => {
            /* player movement */
            Vector2 movementVector = Vector2.Zero;
            if (IsKeyDown(KeyboardKey.W)) movementVector.Y -= 1;
            if (IsKeyDown(KeyboardKey.A)) movementVector.X -= 1;
            if (IsKeyDown(KeyboardKey.S)) movementVector.Y += 1;
            if (IsKeyDown(KeyboardKey.D)) movementVector.X += 1;

            if (movementVector != Vector2.Zero) // avoid normalizing a zeroed vector as it results in division by 0
                movementVector = Vector2.Normalize(movementVector) * speed.Dx;
            vel = new Velocity(movementVector.X, movementVector.Y);

            /* collision */
            /* edit animation configuration */
            // flip character based on movement direction
            if (vel.Dx != 0 || vel.Dy != 0) // don't flip character when it is still
                anim.isFlippedHorizontal = vel.Dx < 0;
        });

        /* animation */
        var queryForAnim = new QueryDescription().WithAll<SourceRects, AnimationData, Velocity>();
        world.Query(in queryForAnim, (ref SourceRects source, ref AnimationData anim, ref Velocity vel) => {
            // Animate while moving
            if ((vel.Dx != 0 || vel.Dy != 0) && gameTime > (anim.lastFrameTime + anim.delay)) {
                source.frame = (source.frame + 1) % source.CurrentSet.Length;
                anim.lastFrameTime = gameTime;
            }
        });

        /* gravity */
        var queryGravity = new QueryDescription().WithAll<Velocity, Gravity>();
        world.Query(in queryGravity, (ref Velocity vel) => {
            vel.Dx += gravity.X;
            vel.Dy += gravity.Y;
        });

        /* apply velocities */
        var queryForVelocities = new QueryDescription().WithAll<Position, Velocity>();
        world.Query(in queryForVelocities, (ref Position pos, ref Velocity vel) => {
            pos.X += vel.Dx * deltaTime;
            pos.Y += vel.Dy * deltaTime;
        });

        /* update gameTime */
        gameTime = GetTime();
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
        world.Query(in queryToDrawSpritesheet, (Entity entity, ref Position pos, ref Texture2D texture, ref SourceRects source) => {
            Rectangle sourceRect = source.CurrentRect;
            // apply AnimationData.isFlippedHorizontal
            if (entity.Has<AnimationData>() && entity.Get<AnimationData>().isFlippedHorizontal)
                sourceRect.Width = -sourceRect.Width;

            DrawTextureRec(texture, sourceRect, pos.Vector2, Color.White);
        });

        if (drawHitboxes) {
            var queryToDrawHitboxes = new QueryDescription().WithAll<Position, HitboxRectangle>();
            world.Query(in queryToDrawHitboxes, (ref Position pos, ref HitboxRectangle hitbox) => {
                DrawRectangleLinesEx(new Rectangle(pos.X + hitbox.rect.X,
                                                   pos.Y + hitbox.rect.Y,
                                                   hitbox.rect.Width,
                                                   hitbox.rect.Height), 2, Color.Gray);
            });
        }

        EndMode2D();
    }
}

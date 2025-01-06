using Raylib_cs;
using static Raylib_cs.Raylib;
using Arch.Core;
using System.Numerics;

namespace Atlantis;

class Program
{
    static int RenderWidth = 640;
    static int RenderHeight = 360;

    private static void Update(World world)
    {
    }

    private static void Draw(World world)
    {
    }

    /* fullTextureSource:
     * Helper method to generate a sourceRectangle that includes the whole texture
     */
    private static Rectangle fullTextureSource(Texture2D texture)
    {
        return new Rectangle(0, 0, texture.Width,
                                   -texture.Height); // why is height negative? I have no idea
    }

    public static void Main()
    {
        SetConfigFlags(ConfigFlags.VSyncHint);
        InitWindow(1280, 720, "The Lost City Of Atlantis: The Kraken's Den");

        /* Initialization */
        Level testLevel = new MainLevel(RenderWidth, RenderHeight);

        /* Loading */
        RenderTexture2D target = LoadRenderTexture(RenderWidth, RenderHeight);

        while (!WindowShouldClose())
        {
            /* Update */
            if (IsKeyPressed(KeyboardKey.F)) ToggleBorderlessWindowed();
            testLevel.UpdateLevel();

            /* Draw */
            BeginDrawing();

            // We draw to a target before framebuffer
            // in order to get consistent scaling on all resolutions
            // Draw here directly for things that are relative to the screen (ie HUD, menus)
            BeginTextureMode(target);
            ClearBackground(Color.White);

            // This call uses the level's camera
            testLevel.DrawLevel();

            DrawText("Hello, world!", 12, 12, 20, Color.Black);

            EndTextureMode();

            // Draws target to screen size with letterboxing
            // Don't mess with any drawing code after this!
            float scale = MathF.Min(
                GetScreenWidth() / target.Texture.Width, // scale of target and screen on x direction
                GetScreenHeight() / target.Texture.Height // scale of target and screen on y direction
            );
            Rectangle gameScreenDestRec =
                new Rectangle(
                        (GetScreenWidth() - target.Texture.Width * scale) * 0.5f,
                        (GetScreenHeight() - target.Texture.Height * scale) * 0.5f,
                        target.Texture.Width * scale,
                        target.Texture.Height * scale);

            DrawTexturePro(target.Texture, fullTextureSource(target.Texture),
                           gameScreenDestRec, Vector2.Zero, 0f, Color.White);
            EndDrawing();
        }

        /* Unloads */
        UnloadRenderTexture(target);

        CloseWindow();
    }
}

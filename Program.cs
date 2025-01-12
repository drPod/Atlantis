using Raylib_cs;
using Arch.Core;
using static Raylib_cs.Raylib;
using System.Numerics;

namespace Atlantis;

class Program
{
    public static int RenderWidth = 640;
    public static int RenderHeight = 360;
    public static bool ShowFPS = true;
    public static bool ShowMousePosition = false;

    public static RenderTexture2D target;
    public static float scale;
    public static Rectangle screenDestRect;

    /* fullTextureSource:
     * Helper method to generate a sourceRectangle that includes the whole texture */
    private static Rectangle fullTextureSource(Texture2D texture)
    {
        return new Rectangle(0, 0, texture.Width,
                                   -texture.Height); // why is height negative? I have no idea
    }

    /* UpdateRenderScaling:
     * Will be called if window is resized
     */
    public static void UpdateRenderScaling()
    {
        scale = MathF.Min(
            GetScreenWidth() / target.Texture.Width, // scale of target and screen on x direction
            GetScreenHeight() / target.Texture.Height // scale of target and screen on y direction
        );
        screenDestRect =
            new Rectangle(
                    (GetScreenWidth() - target.Texture.Width * scale) * 0.5f,
                    (GetScreenHeight() - target.Texture.Height * scale) * 0.5f,
                    target.Texture.Width * scale,
                    target.Texture.Height * scale);
    }

    public static void Main(string[] args)
    {
        // Configure window
        //SetConfigFlags(ConfigFlags.VSyncHint);
        InitWindow(1280, 720, "The Lost City Of Atlantis: The Kraken's Den");
        //SetTargetFPS(GetMonitorRefreshRate(GetCurrentMonitor()));
        SetTargetFPS(60);

        /* Initialization */
        // Read command line arguments to load level
        IContentLoader contentLoader = new MainContentLoader();
        ILevel level;
        if (args.Length > 1)
        {
            Console.Error.WriteLine($"Format:\n\t{Environment.GetCommandLineArgs()[0]} [LEVEL]");
            Environment.Exit(1);
            return;
        }
        else if (args.Length == 0 || args[0] == "main")
            level = new MainLevel(RenderWidth, RenderHeight, contentLoader);
        else if (args[0] == "editor")
            level = new LevelEditor(RenderWidth, RenderHeight, contentLoader);
        else
        {
            Console.Error.WriteLine($"{Environment.GetCommandLineArgs()[0]}: invalid level");
            Environment.Exit(1);
            return;
        }

        /* Loading */
        target = LoadRenderTexture(RenderWidth, RenderHeight);
        UpdateRenderScaling();

        //World previousState = null;
        //Serializer stateLoader = new Serializer();
        //previousState = stateLoader.LoadGameState();

        while (!WindowShouldClose())
        {
            /* Update */
            if (IsKeyPressed(KeyboardKey.F))
                ToggleBorderlessWindowed();

            // Virtual mouse for resolution (clamped to RenderWidth and RenderHeight)
            Vector2 mouse = GetMousePosition();
            Vector2 virtualMouse = new Vector2(
                mouse.X / scale,
                mouse.Y / scale
            );
            Vector2 max = new((float)RenderWidth, (float)RenderHeight);
            virtualMouse = Vector2.Clamp(virtualMouse, Vector2.Zero, max);

            level.UpdateLevel(virtualMouse);

            /* Draw */
            BeginDrawing();
            // We draw to a target before framebuffer
            // in order to get consistent scaling on all resolutions
            // Draw here directly for things that are relative to the screen (ie HUD, menus)
            BeginTextureMode(target);
            ClearBackground(Color.White);

            // This call uses the level's camera
            level.DrawLevel();

            /* Debug Drawing */
            if (ShowFPS)
                DrawText($"FPS: {GetFPS()}", 12, 12, 20, Color.Black);
            if (ShowMousePosition)
            {
                DrawText($"Default Mouse: [{(int)mouse.X} {(int)mouse.Y}]", 350, 12, 20, Color.Green);
                DrawText($"Virtual Mouse: [{(int)virtualMouse.X}, {(int)virtualMouse.Y}]", 350, 42, 20, Color.Yellow);
            }

            EndTextureMode();

            if (IsWindowResized())
                UpdateRenderScaling();
            // Draws target to screen size with letterboxing
            // Don't mess with any drawing code after this!
            ClearBackground(Color.Black);
            DrawTexturePro(target.Texture, fullTextureSource(target.Texture),
                           screenDestRect, Vector2.Zero, 0f, Color.White);
            EndDrawing();
        }

        /* Unloads */
        //Serializer stateSaver = new Serializer();
        //World state = ((Level)level).LevelWorld;

        //stateSaver.SaveGameState(state);

        UnloadRenderTexture(target);

        CloseWindow();
    }
}

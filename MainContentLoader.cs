using Raylib_cs;
using static Raylib_cs.Raylib;
using Arch.Core;
using Arch.Persistence;
using System.Collections;

namespace Atlantis;

/* MainContentLoader is responsible for loading in content
 * like textures and positions and audio into a world
 */
class MainContentLoader : IContentLoader
{
    string filename;

    public MainContentLoader(string filename)
    {
        this.filename = filename;
    }

    public MainContentLoader()
    {
        filename = String.Empty;
    }

    /* HitboxFromRectangle: A method for automatically
     * generating a basic rectangular hitbox from a rectangle,
     */
    private HitboxRectangle HitboxFromRectangle(Rectangle rect)
    {
        return new HitboxRectangle(
            new Rectangle(
                rect.Width / 5f,
                rect.Height / 5f,
                3 * rect.Width / 5f,
                3 * rect.Height / 5f
                ));
    }

    private Rectangle[] FromSpritesheet(Image image, int numSprites)
    {
        Rectangle[] rects = new Rectangle[numSprites];
        int spriteWidth = image.Width / numSprites;
        for (int i = 0; i < numSprites; i++) {
            Rectangle spriteRect = new Rectangle(spriteWidth * i, 0, spriteWidth, image.Height);
            Image spriteImage = ImageFromImage(image, spriteRect);
            Rectangle spriteAlphaBorder = GetImageAlphaBorder(spriteImage, 0.1f);
            rects[i] = new Rectangle(spriteRect.X + spriteAlphaBorder.X,
                                     spriteRect.Y + spriteAlphaBorder.Y,
                                     spriteAlphaBorder.Width, spriteAlphaBorder.Height);
        }
        return rects;
    }

    private SourceRects LoadFromSpritesheetFile(List<string> filenames, List<string> spritenames, int numSprites)
    {
        Dictionary<String, Rectangle[]> sprites = new();
        for (int i = 0; i < filenames.Count; i++) {
            Image image = LoadImage(filenames[i]);
            sprites.Add(spritenames[i], FromSpritesheet(image, numSprites));
        }
        return new SourceRects(sprites, spritenames[0], 0);
    }

    public void LoadContentIntoWorld(World world)
    {
        if (filename == String.Empty) {
            Image playerImage = LoadImage("./assets/underwater-diving-files/PNG/player/player-swiming.png");
            Dictionary<String, Rectangle[]> playerSprites = new()
            {
                { "swiming", FromSpritesheet(playerImage, 7) }
            };
            SourceRects playerSource = new SourceRects(playerSprites, "swiming", 0);
            /* world.Create(new Player(), new Gravity(), new Position(0, 0), new Velocity(0, 50), new Speed(50), */
            /*         LoadTextureFromImage(playerImage), playerSource, new AnimationData(0.1, 0, false), HitboxFromRectangle(playerSource.CurrentRect)); */

            /* ui */
            world.Create(new Player(), new Gravity(), new UIPos(0, 0), new Velocity(), new Speed(50),
                    LoadTextureFromImage(playerImage), playerSource, new AnimationData(0.1, 0, false), HitboxFromRectangle(playerSource.CurrentRect));
            world.Create(new Fish(), new UIPos(30, 0), new Velocity(), new Speed(20),
                    LoadTextureFromImage(), playerSource, new AnimationData(0.1, 0, false), HitboxFromRectangle(playerSource.CurrentRect));
        } else {
            // TODO: implement method to load specific saved levels from file
        }
    }

    public void Dispose()
    {
        // TODO: Dispose of textures and other assets
    }
}

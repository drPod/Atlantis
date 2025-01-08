using Raylib_cs;
using static Raylib_cs.Raylib;
using Arch.Core;
using Arch.Persistence;

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

    /* HitboxFromTexture: A method for automatically
     * generating a basic rectangular hitbox from a Texture,
     */
    private HitboxRectangle HitboxFromImage(Texture2D texture)
    {
        return new HitboxRectangle(
            new Rectangle(
                texture.Width / 5f,
                texture.Height / 5f,
                3 * texture.Width / 5f,
                3 * texture.Height / 5f
                ));
    }

    private Texture2D CropImageAndMakeTexture(ref Image image)
    {
        // Garbage collector do your thing
        image = ImageFromImage(image, GetImageAlphaBorder(image, 0.1f));
        return LoadTextureFromImage(image);
    }

    public void LoadContentIntoWorld(World world)
    {
        if (filename == String.Empty) {
            /* for testing */
            //Image playerImageUncropped = LoadImage("./assets/devel/submarine.png");
            //Image playerImage = ImageFromImage(playerImageUncropped, GetImageAlphaBorder(playerImageUncropped, 0.1f));
            //Texture2D playerTexture = LoadTextureFromImage(playerImage);
            Image playerImage = LoadImage("./assets/devel/submarine.png");
            Texture2D playerTexture = CropImageAndMakeTexture(ref playerImage);
            world.Create(new Player(), new Position(0, 0), new Velocity(0, 50),
                    playerTexture, HitboxFromImage(playerTexture));

            world.Create(LoadTexture("./assets/devel/rotate.png"), new Icon(IconTypes.Rotate));
        } else {
            // TODO: implement method to load specific saved levels from file
        }
    }

    public void Dispose()
    {
        // TODO: Dispose of textures and other assets
    }
}

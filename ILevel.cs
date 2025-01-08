using Raylib_cs;
using System.Numerics;

namespace Atlantis;

interface ILevel
{
    void UpdateLevel(Vector2 virtualMousePos);

    void DrawLevel();
}

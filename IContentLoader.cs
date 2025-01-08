using Raylib_cs;
using static Raylib_cs.Raylib;
using Arch.Core;
using System.Numerics;

namespace Atlantis;

interface IContentLoader : IDisposable
{
    public void LoadContentIntoWorld(World world);
}

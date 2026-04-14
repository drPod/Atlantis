# Atlantis

A 2D underwater game prototype. C# on Raylib with an Arch ECS backbone, Box2D for physics, and SQLite for level serialization.

Written to learn how modern game loops are actually wired — letterboxed render target, virtual-resolution scaling, spritesheet animation, a draggable in-game level editor, and save/load round-trips through SQLite.

## Stack

- **Raylib-cs** — rendering, input, window
- **Arch** — high-performance ECS
- **Box2D.NET** — physics
- **Microsoft.Data.Sqlite** — level state persistence
- **.NET 6**

## Run

```bash
dotnet run
```

A `flake.nix` is provided for Nix users.

## Layout

- `Program.cs` — entry point, game state machine (menu / running), render scaling
- `MainLevel.cs` — player movement, animation, ECS queries
- `LevelEditor.cs` — drag-and-drop level editor
- `Components.cs` — ECS component definitions (`Position`, `Velocity`, `AnimationData`, etc.)
- `Serializer.cs` — SQLite-backed level serialization
- `MainContentLoader.cs` — sprite/animation asset loading

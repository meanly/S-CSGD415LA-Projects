using Raylib_cs;
using System.Numerics;

namespace Antiquera_LabActivity1_Finals;

public enum Direction
{
    Down = 0,
    Left = 1,
    Right = 2,
    Up = 3
}

public enum PlayerState
{
    Idle,
    Walking,
    Running,
    AttackingMelee,
    AttackingRanged
}

class Program
{
    static void Main()
    {
        const int screenWidth = 1280;
        const int screenHeight = 720;
        const int tileSize = 256;

        Raylib.InitWindow(screenWidth, screenHeight, "RPG Tile-Based Game");
        Raylib.SetTargetFPS(60);

        // Initialize game
        Game game = new Game(screenWidth, screenHeight, tileSize);
        game.Initialize();

        // Main game loop
        while (!Raylib.WindowShouldClose())
        {
            float deltaTime = Raylib.GetFrameTime();

            game.Update(deltaTime);
            game.Draw();
        }

        game.Cleanup();
        Raylib.CloseWindow();
    }
}

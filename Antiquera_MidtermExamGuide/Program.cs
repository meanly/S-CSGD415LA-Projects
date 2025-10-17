using Raylib_cs;
using System;

namespace MemoryGame
{
    class Program
    {
        static void Main()
        {
            Raylib.InitWindow(800, 600, "Memory Game");
            Raylib.SetTargetFPS(60);

            Game game = new Game();

            while (!Raylib.WindowShouldClose())
            {
                game.Update();
                
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.White);
                game.Draw();
                Raylib.EndDrawing();
            }

            Raylib.CloseWindow();
        }
    }
}
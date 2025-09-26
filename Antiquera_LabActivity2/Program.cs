using Raylib_cs;
using System.Drawing;
using System.Numerics;

public struct windowSize
{
    public int width;
    public int height;
}
class Program
{
    static void Main()
    {
        windowSize gameSize = new windowSize { width = 1280, height = 720 };
        Raylib.InitWindow(gameSize.width, gameSize.height, "Insaniquarium Clone");
        Raylib.SetTargetFPS(60);

        GameHandler game = new GameHandler(gameSize.width, gameSize.height);

        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Raylib_cs.Color.White);

            game.Update();

            Raylib.EndDrawing();
        }
        Raylib.CloseWindow();
    }
}
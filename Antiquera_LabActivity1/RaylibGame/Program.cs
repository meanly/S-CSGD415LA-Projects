using Raylib_cs;

class Program
{
    static void Main()
    {
        Raylib.InitWindow(800, 600, "My First Raylib.NET Game");
        Raylib.SetTargetFPS(60);

        Game game = new Game();

        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);

            game.Update();
            game.Draw();

            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }
}

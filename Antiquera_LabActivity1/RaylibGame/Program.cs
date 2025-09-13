using Raylib_cs;

class Program
{
    static void Main()
    {
        int screenWidth = 1280;  // change width
        int screenHeight = 720;  // change height

        Raylib.InitWindow(screenWidth, screenHeight, "Jeremy's Game");
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

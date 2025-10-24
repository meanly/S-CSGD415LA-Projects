using Raylib_cs;

namespace MemoryGame
{
    public static class Game
    {
        private static GameManager? gameManager;
        private const int SCREEN_WIDTH = 960;
        private const int SCREEN_HEIGHT = 640;
        private const int GRID_COLS = 7;
        private const int GRID_ROWS = 4;

        public static void Run()
        {
            Raylib.InitWindow(SCREEN_WIDTH, SCREEN_HEIGHT, "Memory / Concentration Game");
            Raylib.SetTargetFPS(60);

            gameManager = new GameManager(SCREEN_WIDTH, SCREEN_HEIGHT, GRID_COLS, GRID_ROWS);
            gameManager.LoadResources();
            gameManager.StartNewGame();

            while (!Raylib.WindowShouldClose())
            {
                float dt = Raylib.GetFrameTime();

                gameManager.Update(dt);

                Raylib.BeginDrawing();
                gameManager.Draw();
                Raylib.EndDrawing();
            }

            gameManager.UnloadResources();
            Raylib.CloseWindow();
        }
    }
}
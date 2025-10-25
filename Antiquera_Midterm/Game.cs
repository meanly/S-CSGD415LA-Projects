using Raylib_cs;

namespace MemoryGame
{
    public static class Game
    {
        private static GameManager? gameManager;
        private const int SCREEN_WIDTH = 1480;
        private const int SCREEN_HEIGHT = 900;
        private const int GRID_COLS = 7;
        private const int GRID_ROWS = 4;

        public static void Run()
        {
            // Initialize window
            Raylib.InitWindow(SCREEN_WIDTH, SCREEN_HEIGHT, "Memory Game");
            Raylib.SetTargetFPS(60);

            // Create game manager
            gameManager = new GameManager(SCREEN_WIDTH, SCREEN_HEIGHT, GRID_COLS, GRID_ROWS);
            gameManager.LoadResources();
            gameManager.StartNewGame();

            // Main game loop
            while (!Raylib.WindowShouldClose())
            {
                float dt = Raylib.GetFrameTime();
                gameManager.Update(dt);

                // Draw everything
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.Black);

                TextureHandler.DrawBackground();
                gameManager.Draw();
                TextureHandler.DrawScreenOverlay(gameManager.GetGameState());

                Raylib.EndDrawing();
            }

            // Cleanup
            gameManager.UnloadResources();
            Raylib.CloseWindow();
        }
    }
}
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
            Raylib.InitWindow(SCREEN_WIDTH, SCREEN_HEIGHT, "Memory / Concentration Game");
            Raylib.SetTargetFPS(60);

            // Load all textures first
            TextureHandler.LoadAllTextures();

            gameManager = new GameManager(SCREEN_WIDTH, SCREEN_HEIGHT, GRID_COLS, GRID_ROWS);
            gameManager.LoadResources();
            gameManager.StartNewGame();

            while (!Raylib.WindowShouldClose())
            {
                float dt = Raylib.GetFrameTime();

                gameManager.Update(dt);

                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.Black);

                // Draw background
                TextureHandler.DrawBackground();

                // Draw game content
                gameManager.Draw();

                // Draw screen overlays based on game state
                TextureHandler.DrawScreenOverlay(gameManager.GetGameState());

                Raylib.EndDrawing();
            }

            gameManager.UnloadResources();
            TextureHandler.UnloadAllTextures();
            Raylib.CloseWindow();
        }
    }
}
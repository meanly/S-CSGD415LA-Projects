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
<<<<<<< HEAD
            // Initialize window
            Raylib.InitWindow(SCREEN_WIDTH, SCREEN_HEIGHT, "Memory Game");
            Raylib.SetTargetFPS(60);

            // Create game manager
=======
            Raylib.InitWindow(SCREEN_WIDTH, SCREEN_HEIGHT, "Antiquera Midterm Exam");
            Raylib.SetTargetFPS(60);

            // Load all textures first
            TextureHandler.LoadAllTextures();

>>>>>>> e529e2be160b9e2c672146ba7a232c99408d5735
            gameManager = new GameManager(SCREEN_WIDTH, SCREEN_HEIGHT, GRID_COLS, GRID_ROWS);
            gameManager.LoadResources();
            gameManager.StartNewGame();

<<<<<<< HEAD
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
=======
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
>>>>>>> e529e2be160b9e2c672146ba7a232c99408d5735
                TextureHandler.DrawScreenOverlay(gameManager.GetGameState());

                Raylib.EndDrawing();
            }

<<<<<<< HEAD
            // Cleanup
            gameManager.UnloadResources();
=======
>>>>>>> e529e2be160b9e2c672146ba7a232c99408d5735
            Raylib.CloseWindow();
        }
    }
}
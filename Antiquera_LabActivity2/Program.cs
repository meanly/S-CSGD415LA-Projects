using Raylib_cs;
using System.Drawing;
using System.Numerics;
using System.Linq;

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
        Raylib.InitWindow(gameSize.width, gameSize.height, "Pororium");
        Raylib.SetTargetFPS(60);

        MainMenu mainMenu = new MainMenu(gameSize.width, gameSize.height);
        AboutScreen aboutScreen = new AboutScreen(gameSize.width, gameSize.height);
        GameHandler game = null;
        float restartTimer = 0f; // Timer to prevent immediate game over after restart

        // Win screen fade animation
        float winFadeTimer = 0f;
        float winFadeDuration = 2.0f; // 2 seconds fade-in

        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();

            // Handle different game states
            if (GameState.GetState() == GameState.State.MainMenu)
            {
                mainMenu.Update();
                mainMenu.Draw();
            }
            else if (GameState.GetState() == GameState.State.About)
            {
                aboutScreen.Update();
                aboutScreen.Draw();
            }
            else if (GameState.GetState() == GameState.State.Playing)
            {
                // Initialize game if not already done
                if (game == null)
                {
                    game = new GameHandler(gameSize.width, gameSize.height);
                }

                Raylib.ClearBackground(Raylib_cs.Color.White);
                game.Update();

                // Check for win condition (PoroKing exists) - but not during animation
                if (game != null && game.fishes.Any(f => f is PoroKing) && !game.IsAnimatingToPoroKing)
                {
                    GameState.SetState(GameState.State.Win);
                }
                // Check for game over and return to main menu (with delay after restart)
                else if (game != null && restartTimer <= 0 && (game.money <= 0 || game.fishes.Count == 0))
                {
                    Console.WriteLine($"Game over detected - Money: {game.money}, Fish count: {game.fishes.Count}");
                    GameState.SetState(GameState.State.GameOver);
                }
            }
            else if (GameState.GetState() == GameState.State.Win)
            {
                // Update fade-in timer
                winFadeTimer += Raylib.GetFrameTime();
                float fadeProgress = Math.Min(1.0f, winFadeTimer / winFadeDuration);

                // Calculate fade alpha (0 to 1)
                float fadeAlpha = (float)(1.0 - Math.Pow(1.0 - fadeProgress, 3)); // Ease out cubic

                Raylib.ClearBackground(Raylib_cs.Color.DarkGreen);

                // Draw win screen with fade-in effect
                string winText = "YOU WIN!";
                int textWidth = Raylib.MeasureText(winText, 60);
                int winTextAlpha = (int)(fadeAlpha * 255);
                Raylib.DrawText(winText,
                    (gameSize.width - textWidth) / 2,
                    gameSize.height / 2 - 50,
                    60,
                    new Raylib_cs.Color(255, 215, 0, winTextAlpha)); // Gold with fade

                string winSubText = "You found the Poro King!";
                int subWidth = Raylib.MeasureText(winSubText, 30);
                Raylib.DrawText(winSubText,
                    (gameSize.width - subWidth) / 2,
                    gameSize.height / 2 + 20,
                    30,
                    new Raylib_cs.Color(255, 255, 255, winTextAlpha)); // White with fade

                string restartText = "Press ESC to Main Menu";
                int restartWidth = Raylib.MeasureText(restartText, 24);
                Raylib.DrawText(restartText,
                    (gameSize.width - restartWidth) / 2,
                    gameSize.height / 2 + 70,
                    24,
                    new Raylib_cs.Color(211, 211, 211, winTextAlpha)); // LightGray with fade

                // Handle input
                if (Raylib.IsKeyPressed(KeyboardKey.Escape))
                {
                    game = null;
                    winFadeTimer = 0f; // Reset fade timer
                    GameState.SetState(GameState.State.MainMenu);
                }
            }
            else if (GameState.GetState() == GameState.State.GameOver)
            {
                Raylib.ClearBackground(Raylib_cs.Color.Black);

                // Draw game over screen
                string gameOverText = "GAME OVER!";
                int textWidth = Raylib.MeasureText(gameOverText, 60);
                Raylib.DrawText(gameOverText,
                    (gameSize.width - textWidth) / 2,
                    gameSize.height / 2 - 50,
                    60,
                    Raylib_cs.Color.Red);

                string restartText = "Press R to Restart or ESC to Main Menu";
                int restartWidth = Raylib.MeasureText(restartText, 30);
                Raylib.DrawText(restartText,
                    (gameSize.width - restartWidth) / 2,
                    gameSize.height / 2 + 20,
                    30,
                    Raylib_cs.Color.White);

                // Handle restart input
                if (Raylib.IsKeyPressed(KeyboardKey.R))
                {
                    Console.WriteLine("R key pressed - restarting game...");
                    game = new GameHandler(gameSize.width, gameSize.height);
                    GameState.SetState(GameState.State.Playing);
                    restartTimer = 2f; // Give 2 seconds before checking for game over
                    Console.WriteLine($"Game restarted! Initial state - Money: {game.money}, Fish count: {game.fishes.Count}");
                }
                else if (Raylib.IsKeyPressed(KeyboardKey.Escape))
                {
                    game = null; // Reset game
                    GameState.SetState(GameState.State.MainMenu);
                }
            }

            Raylib.EndDrawing();
        }

        // Clean up resources
        mainMenu.Dispose();
        Raylib.CloseWindow();
    }
}
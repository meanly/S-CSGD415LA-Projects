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

        MainMenu mainMenu = new MainMenu(gameSize.width, gameSize.height);
        AboutScreen aboutScreen = new AboutScreen(gameSize.width, gameSize.height);
        GameHandler game = null;

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

                // Check for game over and return to main menu
                if (game.money <= 0 || game.fishes.Count == 0)
                {
                    GameState.SetState(GameState.State.GameOver);
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
                    game = new GameHandler(gameSize.width, gameSize.height);
                    GameState.SetState(GameState.State.Playing);
                }
                else if (Raylib.IsKeyPressed(KeyboardKey.Escape))
                {
                    game = null; // Reset game
                    GameState.SetState(GameState.State.MainMenu);
                }
            }

            Raylib.EndDrawing();
        }
        Raylib.CloseWindow();
    }
}
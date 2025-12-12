using Finals_SpaceFlap.GameObjects;
using Raylib_cs;

namespace Finals_SpaceFlap.Systems;

public class GameManager
{
    private Spaceship? spaceship;
    private AsteroidManager? asteroidManager;
    private ParallaxBackground? background;
    private int score;
    private bool isGameOver;
    private bool isGameStarted;
    private readonly int screenWidth;
    private readonly int screenHeight;

    public GameManager(int screenWidth, int screenHeight)
    {
        this.screenWidth = screenWidth;
        this.screenHeight = screenHeight;
        ResetGame();
    }

    public void Update(float deltaTime)
    {
        // Update background (always scrolling)
        background?.Update(deltaTime);

        if (!isGameStarted)
        {
            if (Raylib.IsKeyPressed(KeyboardKey.Space) || Raylib.IsKeyPressed(KeyboardKey.Up))
            {
                isGameStarted = true;
            }
            return;
        }

        if (isGameOver)
        {
            if (Raylib.IsKeyPressed(KeyboardKey.R))
            {
                ResetGame();
            }
            return;
        }

        // Update spaceship
        spaceship?.Update(deltaTime);

        // Update asteroids
        asteroidManager?.Update(deltaTime, spaceship?.Position.X ?? 0);

        // Check collisions
        if (spaceship != null && asteroidManager != null)
        {
            foreach (var asteroid in asteroidManager.GetAsteroids())
            {
                if (spaceship.CheckCollision(asteroid))
                {
                    isGameOver = true;
                    break;
                }
            }

            // Check if spaceship hit screen bounds
            if (spaceship.Position.Y + spaceship.GetBounds().Height > screenHeight)
            {
                isGameOver = true;
            }

            // Update score
            score += asteroidManager.CheckScore(spaceship.Position.X);
        }
    }

    public void Render()
    {
        // Draw background first (parallax scrolling)
        background?.Render();

        // Draw game objects
        spaceship?.Render();
        asteroidManager?.Render();

        // Draw UI
        if (!isGameStarted)
        {
            string startText = "Press SPACE to Start";
            int textWidth = Raylib.MeasureText(startText, 30);
            Raylib.DrawText(startText, (screenWidth - textWidth) / 2, screenHeight / 2, 30, Color.White);
        }
        else if (isGameOver)
        {
            string gameOverText = "Game Over!";
            string scoreText = $"Score: {score}";
            string restartText = "Press R to Restart";

            int gameOverWidth = Raylib.MeasureText(gameOverText, 40);
            int scoreWidth = Raylib.MeasureText(scoreText, 30);
            int restartWidth = Raylib.MeasureText(restartText, 25);

            Raylib.DrawText(gameOverText, (screenWidth - gameOverWidth) / 2, screenHeight / 2 - 60, 40, Color.Red);
            Raylib.DrawText(scoreText, (screenWidth - scoreWidth) / 2, screenHeight / 2 - 20, 30, Color.White);
            Raylib.DrawText(restartText, (screenWidth - restartWidth) / 2, screenHeight / 2 + 20, 25, Color.Yellow);
        }
        else
        {
            // Draw score during gameplay
            Raylib.DrawText($"Score: {score}", 10, 10, 30, Color.White);
        }
    }

    private void ResetGame()
    {
        score = 0;
        isGameOver = false;
        isGameStarted = false;
        spaceship = new Spaceship(100, screenHeight / 2);
        asteroidManager = new AsteroidManager(screenWidth, screenHeight);
        background = new ParallaxBackground(screenWidth, screenHeight);
    }
}

using System;
using Raylib_cs;
using System.Numerics;

namespace MemoryGame
{
    public enum GameState
    {
        Loading,
        Playing,
        Paused,
        Victory,
        GameOver
    }

    public class GameManager
    {
        private readonly int screenW;
        private readonly int screenH;
        private readonly TileManager tileManager;

        // Game variables
        private GameState gameState = GameState.Playing;
        private int health = 8;
        private int score = 0;
        private float timer = 8.0f;
        private float previewTime = 3.0f;
        private bool previewDone = false;

        public GameManager(int width, int height, int columns, int rows)
        {
            screenW = width;
            screenH = height;
            tileManager = new TileManager(columns, rows);
        }

        public void LoadResources()
        {
            gameState = GameState.Loading;
        }

        public void UnloadResources()
        {
            // Nothing to unload
        }

        public void StartNewGame()
        {
            health = 8;
            score = 0;
            timer = 8.0f;
            gameState = GameState.Playing;
            previewDone = false;
            previewTime = 3.0f;
            tileManager.CreateTiles(screenW, screenH);
        }

        public void Update(float dt)
        {
            // Handle different game states
            if (gameState == GameState.Loading)
            {
                if (Raylib.IsKeyPressed((KeyboardKey)'X'))
                {
                    gameState = GameState.Playing;
                    previewDone = false;
                }
                return;
            }

            if (gameState == GameState.Paused)
            {
                if (Raylib.IsKeyPressed((KeyboardKey)'P')) gameState = GameState.Playing;
                return;
            }

            if (gameState == GameState.Victory || gameState == GameState.GameOver)
            {
                if (Raylib.IsKeyPressed((KeyboardKey)'R')) StartNewGame();
                return;
            }

            // Show all tiles for preview
            if (!previewDone)
            {
                previewTime -= dt;
                tileManager.ShowAllTiles();
                if (previewTime <= 0f)
                {
                    tileManager.HideAllTiles();
                    previewDone = true;
                    timer = 8.0f;
                }
                return;
            }

            // Update game logic
            tileManager.UpdateMismatchTimer(dt);
            UpdateTimer(dt);
            HandleInput();
        }

        private void UpdateTimer(float dt)
        {
            timer -= dt;
            if (timer <= 0f)
            {
                health--;
                timer = 8.0f;
                tileManager.ResetRevealedTiles();
                if (health <= 0) gameState = GameState.GameOver;
            }
        }

        private void HandleInput()
        {
            var mousePos = Raylib.GetMousePosition();
            tileManager.UpdateHover(mousePos);

            // Pause game
            if (Raylib.IsKeyPressed((KeyboardKey)'P'))
            {
                gameState = GameState.Paused;
                return;
            }

            // Handle tile clicks
            if (Raylib.IsMouseButtonPressed(0))
            {
                bool tileClicked = tileManager.HandleTileClick(
                    mousePos,
                    () => // On match
                    {
                        score += 150;
                        if (tileManager.CheckVictory()) gameState = GameState.Victory;
                    },
                    () => // On mismatch
                    {
                        health--;
                        if (health <= 0) gameState = GameState.GameOver;
                    }
                );

                if (tileClicked) timer = 8.0f;
            }
        }

        public void Draw()
        {
            if (gameState == GameState.Loading)
            {
                DrawLoadingScreen();
            }
            else
            {
                DrawBackground();
                DrawGUI();
                DrawTiles();
                DrawStateOverlay();
            }
        }

        private void DrawLoadingScreen()
        {
            Raylib.DrawRectangleGradientV(0, 0, screenW, screenH,
                new Color(135, 206, 235, 255), new Color(25, 25, 112, 255));

            string msg = "Memory Game - Press X to Start";
            Raylib.DrawText(msg, screenW / 2 - Raylib.MeasureText(msg, 36) / 2,
                          screenH / 2 - 20, 36, Palette.White);
        }

        private void DrawBackground()
        {
            Raylib.DrawRectangleGradientV(0, 0, screenW, screenH,
                new Color(135, 206, 235, 255), new Color(25, 25, 112, 255));
        }

        private void DrawGUI()
        {
            // Title and score
            Raylib.DrawText("Memory Game", 20, 10, 20, Palette.White);
            Raylib.DrawText($"Score: {score}", screenW - 200, 10, 20, Palette.White);

            // Health hearts
            for (int i = 0; i < health; i++)
            {
                Raylib.DrawRectangle(20 + i * 36, 40, 28, 28, Palette.Red);
                Raylib.DrawRectangleLines(20 + i * 36, 40, 28, 28, Palette.White);
            }
            Raylib.DrawText("HP", 20, 72, 12, Palette.White);

            // Timer bar
            float barW = 300;
            float progress = Math.Max(0f, Math.Min(1f, timer / 8.0f));
            int barX = (int)(screenW / 2 - barW / 2 + 50);
            int barY = 40;

            Raylib.DrawRectangle(barX, barY, (int)barW, 20, Palette.LightGray);
            Raylib.DrawRectangle(barX, barY, (int)(barW * progress), 20, Palette.SkyBlue);
            Raylib.DrawRectangleLines(barX, barY, (int)barW, 20, Palette.White);
            Raylib.DrawText($"Time: {timer:0.0}s", screenW / 2 - 30 + 50, 66, 12, Palette.White);

            // Instructions
            Raylib.DrawText("Click tiles to flip. P to Pause. R to Restart.",
                20, screenH - 30, 14, Palette.LightGray);
        }

        private void DrawTiles()
        {
            var tiles = tileManager.GetTiles();
            int hoverIndex = tileManager.GetLastHoverIndex();

            for (int i = 0; i < tiles.Count; i++)
            {
                bool isHovered = (i == hoverIndex);
                TextureHandler.DrawTile(tiles[i], isHovered);
            }
        }

        private void DrawStateOverlay()
        {
            if (gameState == GameState.Paused)
            {
                Raylib.DrawRectangle(0, 0, screenW, screenH, new Color(0, 0, 0, 120));
                Raylib.DrawText("PAUSED", screenW / 2 - 60, screenH / 2 - 20, 40, Palette.White);
            }

            if (gameState == GameState.Victory)
            {
                Raylib.DrawRectangle(0, 0, screenW, screenH, new Color(0, 0, 0, 150));
                string msg = "VICTORY! All tiles matched.";
                Raylib.DrawText(msg, screenW / 2 - Raylib.MeasureText(msg, 28) / 2,
                    screenH / 2 - 40, 28, Palette.Gold);
                Raylib.DrawText("Press R to play again", screenW / 2 - 100, screenH / 2 + 4, 20, Palette.White);
            }

            if (gameState == GameState.GameOver)
            {
                Raylib.DrawRectangle(0, 0, screenW, screenH, new Color(0, 0, 0, 150));
                string msg = "GAME OVER";
                Raylib.DrawText(msg, screenW / 2 - Raylib.MeasureText(msg, 44) / 2,
                    screenH / 2 - 40, 44, Palette.Red);
                string sub = $"Score: {score} - Press R to try again";
                Raylib.DrawText(sub, screenW / 2 - Raylib.MeasureText(sub, 20) / 2,
                    screenH / 2 + 12, 20, Palette.White);
            }
        }

        public GameState GetGameState() => gameState;
    }
}
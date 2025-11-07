using System;
using System.IO;
using System.Numerics;
using Raylib_cs;

namespace MemoryGame
{
    public enum GameState
    {
        Loading,
        Playing,
        Paused,
        Victory,
        GameOver,
    }

    public class GameManager
    {
        private readonly int screenW;
        private readonly int screenH;
        private readonly TileManager tileManager;

        // Game state
        private GameState gameState = GameState.Playing;
        private int health;
        private readonly int healthMax = 8;
        private int score = 0;
        private float globalTimer;
        private readonly float globalTimerMax = 8.0f;
        private float showAllAtStartSec = 3.0f;
        private bool usedPreview = false;
        private float loadingTimer = 0f;
        private readonly float loadingDuration = 3.0f;

        // Resources
        private Texture2D tileNormalTex;
        private Texture2D tileHoverTex;
        private bool texturesLoaded = false;

        private Texture2D iconHealthTex;
        private Texture2D iconTimerTex;
        private bool iconsLoaded = false;

        private Texture2D[] pairTextures = Array.Empty<Texture2D>();
        private bool pairTexturesLoaded = false;

        private Texture2D backgroundTex;
        private bool backgroundLoaded = false;

        private Texture2D loadingScreenTex;
        private bool loadingScreenLoaded = false;

        public GameManager(int width, int height, int columns, int rows)
        {
            screenW = width;
            screenH = height;
            tileManager = new TileManager(columns, rows);
            health = healthMax;
            globalTimer = globalTimerMax;
        }

        public void LoadResources()
        {
            LoadTextures();
            LoadIcons();
            LoadBackground();
            LoadLoadingScreen();
        }

        private void LoadTextures()
        {
            texturesLoaded = true;
        }

        private void LoadIcons()
        {
            string iconPath = Path.Combine(Directory.GetCurrentDirectory(), "img");
            if (Directory.Exists(iconPath))
            {
                iconsLoaded = (iconHealthTex.Width > 0 && iconTimerTex.Width > 0);
            }
            else
                iconsLoaded = false;
        }

        private void LoadBackground()
        {
            backgroundLoaded = true;
        }

        private void LoadLoadingScreen()
        {
            loadingScreenLoaded = true;
            gameState = GameState.Loading;
            loadingTimer = 0f;
        }


        public void StartNewGame()
        {
            health = healthMax;
            score = 0;
            globalTimer = globalTimerMax;
            gameState = GameState.Playing;
            usedPreview = false;
            showAllAtStartSec = 3.0f;
            tileManager.CreateTiles(screenW, screenH);
        }

        public void Update(float dt)
        {
            if (gameState == GameState.Loading)
            {
                HandleLoadingState(dt);
                return;
            }

            if (gameState == GameState.Paused)
            {
                if (Raylib.IsKeyPressed((KeyboardKey)'P'))
                    gameState = GameState.Playing;
                return;
            }

            if (gameState == GameState.Victory || gameState == GameState.GameOver)
            {
                if (Raylib.IsKeyPressed((KeyboardKey)'R'))
                    StartNewGame();
                return;
            }

            if (!usedPreview)
            {
                HandlePreview(dt);
                return;
            }

            tileManager.UpdateMismatchTimer(dt);
            UpdateTimer(dt);
            HandleInput();
        }

        private void HandleLoadingState(float dt)
        {
            loadingTimer += dt;
            if (loadingTimer >= loadingDuration || Raylib.IsKeyPressed((KeyboardKey)'X'))
            {
                gameState = GameState.Playing;
                usedPreview = false;
                showAllAtStartSec = 10.0f;
            }
        }

        private void HandlePreview(float dt)
        {
            showAllAtStartSec -= dt;
            tileManager.ShowAllTiles();
            if (showAllAtStartSec <= 0f)
            {
                tileManager.HideAllTiles();
                usedPreview = true;
                globalTimer = globalTimerMax;
            }
        }

        private void UpdateTimer(float dt)
        {
            globalTimer -= dt;
            if (globalTimer <= 0f)
            {
                health--;
                globalTimer = globalTimerMax;
                tileManager.ResetRevealedTiles();

                if (health <= 0)
                {
                    gameState = GameState.GameOver;
                }
            }
        }

        private void HandleInput()
        {
            var mousePos = Raylib.GetMousePosition();
            int newHoverIndex = tileManager.UpdateHover(mousePos);

            // Handle pause
            if (Raylib.IsKeyPressed((KeyboardKey)'P'))
            {
                gameState = GameState.Paused;
                return;
            }

            // Handle tile click
            if (Raylib.IsMouseButtonPressed(0))
            {
                bool tileClicked = tileManager.HandleTileClick(
                    mousePos,
                    () =>
                    {
                        int basePoints = 100;
                        int bonus = 50;
                        score += basePoints + bonus;
                        if (tileManager.CheckVictory())
                            gameState = GameState.Victory;
                    },
                    () =>
                    {
                        health--;
                        if (health <= 0)
                            gameState = GameState.GameOver;
                    }
                );

                if (tileClicked)
                {
                    globalTimer = globalTimerMax;
                }
            }
        }

        public void Draw()
        {
            Raylib.ClearBackground(Palette.RayWhite);
            DrawGUI();
            DrawTiles();
            DrawStateOverlay();
        } 


        private void DrawGUI()
        {
            // Header
            Raylib.DrawText("Memory Game (Raylib-cs)", 20, 10, 20, Palette.DarkBlue);
            Raylib.DrawText($"Score: {score}", screenW - 200, 10, 20, Palette.Black);

            // Health
            int hx = 20;
            int iconSize = iconsLoaded ? 32 : 28;
            if (iconsLoaded)
            {
                for (int i = 0; i < health; i++)
                {
                    var dst = new Rectangle(hx + i * (iconSize + 6), 36, iconSize, iconSize);
                    Raylib.DrawTexturePro(
                        iconHealthTex,
                        new Rectangle(0, 0, iconHealthTex.Width, iconHealthTex.Height),
                        dst,
                        new Vector2(0, 0),
                        0f,
                        Palette.White
                    );
                }
            }
            else
            {
                for (int i = 0; i < health; i++)
                {
                    Raylib.DrawRectangle(hx + i * 36, 40, 28, 28, Palette.Red);
                    Raylib.DrawRectangleLines(hx + i * 36, 40, 28, 28, Palette.DarkGray);
                }
            }
            if (health > 0)
                Raylib.DrawText("HP", 20, 72, 12, Palette.Black);

            // Timer
            float barW = 300;
            float progress = Math.Max(0f, Math.Min(1f, globalTimer / globalTimerMax));
            int barX = (int)(screenW / 2 - barW / 2 + 50);
            int barY = 40;

            if (iconsLoaded)
            {
                int itSize = 28;
                Raylib.DrawTexturePro(
                    iconTimerTex,
                    new Rectangle(0, 0, iconTimerTex.Width, iconTimerTex.Height),
                    new Rectangle(barX - itSize - 8, barY - 4, itSize, itSize),
                    new Vector2(0, 0),
                    0f,
                    Palette.White
                );
            }

            Raylib.DrawRectangle(barX, barY, (int)barW, 20, Palette.LightGray);
            Raylib.DrawRectangle(barX, barY, (int)(barW * progress), 20, Palette.SkyBlue);
            Raylib.DrawRectangleLines(barX, barY, (int)barW, 20, Palette.DarkBlue);
            Raylib.DrawText(
                $"Time: {globalTimer:0.0}s",
                screenW / 2 - 30 + 50,
                66,
                12,
                Palette.Black
            );

            // Help
            Raylib.DrawText(
                "Click any tiles to reveal. P to Pause. R to Restart after the game.",
                20,
                screenH - 30,
                14,
                Palette.DarkGray
            );
        }

        private void DrawTiles()
        {
            var tiles = tileManager.GetTiles();
            int lastHoverIndex = tileManager.GetLastHoverIndex();

            for (int i = 0; i < tiles.Count; i++)
            {
                var t = tiles[i];
                bool isHovered = (i == lastHoverIndex);

                // Use TextureHandler to draw the tile
                TextureHandler.DrawTile(t, isHovered);
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
                Raylib.DrawText(
                    msg,
                    screenW / 2 - Raylib.MeasureText(msg, 28) / 2,
                    screenH / 2 - 40,
                    28,
                    Palette.Gold
                );
                string sub = "Press R to play again";
                Raylib.DrawText(
                    sub,
                    screenW / 2 - Raylib.MeasureText(sub, 20) / 2,
                    screenH / 2 + 4,
                    20,
                    Palette.White
                );
            }

            if (gameState == GameState.GameOver)
            {
                Raylib.DrawRectangle(0, 0, screenW, screenH, new Color(0, 0, 0, 150));
                string msg = "GAME OVER";
                Raylib.DrawText(
                    msg,
                    screenW / 2 - Raylib.MeasureText(msg, 44) / 2,
                    screenH / 2 - 40,
                    44,
                    Palette.Red
                );
                string sub = $"Score: {score} - Press R to try again";
                Raylib.DrawText(
                    sub,
                    screenW / 2 - Raylib.MeasureText(sub, 20) / 2,
                    screenH / 2 + 12,
                    20,
                    Palette.White
                );
            }
        }

        public GameState GetGameState() => gameState;
    }
}

using System;
using System.IO;
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
        private Sound sFlip;
        private Sound sCorrect;
        private Sound sWrong;
        private Sound sHit;
        private Sound sHover;
        private Music bgMusic;
        private bool audioLoaded = false;

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
            LoadAudio();
            LoadTextures();
            LoadIcons();
            LoadPairTextures();
            LoadBackground();
            LoadLoadingScreen();
        }

        private void LoadAudio()
        {
            try
            {
                sFlip = Raylib.LoadSound("res/Flip.mp3");
                sCorrect = Raylib.LoadSound("res/Correct.wav");
                sWrong = Raylib.LoadSound("res/Wrong.wav");
                sHit = Raylib.LoadSound("res/Hit.wav");
                sHover = Raylib.LoadSound("res/onHover.wav");
                bgMusic = Raylib.LoadMusicStream("res/Shadow Circuit.mp3");
                Raylib.SetMusicVolume(bgMusic, 0.4f);
                Raylib.PlayMusicStream(bgMusic);
                audioLoaded = true;
            }
            catch { audioLoaded = false; }
        }

        private void LoadTextures()
        {
            try
            {
                tileNormalTex = Raylib.LoadTexture("tiles/PATileNormal.png");
                tileHoverTex = Raylib.LoadTexture("tiles/PATileHighlighted.png");
                texturesLoaded = true;
            }
            catch { texturesLoaded = false; }
        }

        private void LoadIcons()
        {
            try
            {
                string iconPath = Path.Combine(Directory.GetCurrentDirectory(), "img");
                if (Directory.Exists(iconPath))
                {
                    iconHealthTex = Raylib.LoadTexture(@"img\IconHealth.png");
                    iconTimerTex = Raylib.LoadTexture(@"img\IconTimer.png");
                    iconsLoaded = (iconHealthTex.Width > 0 && iconTimerTex.Width > 0);
                }
                else iconsLoaded = false;
            }
            catch { iconsLoaded = false; }
        }

        private void LoadPairTextures()
        {
            try
            {
                int pairs = 14; // (7x4 grid -> 28 tiles -> 14 pairs)
                var files = Directory.GetFiles("tileImage").OrderBy(f => f).ToArray();
                if (files.Length >= pairs)
                {
                    pairTextures = new Texture2D[pairs];
                    for (int i = 0; i < pairs; i++) pairTextures[i] = Raylib.LoadTexture(files[i]);
                    pairTexturesLoaded = true;
                }
                else pairTexturesLoaded = false;
            }
            catch { pairTexturesLoaded = false; }
        }

        private void LoadBackground()
        {
            try
            {
                backgroundTex = Raylib.LoadTexture("bg/TileBackground.png");
                backgroundLoaded = true;
            }
            catch { backgroundLoaded = false; }
        }

        private void LoadLoadingScreen()
        {
            try
            {
                loadingScreenTex = Raylib.LoadTexture("screens/ScreenGameStart.png");
                loadingScreenLoaded = true;
                gameState = GameState.Loading;
                loadingTimer = 0f;
            }
            catch { loadingScreenLoaded = false; }
        }

        public void UnloadResources()
        {
            try
            {
                if (texturesLoaded)
                {
                    Raylib.UnloadTexture(tileNormalTex);
                    Raylib.UnloadTexture(tileHoverTex);
                }

                if (pairTexturesLoaded)
                {
                    foreach (var tx in pairTextures) Raylib.UnloadTexture(tx);
                }

                if (iconsLoaded)
                {
                    Raylib.UnloadTexture(iconHealthTex);
                    Raylib.UnloadTexture(iconTimerTex);
                }

                if (backgroundLoaded) Raylib.UnloadTexture(backgroundTex);
                if (loadingScreenLoaded) Raylib.UnloadTexture(loadingScreenTex);

                if (audioLoaded)
                {
                    Raylib.UnloadSound(sFlip);
                    Raylib.UnloadSound(sCorrect);
                    Raylib.UnloadSound(sWrong);
                    Raylib.UnloadSound(sHit);
                    Raylib.UnloadSound(sHover);
                    Raylib.StopMusicStream(bgMusic);
                    Raylib.UnloadMusicStream(bgMusic);
                }
            }
            catch (Exception ex) { Console.WriteLine($"Unload error: {ex.Message}"); }
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
            if (audioLoaded) Raylib.UpdateMusicStream(bgMusic);

            if (gameState == GameState.Loading)
            {
                HandleLoadingState(dt);
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
                if (audioLoaded) Raylib.PlaySound(sHit);
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
            // Handle hover sound
            var mousePos = Raylib.GetMousePosition();
            int newHoverIndex = tileManager.UpdateHover(mousePos);
            if (newHoverIndex >= 0 && audioLoaded)
            {
                Raylib.PlaySound(sHover);
            }

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
                        if (audioLoaded) Raylib.PlaySound(sCorrect);
                        int basePoints = 100;
                        int bonus = 50; // Bonus if pair hasn't been unflipped twice
                        score += basePoints + bonus;
                        if (tileManager.CheckVictory()) gameState = GameState.Victory;
                    },
                    () =>
                    {
                        health--;
                        if (audioLoaded)
                        {
                            Raylib.PlaySound(sWrong);
                            Raylib.PlaySound(sHit);
                        }
                        if (health <= 0) gameState = GameState.GameOver;
                    }
                );

                if (tileClicked)
                {
                    if (audioLoaded) Raylib.PlaySound(sFlip);
                    globalTimer = globalTimerMax;
                }
            }
        }

        public void Draw()
        {
            Raylib.ClearBackground(Palette.RayWhite);

            if (gameState == GameState.Loading && loadingScreenLoaded)
            {
                DrawLoadingScreen();
            }
            else if (backgroundLoaded)
            {
                DrawBackground();
            }

            DrawGUI();
            DrawTiles();
            DrawStateOverlay();
        }

        private void DrawLoadingScreen()
        {
            Raylib.DrawTexturePro(
                loadingScreenTex,
                new Rectangle(0, 0, loadingScreenTex.Width, loadingScreenTex.Height),
                new Rectangle(0, 0, screenW, screenH),
                new Vector2(0, 0),
                0f,
                Palette.White
            );

            string pressMsg = "Press X to Start";
            int fontSize = 24;
            float alpha = (float)Math.Sin(loadingTimer * 4) * 0.5f + 0.5f;
            Color textColor = new Color(255, 255, 255, (int)(255 * alpha));
            Raylib.DrawText(pressMsg, screenW / 2 - Raylib.MeasureText(pressMsg, fontSize) / 2,
                          screenH - 80, fontSize, textColor);
        }

        private void DrawBackground()
        {
            Raylib.DrawTexturePro(
                backgroundTex,
                new Rectangle(0, 0, backgroundTex.Width, backgroundTex.Height),
                new Rectangle(0, 0, screenW, screenH),
                new Vector2(0, 0),
                0f,
                Palette.White
            );
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
                    Raylib.DrawTexturePro(iconHealthTex,
                        new Rectangle(0, 0, iconHealthTex.Width, iconHealthTex.Height),
                        dst, new Vector2(0, 0), 0f, Palette.White);
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
            if (health > 0) Raylib.DrawText("HP", 20, 72, 12, Palette.Black);

            // Timer
            float barW = 300;
            float progress = Math.Max(0f, Math.Min(1f, globalTimer / globalTimerMax));
            int barX = (int)(screenW / 2 - barW / 2 + 50);
            int barY = 40;

            if (iconsLoaded)
            {
                int itSize = 28;
                Raylib.DrawTexturePro(iconTimerTex,
                    new Rectangle(0, 0, iconTimerTex.Width, iconTimerTex.Height),
                    new Rectangle(barX - itSize - 8, barY - 4, itSize, itSize),
                    new Vector2(0, 0), 0f, Palette.White);
            }

            Raylib.DrawRectangle(barX, barY, (int)barW, 20, Palette.LightGray);
            Raylib.DrawRectangle(barX, barY, (int)(barW * progress), 20, Palette.SkyBlue);
            Raylib.DrawRectangleLines(barX, barY, (int)barW, 20, Palette.DarkBlue);
            Raylib.DrawText($"Time: {globalTimer:0.0}s", screenW / 2 - 30 + 50, 66, 12, Palette.Black);

            // Help
            Raylib.DrawText("Click tiles to flip. P to Pause. R to Restart after Win/Loss.",
                20, screenH - 30, 14, Palette.DarkGray);
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
                Raylib.DrawText(msg, screenW / 2 - Raylib.MeasureText(msg, 28) / 2,
                    screenH / 2 - 40, 28, Palette.Gold);
                string sub = "Press R to play again";
                Raylib.DrawText(sub, screenW / 2 - Raylib.MeasureText(sub, 20) / 2,
                    screenH / 2 + 4, 20, Palette.White);
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
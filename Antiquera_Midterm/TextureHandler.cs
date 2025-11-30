using System;
<<<<<<< HEAD
=======
using System.Collections.Generic;
using System.IO;
>>>>>>> e529e2be160b9e2c672146ba7a232c99408d5735
using Raylib_cs;

namespace MemoryGame
{
    public static class TextureHandler
    {
<<<<<<< HEAD
        public static void LoadAllTextures()
        {
            Console.WriteLine("Color-based memory game - no textures needed!");
=======
        // Texture storage
        private static Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
        private static bool texturesLoaded = false;

        // Screen textures
        private static Texture2D? screenGameStart;
        private static Texture2D? screenPaused;
        private static Texture2D? screenVictory;
        private static Texture2D? screenGameOver;
        private static Texture2D? tileBackground;

        // Tile textures
        private static Texture2D? tileNormal;
        private static Texture2D? tileHighlighted;
        private static Dictionary<string, Texture2D> characterTiles =
            new Dictionary<string, Texture2D>();

        // UI textures
        private static Texture2D? iconHealth;
        private static Texture2D? iconTimer;
        private static Texture2D? buttonPlay;
        private static Texture2D? buttonPause;
        private static Texture2D? hpBar;
        private static Texture2D? timerBar;

        public static void LoadAllTextures()
        {
            if (texturesLoaded)
                return;
            tileBackground = LoadTexture("img/TileBackground.png");

            screenGameStart = LoadTexture("img/ScreenGameStart.png");
            screenPaused = LoadTexture("img/ScreenPaused.png");
            screenVictory = LoadTexture("img/ScreenVictory.png");
            screenGameOver = LoadTexture("img/ScreenGameOver.png");

            tileNormal = LoadTexture("img/PATileNormal.png");
            tileHighlighted = LoadTexture("img/PATileHighlighted.png");

            iconHealth = LoadTexture("img/IconHealth.png");
            iconTimer = LoadTexture("img/IconTimer.png");
            buttonPlay = LoadTexture("img/ButtonPlay.png");
            buttonPause = LoadTexture("img/ButtonPause.png");
            hpBar = LoadTexture("img/HP Bar.png");
            timerBar = LoadTexture("img/TimerBar.png");

            LoadCharacterTiles();

            texturesLoaded = true;
            Console.WriteLine("All textures loaded successfully!");
        }

        private static void LoadCharacterTiles()
        {
            string[] characterNames =
            {
                "Aren",
                "Cisco",
                "ENGage",
                "Euriepidies",
                "Jiyo",
                "Kuzuri",
                "Marky",
                "Meanly",
                "Moon^2",
                "N1by",
                "Nyte",
                "Proksy",
                "Sia",
                "Zakkiyan",
            };

            foreach (string name in characterNames)
            {
                string filename = $"img/Tile_{name}.png";
                Texture2D texture = LoadTexture(filename);
                characterTiles[name] = texture;
            }
        }

        private static Texture2D LoadTexture(string path)
        {
            if (File.Exists(path))
            {
                Texture2D texture = Raylib.LoadTexture(path);
                textures[path] = texture;
                return texture;
            }
            else
            {
                Console.WriteLine($"Warning: Could not load texture: {path}");
                // Return a 1x1 white texture as fallback
                return Raylib.LoadTextureFromImage(Raylib.GenImageColor(1, 1, Color.White));
            }
>>>>>>> e529e2be160b9e2c672146ba7a232c99408d5735
        }

        public static void DrawBackground()
        {
<<<<<<< HEAD
            // Simple blue gradient background
            Raylib.DrawRectangleGradientV(0, 0, 1480, 900,
                new Color(135, 206, 235, 255), // Sky blue
                new Color(25, 25, 112, 255)); // Dark blue
=======
            if (tileBackground != null)
            {
                Raylib.DrawTexture(tileBackground.Value, 0, 0, Color.White);
            }
>>>>>>> e529e2be160b9e2c672146ba7a232c99408d5735
        }

        public static void DrawScreenOverlay(GameState gameState)
        {
<<<<<<< HEAD
            // Draw overlays for different game states
            switch (gameState)
            {
                case GameState.Loading:
                    DrawLoadingOverlay();
                    break;
                case GameState.Paused:
                    DrawPauseOverlay();
                    break;
                case GameState.Victory:
                    DrawVictoryOverlay();
                    break;
                case GameState.GameOver:
                    DrawGameOverOverlay();
                    break;
            }
        }

        private static void DrawLoadingOverlay()
        {
            Raylib.DrawRectangle(0, 0, 1480, 900, new Color(0, 0, 0, 200));
            string pressMsg = "Memory Game - Press X to Start";
            int fontSize = 36;
            Raylib.DrawText(pressMsg, 1480 / 2 - Raylib.MeasureText(pressMsg, fontSize) / 2,
                         900 / 2 - 20, fontSize, Palette.White);
        }

        private static void DrawPauseOverlay()
        {
            Raylib.DrawRectangle(0, 0, 1480, 900, new Color(0, 0, 0, 150));
            Raylib.DrawText("PAUSED", 1480 / 2 - 60, 900 / 2 - 20, 40, Palette.White);
        }

        private static void DrawVictoryOverlay()
        {
            Raylib.DrawRectangle(0, 0, 1480, 900, new Color(0, 0, 0, 150));
            string msg = "VICTORY! All tiles matched.";
            Raylib.DrawText(msg, 1480 / 2 - Raylib.MeasureText(msg, 28) / 2,
               900 / 2 - 40, 28, Palette.Gold);
            string sub = "Press R to play again";
            Raylib.DrawText(sub, 1480 / 2 - Raylib.MeasureText(sub, 20) / 2,
               900 / 2 + 4, 20, Palette.White);
        }

        private static void DrawGameOverOverlay()
        {
            Raylib.DrawRectangle(0, 0, 1480, 900, new Color(0, 0, 0, 150));
            string msg = "GAME OVER";
            Raylib.DrawText(msg, 1480 / 2 - Raylib.MeasureText(msg, 44) / 2,
               900 / 2 - 40, 44, Palette.Red);
            string sub = "Press R to try again";
            Raylib.DrawText(sub, 1480 / 2 - Raylib.MeasureText(sub, 20) / 2,
               900 / 2 + 12, 20, Palette.White);
=======
            Texture2D? screenTexture = null;

            switch (gameState)
            {
                case GameState.Loading:
                    screenTexture = screenGameStart;
                    break;
                case GameState.Paused:
                    screenTexture = screenPaused;
                    break;
                case GameState.Victory:
                    screenTexture = screenVictory;
                    break;
                case GameState.GameOver:
                    screenTexture = screenGameOver;
                    break;
            }

            if (screenTexture != null)
            {
                Raylib.DrawTexture(screenTexture.Value, 0, 0, Color.White);
            }
>>>>>>> e529e2be160b9e2c672146ba7a232c99408d5735
        }

        public static void DrawTile(Tile tile, bool isHovered = false)
        {
            if (tile.State == TileState.Closed)
            {
<<<<<<< HEAD
                // Draw closed tile
                Color tileColor = isHovered ? Palette.SkyBlue : Palette.DarkBlue;
                Raylib.DrawRectangle((int)tile.X, (int)tile.Y, (int)tile.W, (int)tile.H, tileColor);
                Raylib.DrawRectangleLines((int)tile.X, (int)tile.Y, (int)tile.W, (int)tile.H, Palette.White);

                // Draw question mark
                string questionMark = "?";
                int fontSize = (int)(tile.H * 0.4f);
                int textWidth = Raylib.MeasureText(questionMark, fontSize);
                int textX = (int)(tile.X + (tile.W - textWidth) / 2);
                int textY = (int)(tile.Y + (tile.H - fontSize) / 2);
                Raylib.DrawText(questionMark, textX, textY, fontSize, Palette.White);
            }
            else if (tile.State == TileState.Revealed)
            {
                // Draw character color
                Color tileColor = GetCharacterColor(tile.CharacterName);
                Raylib.DrawRectangle((int)tile.X, (int)tile.Y, (int)tile.W, (int)tile.H, tileColor);
                Raylib.DrawRectangleLines((int)tile.X, (int)tile.Y, (int)tile.W, (int)tile.H, Palette.White);

                // Draw character name
                int fontSize = (int)(tile.H * 0.2f);
                int textWidth = Raylib.MeasureText(tile.CharacterName, fontSize);
                int textX = (int)(tile.X + (tile.W - textWidth) / 2);
                int textY = (int)(tile.Y + (tile.H - fontSize) / 2);
                Raylib.DrawText(tile.CharacterName, textX, textY, fontSize, Palette.White);
            }
            else if (tile.State == TileState.Matched)
            {
                // Draw matched tile with checkmark
                Color tileColor = GetCharacterColor(tile.CharacterName);
                Color dimmedColor = new Color((byte)(tileColor.R * 0.7f), (byte)(tileColor.G * 0.7f), (byte)(tileColor.B * 0.7f), (byte)255);
                Raylib.DrawRectangle((int)tile.X, (int)tile.Y, (int)tile.W, (int)tile.H, dimmedColor);
                Raylib.DrawRectangleLines((int)tile.X, (int)tile.Y, (int)tile.W, (int)tile.H, Palette.Gold);

                // Draw checkmark
                string checkmark = "✓";
                int fontSize = (int)(tile.H * 0.4f);
                int textWidth = Raylib.MeasureText(checkmark, fontSize);
                int textX = (int)(tile.X + (tile.W - textWidth) / 2);
                int textY = (int)(tile.Y + (tile.H - fontSize) / 2);
                Raylib.DrawText(checkmark, textX, textY, fontSize, Palette.Gold);
=======
                // draw closed tile
                Texture2D? tileTex = isHovered ? tileHighlighted : tileNormal;
                if (tileTex != null)
                {
                    Raylib.DrawTexture(tileTex.Value, (int)tile.X, (int)tile.Y, Color.White);
                }
            }
            else if (tile.State == TileState.Revealed)
            {
                // draw character image
                if (characterTiles.ContainsKey(tile.CharacterName))
                {
                    Raylib.DrawTexture(
                        characterTiles[tile.CharacterName],
                        (int)tile.X,
                        (int)tile.Y,
                        Color.White
                    );
                }
                else
                {
                    // Fallback: draw a colored rectangle with character name
                    Color tileColor = GetCharacterColor(tile.CharacterName);
                    Raylib.DrawRectangle(
                        (int)tile.X,
                        (int)tile.Y,
                        (int)tile.W,
                        (int)tile.H,
                        tileColor
                    );
                    Raylib.DrawText(
                        tile.CharacterName,
                        (int)tile.X + 10,
                        (int)tile.Y + 10,
                        20,
                        Color.White
                    );
                }
            }
            else if (tile.State == TileState.Matched)
            {
                // Draw matched tile (slightly dimmed)
                if (characterTiles.ContainsKey(tile.CharacterName))
                {
                    Raylib.DrawTexture(
                        characterTiles[tile.CharacterName],
                        (int)tile.X,
                        (int)tile.Y,
                        new Color(255, 255, 255, 180)
                    );
                }
                else
                {
                    Color tileColor = GetCharacterColor(tile.CharacterName);
                    Raylib.DrawRectangle(
                        (int)tile.X,
                        (int)tile.Y,
                        (int)tile.W,
                        (int)tile.H,
                        new Color(
                            (byte)tileColor.R,
                            (byte)tileColor.G,
                            (byte)tileColor.B,
                            (byte)180
                        )
                    );
                }
>>>>>>> e529e2be160b9e2c672146ba7a232c99408d5735
            }
        }

        private static Color GetCharacterColor(string characterName)
        {
<<<<<<< HEAD
            // Each character gets a unique color
=======
            // Assign different colors for each character
>>>>>>> e529e2be160b9e2c672146ba7a232c99408d5735
            return characterName switch
            {
                "Aren" => Color.Red,
                "Cisco" => Color.Blue,
                "ENGage" => Color.Green,
                "Euriepidies" => Color.Yellow,
                "Jiyo" => Color.Orange,
                "Kuzuri" => Color.Purple,
                "Marky" => Color.Pink,
                "Meanly" => Color.Brown,
                "Moon^2" => Color.SkyBlue,
                "N1by" => Color.Lime,
                "Nyte" => Color.DarkBlue,
                "Proksy" => Color.Magenta,
<<<<<<< HEAD
                "Sia" => new Color(0, 255, 255, 255), // Cyan
                "Zakkiyan" => Color.Gold,
                _ => Color.Gray
            };
        }

        public static void UnloadAllTextures()
        {
            // No textures to unload
        }
    }
}
=======
                "Sia" => Color.Blue,
                "Zakkiyan" => Color.Gold,
                _ => Color.Gray,
            };
        }

        public static void DrawUI(int health, int maxHealth, float timer, float maxTimer, int score)
        {
            // Draw health icon and bar
            if (iconHealth != null)
            {
                Raylib.DrawTexture(iconHealth.Value, 20, 20, Color.White);
            }
            if (hpBar != null)
            {
                Raylib.DrawTexture(hpBar.Value, 60, 25, Color.White);
                // Draw health bar fill
                float healthPercent = (float)health / maxHealth;
                Raylib.DrawRectangle(65, 30, (int)(150 * healthPercent), 20, Color.Red);
            }

            // Draw timer icon and bar
            if (iconTimer != null)
            {
                Raylib.DrawTexture(iconTimer.Value, 20, 60, Color.White);
            }
            if (timerBar != null)
            {
                Raylib.DrawTexture(timerBar.Value, 60, 65, Color.White);
                // Draw timer bar fill
                float timerPercent = timer / maxTimer;
                Raylib.DrawRectangle(65, 70, (int)(150 * timerPercent), 20, Color.Green);
            }

            // Draw score
            Raylib.DrawText($"Score: {score}", 20, 100, 30, Color.White);
        }

        public static void DrawPauseButton(bool isPaused)
        {
            Texture2D? buttonTex = isPaused ? buttonPlay : buttonPause;
            if (buttonTex != null)
            {
                Raylib.DrawTexture(buttonTex.Value, 800, 20, Color.White);
            }
        }

        public static bool AreTexturesLoaded => texturesLoaded;
    }
}
>>>>>>> e529e2be160b9e2c672146ba7a232c99408d5735

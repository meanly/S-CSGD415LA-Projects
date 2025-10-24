using System;
using System.Collections.Generic;
using System.IO;
using Raylib_cs;

namespace MemoryGame
{
    public static class TextureHandler
    {
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
        private static Dictionary<string, Texture2D> characterTiles = new Dictionary<string, Texture2D>();

        // UI textures
        private static Texture2D? iconHealth;
        private static Texture2D? iconTimer;
        private static Texture2D? buttonPlay;
        private static Texture2D? buttonPause;
        private static Texture2D? hpBar;
        private static Texture2D? timerBar;

        public static void LoadAllTextures()
        {
            if (texturesLoaded) return;

            try
            {
                // Load background
                tileBackground = LoadTexture("img/TileBackground.png");

                // Load screen overlays
                screenGameStart = LoadTexture("img/ScreenGameStart.png");
                screenPaused = LoadTexture("img/ScreenPaused.png");
                screenVictory = LoadTexture("img/ScreenVictory.png");
                screenGameOver = LoadTexture("img/ScreenGameOver.png");

                // Load tile textures
                tileNormal = LoadTexture("img/PATileNormal.png");
                tileHighlighted = LoadTexture("img/PATileHighlighted.png");

                // Load UI elements
                iconHealth = LoadTexture("img/IconHealth.png");
                iconTimer = LoadTexture("img/IconTimer.png");
                buttonPlay = LoadTexture("img/ButtonPlay.png");
                buttonPause = LoadTexture("img/ButtonPause.png");
                hpBar = LoadTexture("img/HP Bar.png");
                timerBar = LoadTexture("img/TimerBar.png");

                // Load character tile images
                LoadCharacterTiles();

                texturesLoaded = true;
                Console.WriteLine("All textures loaded successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading textures: {ex.Message}");
            }
        }

        private static void LoadCharacterTiles()
        {
            string[] characterNames = {
                "Aren", "Cisco", "ENGage", "Euriepidies", "Jiyo", "Kuzuri",
                "Marky", "Meanly", "Moon^2", "N1by", "Nyte", "Proksy", "Sia", "Zakkiyan"
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
        }

        public static void DrawBackground()
        {
            if (tileBackground != null)
            {
                Raylib.DrawTexture(tileBackground.Value, 0, 0, Color.White);
            }
        }

        public static void DrawScreenOverlay(GameState gameState)
        {
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
        }

        public static void DrawTile(Tile tile, bool isHovered = false)
        {
            if (tile.State == TileState.Closed)
            {
                // Draw closed tile
                Texture2D? tileTex = isHovered ? tileHighlighted : tileNormal;
                if (tileTex != null)
                {
                    Raylib.DrawTexture(tileTex.Value, (int)tile.X, (int)tile.Y, Color.White);
                }
            }
            else if (tile.State == TileState.Revealed)
            {
                // Draw character image
                if (characterTiles.ContainsKey(tile.CharacterName))
                {
                    Raylib.DrawTexture(characterTiles[tile.CharacterName], (int)tile.X, (int)tile.Y, Color.White);
                }
                else
                {
                    // Fallback: draw a colored rectangle with character name
                    Color tileColor = GetCharacterColor(tile.CharacterName);
                    Raylib.DrawRectangle((int)tile.X, (int)tile.Y, (int)tile.W, (int)tile.H, tileColor);
                    Raylib.DrawText(tile.CharacterName, (int)tile.X + 10, (int)tile.Y + 10, 20, Color.White);
                }
            }
            else if (tile.State == TileState.Matched)
            {
                // Draw matched tile (slightly dimmed)
                if (characterTiles.ContainsKey(tile.CharacterName))
                {
                    Raylib.DrawTexture(characterTiles[tile.CharacterName], (int)tile.X, (int)tile.Y, new Color(255, 255, 255, 180));
                }
                else
                {
                    Color tileColor = GetCharacterColor(tile.CharacterName);
                    Raylib.DrawRectangle((int)tile.X, (int)tile.Y, (int)tile.W, (int)tile.H, new Color((byte)tileColor.R, (byte)tileColor.G, (byte)tileColor.B, (byte)180));
                }
            }
        }

        private static Color GetCharacterColor(string characterName)
        {
            // Assign different colors for each character
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
                "Sia" => Color.Blue,
                "Zakkiyan" => Color.Gold,
                _ => Color.Gray
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

        public static void UnloadAllTextures()
        {
            foreach (var texture in textures.Values)
            {
                Raylib.UnloadTexture(texture);
            }
            textures.Clear();
            texturesLoaded = false;
        }

        public static bool AreTexturesLoaded => texturesLoaded;
    }
}

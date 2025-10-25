using System;
using Raylib_cs;

namespace MemoryGame
{
    public static class TextureHandler
    {
        public static void LoadAllTextures()
        {
            Console.WriteLine("Color-based memory game - no textures needed!");
        }

        public static void DrawBackground()
        {
            // Simple blue gradient background
            Raylib.DrawRectangleGradientV(0, 0, 1480, 900,
                new Color(135, 206, 235, 255), // Sky blue
                new Color(25, 25, 112, 255)); // Dark blue
        }

        public static void DrawScreenOverlay(GameState gameState)
        {
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
        }

        public static void DrawTile(Tile tile, bool isHovered = false)
        {
            if (tile.State == TileState.Closed)
            {
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
            }
        }

        private static Color GetCharacterColor(string characterName)
        {
            // Each character gets a unique color
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
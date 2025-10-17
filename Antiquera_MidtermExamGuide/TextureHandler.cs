using Raylib_cs;
using System.Collections.Generic;

namespace MemoryGame
{
    public class TextureHandler
    {
        private Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();

        public void LoadTextures()
        {
            try { textures["background"] = Raylib.LoadTexture("img/TileBackground.png"); } catch { }
            try { textures["tile_normal"] = Raylib.LoadTexture("img/PATileNormal.png"); } catch { }
            try { textures["tile_highlighted"] = Raylib.LoadTexture("img/PATileHighlighted.png"); } catch { }
            try { textures["screen_start"] = Raylib.LoadTexture("img/ScreenGameStart.png"); } catch { }
            try { textures["screen_paused"] = Raylib.LoadTexture("img/ScreenPaused.png"); } catch { }
            try { textures["screen_game_over"] = Raylib.LoadTexture("img/ScreenGameOver.png"); } catch { }
            try { textures["screen_victory"] = Raylib.LoadTexture("img/ScreenVictory.png"); } catch { }

            string[] chars = { "Tile_Aren.png", "Tile_Cisco.png", "Tile_ENGage.png", "Tile_Euriepidies.png", "Tile_Jiyo.png", "Tile_Kuzuri.png", "Tile_Marky.png", "Tile_Meanly.png", "Tile_Moon^2.png", "Tile_N1by.png", "Tile_Nyte.png", "Tile_Proksy.png", "Tile_Sia.png", "Tile_Zakkiyan.png" };
            foreach (var c in chars) try { textures[c] = Raylib.LoadTexture($"img/{c}"); } catch { }
        }

        public void UnloadTextures() { foreach (var t in textures.Values) Raylib.UnloadTexture(t); }

        public void DrawGame(GameManager gm)
        {
            if (textures.ContainsKey("background")) Raylib.DrawTexture(textures["background"], 0, 0, Color.White);

            if (gm.State == GameState.Ongoing)
            {
                // Draw tiles - ALWAYS draw them
                foreach (var tile in gm.TileManager.Tiles)
                {
                    if (tile.IsMatched || tile.IsFlipped)
                    {
                        var img = gm.TileManager.GetCharacterImage(tile.Value);
                        if (img != null && textures.ContainsKey(img))
                            Raylib.DrawTexture(textures[img], (int)tile.X, (int)tile.Y, Color.White);
                        else
                        {
                            // Fallback to normal tile if character image not found
                            if (textures.ContainsKey("tile_normal"))
                                Raylib.DrawTexture(textures["tile_normal"], (int)tile.X, (int)tile.Y, Color.White);
                        }
                    }
                    else
                    {
                        var tex = tile.IsHovered ? "tile_highlighted" : "tile_normal";
                        if (textures.ContainsKey(tex))
                            Raylib.DrawTexture(textures[tex], (int)tile.X, (int)tile.Y, Color.White);
                    }
                }
            }
            else if (gm.State == GameState.Paused && textures.ContainsKey("screen_paused"))
                Raylib.DrawTexture(textures["screen_paused"], 0, 0, Color.White);
            else if (gm.State == GameState.GameOver && textures.ContainsKey("screen_game_over"))
                Raylib.DrawTexture(textures["screen_game_over"], 0, 0, Color.White);
            // Disabled victory screen overlay
            // else if (gm.State == GameState.Win && textures.ContainsKey("screen_victory"))
            //     Raylib.DrawTexture(textures["screen_victory"], 0, 0, Color.White);
        }
    }
}
using Raylib_cs;
using System.Collections.Generic;
using System.Linq;

namespace MemoryGame
{
    public class TileManager
    {
        public List<Tile> Tiles { get; private set; } = new List<Tile>();
        public List<Tile> FlippedTiles { get; private set; } = new List<Tile>();
        private string[] characterImages = { "Tile_Aren.png", "Tile_Cisco.png", "Tile_ENGage.png", "Tile_Euriepidies.png", "Tile_Jiyo.png", "Tile_Kuzuri.png", "Tile_Marky.png", "Tile_Meanly.png", "Tile_Moon^2.png", "Tile_N1by.png", "Tile_Nyte.png", "Tile_Proksy.png", "Tile_Sia.png", "Tile_Zakkiyan.png" };

        public void NewGame()
        {
            Tiles.Clear(); FlippedTiles.Clear();
            var tileValues = new List<int>();
            for (int i = 0; i < 14; i++) { tileValues.Add(i); tileValues.Add(i); }
            tileValues = tileValues.OrderBy(x => new System.Random().Next()).ToList();

            for (int row = 0; row < 7; row++)
                for (int col = 0; col < 4; col++)
                    Tiles.Add(new Tile(400 + col * 120, 150 + row * 100, 100, 100, tileValues[row * 4 + col]));
        }

        public void FlipTile(Tile tile) { if (!FlippedTiles.Contains(tile) && FlippedTiles.Count < 2) { tile.Flip(); FlippedTiles.Add(tile); } }
        public void UnflipAllTiles() { foreach (var tile in FlippedTiles) tile.Unflip(); FlippedTiles.Clear(); }
        public void HideAllTiles() { foreach (var tile in Tiles) tile.Unflip(); }
        public void ShowAllTiles() { foreach (var tile in Tiles) tile.Flip(); }
        public bool HasTwoFlipped() => FlippedTiles.Count == 2;
        public bool CompareFlippedTiles() { if (FlippedTiles.Count != 2) return false; if (FlippedTiles[0].Value == FlippedTiles[1].Value) { FlippedTiles[0].Match(); FlippedTiles[1].Match(); FlippedTiles.Clear(); return true; } return false; }
        public Tile? GetTileAtPosition(float x, float y) => Tiles.FirstOrDefault(t => t.IsPointInside(x, y) && !t.IsMatched);
        public bool AllTilesMatched() => Tiles.All(t => t.IsMatched);
        public string? GetCharacterImage(int tileValue) => tileValue >= 0 && tileValue < characterImages.Length ? characterImages[tileValue] : null;
    }
}
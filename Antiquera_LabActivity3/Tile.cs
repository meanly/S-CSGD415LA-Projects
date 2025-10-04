using Raylib_cs;
using System.Numerics;

namespace Antiquera_LabActivity3
{
    // Tile colors enum
    public enum TileColor
    {
        Black = 0,      // Empty
        Yellow = 1,
        Blue = 2,
        Red = 3,
        LimeGreen = 4,
        Violet = 5,
        Orange = 6,
        Cyan = 7,
        Pink = 8
    }

    // Tile class representing individual tiles
    public class Tile
    {
        public TileColor Color { get; set; }
        public bool IsEmpty => Color == TileColor.Black;

        public Tile(TileColor color = TileColor.Black)
        {
            Color = color;
        }
    }

    // TilePattern class representing preset patterns using tileSet[x][y] syntax
    public class TilePattern
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public TileColor[,] TileSet { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public TilePattern(int id, string name, TileColor[,] tileSet)
        {
            Id = id;
            Name = name;
            TileSet = tileSet;
            Width = tileSet.GetLength(0);
            Height = tileSet.GetLength(1);
        }

        // Helper method to get positions of non-empty tiles
        public List<Vector2> GetPositions()
        {
            var positions = new List<Vector2>();
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (TileSet[x, y] != TileColor.Black)
                    {
                        positions.Add(new Vector2(x, y));
                    }
                }
            }
            return positions;
        }

        // Helper method to get the color of the pattern (first non-black tile)
        public TileColor GetColor()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (TileSet[x, y] != TileColor.Black)
                    {
                        return TileSet[x, y];
                    }
                }
            }
            return TileColor.Black;
        }
    }
}

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

    // TilePattern class representing preset patterns
    public class TilePattern
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Vector2> Positions { get; set; }
        public TileColor Color { get; set; }

        public TilePattern(int id, string name, List<Vector2> positions, TileColor color)
        {
            Id = id;
            Name = name;
            Positions = positions;
            Color = color;
        }
    }
}

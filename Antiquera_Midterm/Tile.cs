using System;
using Raylib_cs;

namespace MemoryGame
{
    public enum TileState
    {
        Closed,
        Revealed,
        Matched,
    }

    public class Tile
    {
        public int PairId { get; set; }
        public TileState State { get; set; } = TileState.Closed;
        public int FlipCount { get; set; } = 0; // tracks how many times it was unflipped
        public string CharacterName { get; set; } = "";
        public Raylib_cs.Rectangle Rect { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float W { get; set; }
        public float H { get; set; }
    }
}

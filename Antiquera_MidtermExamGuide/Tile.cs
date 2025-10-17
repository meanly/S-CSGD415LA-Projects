using Raylib_cs;
using System;

namespace MemoryGame
{
    public class Tile
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Value { get; set; }
        public bool IsFlipped { get; set; }
        public bool IsMatched { get; set; }
        
        private int screenX;
        private int screenY;
        private int width = 80;
        private int height = 80;
        
        public Tile(int x, int y, int value)
        {
            X = x;
            Y = y;
            Value = value;
            IsFlipped = false;
            IsMatched = false;
            
            // Calculate screen position
            screenX = 200 + x * 100;
            screenY = 100 + y * 60;
        }
        
        public void Flip()
        {
            if (!IsMatched)
                IsFlipped = !IsFlipped;
        }
        
        public void Unflip()
        {
            if (!IsMatched)
                IsFlipped = false;
        }
        
        public void Match()
        {
            IsMatched = true;
            IsFlipped = true;
        }
        
        public bool IsPointInside(float x, float y)
        {
            return x >= screenX && x <= screenX + width && y >= screenY && y <= screenY + height;
        }
        
        public void Draw()
        {
            if (IsMatched || IsFlipped)
            {
                // Draw character (just a colored rectangle for now)
                Color[] colors = { Color.Red, Color.Blue, Color.Green, Color.Yellow, Color.Orange, Color.Purple, Color.Pink, Color.Cyan, Color.Magenta, Color.Lime, Color.Gold, Color.Silver, Color.Maroon, Color.Navy };
                Raylib.DrawRectangle(screenX, screenY, width, height, colors[Value % colors.Length]);
                Raylib.DrawText(Value.ToString(), screenX + 30, screenY + 30, 20, Color.White);
            }
            else
            {
                // Draw back of tile
                Raylib.DrawRectangle(screenX, screenY, width, height, Color.DarkGray);
                Raylib.DrawText("?", screenX + 30, screenY + 30, 20, Color.White);
            }
        }
    }
}
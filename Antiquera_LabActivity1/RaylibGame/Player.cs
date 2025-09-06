using Raylib_cs;
using System.Numerics;

public class Player
{
    public Vector2 Position;
    public float Speed = 5f;
    public int hp = 100;

    public struct Size
    {
        public float height;
        public float width;
        public Size(float _height, float _width)
        {
            height = _height;
            width = _width;
        }
    }
    public float x { get; set; }
    public float y { get; set; }
    public Size objectSize;
    public Player(float _x, float _y)
    {
        Position = new Vector2(_x, _y);
        x = Position.X;
        y = Position.Y;

        objectSize = new Size(50, 50);

    }
    public void Move()
    {
        if (Raylib.IsKeyDown(KeyboardKey.W)) Position.Y -= Speed;
        if (Raylib.IsKeyDown(KeyboardKey.S)) Position.Y += Speed;
        if (Raylib.IsKeyDown(KeyboardKey.A)) Position.X -= Speed;
        if (Raylib.IsKeyDown(KeyboardKey.D)) Position.X += Speed;
  
    }
    public void Draw()
    {
        Raylib.DrawRectangle((int)Position.X, (int)Position.Y, (int)objectSize.height, (int)objectSize.width, Color.Blue);
    }
}


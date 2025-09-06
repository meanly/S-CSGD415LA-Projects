using Raylib_cs;
using System.Numerics;

public class Enemy
{
    public Vector2 Position;
    public float Speed = 5f;
    public int hp = 100;
    public float DetectionRange = 300f;
    public float X { get; set;}
    public float Y { get; set;}
 

    public Enemy(float _x, float _y)
    {
        Position = new Vector2(_x, _y);
        X = _x;
        Y = _y;


    }
   public void Update(Vector2 playerPosition)
    {
        float distance = Raymath.Vector2Distance(Position, playerPosition);
        if (distance <= DetectionRange)
        {
            Vector2 direction = Raymath.Vector2Subtract(playerPosition, Position);
            direction = Raymath.Vector2Normalize(direction);
            Position = Raymath.Vector2Add(Position, Raymath.Vector2Scale(direction, Speed));
        }
        // Else: stay still
    }

    public void Draw()
    {
        Raylib.DrawRectangle((int)Position.X, (int)Position.Y, 35, 35, Color.Red);
    }
}

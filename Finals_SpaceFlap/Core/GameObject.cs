using Raylib_cs;
using System.Numerics;

namespace Finals_SpaceFlap.Core;

public abstract class GameObject
{
    public Vector2 Position { get; protected set; }
    protected Vector2 Velocity { get; set; }
    protected float Width { get; set; }
    protected float Height { get; set; }

    protected GameObject(float x, float y, float width, float height)
    {
        Position = new Vector2(x, y);
        Velocity = new Vector2(0, 0);
        Width = width;
        Height = height;
    }

    public virtual Rectangle GetBounds()
    {
        return new Rectangle(Position.X, Position.Y, Width, Height);
    }

    public virtual bool CheckCollision(GameObject other)
    {
        Rectangle bounds = GetBounds();
        Rectangle otherBounds = other.GetBounds();
        return Raylib.CheckCollisionRecs(bounds, otherBounds);
    }

    public abstract void Update(float deltaTime);
    public abstract void Render();
}

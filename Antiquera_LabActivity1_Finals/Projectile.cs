using Raylib_cs;
using System.Numerics;

namespace Antiquera_LabActivity1_Finals;

public class Projectile
{
    private Vector2 position;
    private Vector2 velocity;
    private float lifetime;
    private float maxLifetime = 3.0f;
    private Texture2D texture;

    public Vector2 Position => position;

    public Projectile(Vector2 startPosition, Vector2 direction, float speed)
    {
        position = startPosition;
        velocity = Vector2.Normalize(direction) * speed;
        lifetime = 0;

        // Load arrow texture
        string arrowPath = "Tiny Adventure Pack Plus/Extras/Arrow(projectile)/Arrow01(32x32).png";
        if (System.IO.File.Exists(arrowPath))
        {
            texture = Raylib.LoadTexture(arrowPath);
        }
    }

    public void Update(float deltaTime)
    {
        position += velocity * deltaTime;
        lifetime += deltaTime;
    }

    public bool IsExpired()
    {
        return lifetime >= maxLifetime;
    }

    public void Draw()
    {
        if (texture.Id != 0)
        {
            // Calculate rotation based on velocity direction
            float rotation = (float)Math.Atan2(velocity.Y, velocity.X) * 180.0f / (float)Math.PI;

            Rectangle sourceRect = new Rectangle(0, 0, texture.Width, texture.Height);
            Rectangle destRect = new Rectangle(position.X, position.Y, texture.Width, texture.Height);
            Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 2);

            Raylib.DrawTexturePro(texture, sourceRect, destRect, origin, rotation, new Color(255, 255, 255, 255));
        }
        else
        {
            // Fallback: draw colored rectangle
            Raylib.DrawRectangle((int)position.X, (int)position.Y, 8, 8, new Color(255, 255, 0, 255));
        }
    }

    public void Unload()
    {
        if (texture.Id != 0)
        {
            Raylib.UnloadTexture(texture);
        }
    }
}


using Raylib_cs;
using System.Numerics;

public class HeartParticle
{
    public float x, y;
    public float velocityX, velocityY;
    public float life;
    public float maxLife;
    public Color color;
    public float size;
    public bool isActive;

    public HeartParticle(float startX, float startY)
    {
        x = startX;
        y = startY;

        // Random upward velocity with slight horizontal spread
        velocityX = (float)(new Random().NextDouble() - 0.5) * 50f; // -25 to 25
        velocityY = -(float)(new Random().NextDouble() * 30f + 20f); // -50 to -20 (upward)

        maxLife = 1.5f; // 1.5 seconds
        life = maxLife;

        // Random heart color (pink variations)
        int colorVariant = new Random().Next(0, 3);
        switch (colorVariant)
        {
            case 0:
                color = new Color(255, 182, 193, 255); // Light pink
                break;
            case 1:
                color = new Color(255, 105, 180, 255); // Hot pink
                break;
            case 2:
                color = new Color(255, 20, 147, 255); // Deep pink
                break;
        }

        size = (float)(new Random().NextDouble() * 0.3f + 0.3f); // 0.3 to 0.6 (medium hearts)
        isActive = true;
    }

    public void Update()
    {
        if (!isActive) return;

        // Apply gravity
        velocityY += 20f * Raylib.GetFrameTime();

        // Update position
        x += velocityX * Raylib.GetFrameTime();
        y += velocityY * Raylib.GetFrameTime();

        // Decrease life
        life -= Raylib.GetFrameTime();

        // Fade out as life decreases
        float alpha = (life / maxLife) * 255f;
        // Clamp alpha to valid range (0-255) to prevent overflow
        alpha = Math.Max(0f, Math.Min(255f, alpha));
        color = new Color(color.R, color.G, color.B, (int)alpha);

        // Deactivate when life runs out
        if (life <= 0)
        {
            isActive = false;
        }
    }

    public void Draw()
    {
        if (!isActive) return;

        // Draw a simple heart shape using two circles and a triangle
        float heartSize = size * 25f; // Scale up the size (medium hearts)

        // Left circle of heart
        Raylib.DrawCircle((int)(x - heartSize * 0.3f), (int)(y - heartSize * 0.1f), heartSize * 0.4f, color);

        // Right circle of heart
        Raylib.DrawCircle((int)(x + heartSize * 0.3f), (int)(y - heartSize * 0.1f), heartSize * 0.4f, color);

        // Triangle point of heart
        Vector2[] trianglePoints = {
            new Vector2(x, y + heartSize * 0.6f),
            new Vector2(x - heartSize * 0.6f, y + heartSize * 0.2f),
            new Vector2(x + heartSize * 0.6f, y + heartSize * 0.2f)
        };

        Raylib.DrawTriangle(trianglePoints[0], trianglePoints[1], trianglePoints[2], color);
    }
}

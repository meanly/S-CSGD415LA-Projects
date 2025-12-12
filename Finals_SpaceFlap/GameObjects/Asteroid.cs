using Finals_SpaceFlap.Core;
using Raylib_cs;
using System.Numerics;

namespace Finals_SpaceFlap.GameObjects;

public class Asteroid : GameObject
{
    private const float Speed = 200f;
    private const float Scale = 0.3f; // Scale down the image
    private Texture2D? texture;
    private bool hasBeenPassed;

    public bool HasBeenPassed => hasBeenPassed;
    public float RightEdge => Position.X + Width;

    public Asteroid(float x, float y, float width, float height)
        : base(x, y, width, height)
    {
        Velocity = new Vector2(-Speed, 0);
        LoadTexture(width, height);
    }

    private void LoadTexture(float providedWidth, float providedHeight)
    {
        string texturePath = Path.Combine("Assets", "asteroid.gif");
        if (File.Exists(texturePath))
        {
            Image image = Raylib.LoadImage(texturePath);
            texture = Raylib.LoadTextureFromImage(image);

            // Store original dimensions before unloading
            int imgWidth = image.Width;
            int imgHeight = image.Height;

            // Update collision bounds based on actual scaled texture size
            // Use the smaller of the texture size or provided size, scaled
            float textureWidth = imgWidth * Scale;
            float textureHeight = imgHeight * Scale;

            // Use provided dimensions if they're smaller, otherwise use scaled texture size
            Width = Math.Min(providedWidth, textureWidth);
            Height = Math.Min(providedHeight, textureHeight);

            Raylib.UnloadImage(image);
        }
    }

    public override void Update(float deltaTime)
    {
        Position = new Vector2(Position.X + Velocity.X * deltaTime, Position.Y);
    }

    public override void Render()
    {
        if (texture.HasValue)
        {
            // Get texture dimensions for source rectangle
            int texWidth = (int)(Width / Scale);
            int texHeight = (int)(Height / Scale);

            // Draw scaled texture
            Rectangle source = new Rectangle(0, 0, texWidth, texHeight);
            Rectangle dest = new Rectangle(Position.X, Position.Y, Width, Height);
            Vector2 origin = new Vector2(0, 0);
            float rotation = 0f;

            Raylib.DrawTexturePro(texture.Value, source, dest, origin, rotation, Color.White);
        }
        else
        {
            // Fallback rectangle if texture not loaded
            Raylib.DrawRectangle((int)Position.X, (int)Position.Y, (int)Width, (int)Height, Color.Gray);
        }
    }

    public void MarkAsPassed()
    {
        hasBeenPassed = true;
    }

    public bool IsOffScreen(int screenWidth)
    {
        return Position.X + Width < 0;
    }
}


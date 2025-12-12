using Finals_SpaceFlap.Core;
using Raylib_cs;
using System.Numerics;

namespace Finals_SpaceFlap.GameObjects;

public class Spaceship : GameObject
{
    private const float JumpVelocity = -250f; // Lower jump for easier navigation
    private const float Gravity = 800f;
    private const float Scale = 0.2f; // Smaller scale like flappy bird
    private Texture2D? texture;
    private float facingDirection = 1f; // 1 = right, -1 = left

    public Spaceship(float x, float y)
        : base(x, y, 50, 50)
    {
        LoadTexture();
        UpdateCollisionBounds();
    }

    private void LoadTexture()
    {
        string texturePath = Path.Combine("Assets", "spaceship.png");
        if (File.Exists(texturePath))
        {
            Image image = Raylib.LoadImage(texturePath);
            texture = Raylib.LoadTextureFromImage(image);

            // Update collision bounds based on actual scaled texture size
            // Store original dimensions before unloading
            int imgWidth = image.Width;
            int imgHeight = image.Height;
            Width = imgWidth * Scale;
            Height = imgHeight * Scale;

            Raylib.UnloadImage(image);
        }
    }

    private void UpdateCollisionBounds()
    {
        // Collision bounds match the scaled visual size
    }

    public override void Update(float deltaTime)
    {
        // Apply gravity
        Velocity = new Vector2(Velocity.X, Velocity.Y + Gravity * deltaTime);

        // Handle horizontal movement for facing direction
        float horizontalInput = 0f;
        if (Raylib.IsKeyDown(KeyboardKey.Left) || Raylib.IsKeyDown(KeyboardKey.A))
        {
            horizontalInput = -1f;
        }
        else if (Raylib.IsKeyDown(KeyboardKey.Right) || Raylib.IsKeyDown(KeyboardKey.D))
        {
            horizontalInput = 1f;
        }

        // Update facing direction based on horizontal input or position relative to screen
        if (horizontalInput != 0)
        {
            facingDirection = horizontalInput;
        }
        else
        {
            // Face right by default (towards asteroids)
            facingDirection = 1f;
        }

        // Handle jump input
        if (Raylib.IsKeyPressed(KeyboardKey.Space) || Raylib.IsKeyPressed(KeyboardKey.Up))
        {
            Velocity = new Vector2(Velocity.X, JumpVelocity);
        }

        // Update position
        Position = new Vector2(Position.X, Position.Y + Velocity.Y * deltaTime);

        // Keep spaceship within screen bounds
        if (Position.Y < 0)
        {
            Position = new Vector2(Position.X, 0);
            Velocity = new Vector2(Velocity.X, 0);
        }
    }

    public override void Render()
    {
        if (texture.HasValue)
        {
            // Get texture dimensions for source rectangle
            int texWidth = (int)(Width / Scale);
            int texHeight = (int)(Height / Scale);

            Rectangle source = new Rectangle(0, 0, facingDirection > 0 ? texWidth : -texWidth, texHeight);
            Rectangle dest = new Rectangle(Position.X + Width / 2, Position.Y + Height / 2, Width, Height);
            Vector2 origin = new Vector2(Width / 2, Height / 2);
            float rotation = 0f;

            Raylib.DrawTexturePro(texture.Value, source, dest, origin, rotation, Color.White);
        }
        else
        {
            // Fallback rectangle if texture not loaded
            Raylib.DrawRectangle((int)Position.X, (int)Position.Y, (int)Width, (int)Height, Color.Blue);
        }
    }

    public void Reset(float x, float y)
    {
        Position = new Vector2(x, y);
        Velocity = new Vector2(0, 0);
        facingDirection = 1f;
    }
}

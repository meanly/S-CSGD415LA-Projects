using Raylib_cs;

namespace Finals_SpaceFlap.Systems;

public class ParallaxBackground
{
    private Texture2D? backgroundTexture;
    private float scrollSpeed;
    private float backgroundX1;
    private float backgroundX2;
    private readonly int screenWidth;
    private readonly int screenHeight;

    public ParallaxBackground(int screenWidth, int screenHeight, float scrollSpeed = 50f)
    {
        this.screenWidth = screenWidth;
        this.screenHeight = screenHeight;
        this.scrollSpeed = scrollSpeed;
        LoadTexture();
        Reset();
    }

    private void LoadTexture()
    {
        string bgPath = Path.Combine("Assets", "bg 2.png");
        if (File.Exists(bgPath))
        {
            Image image = Raylib.LoadImage(bgPath);
            backgroundTexture = Raylib.LoadTextureFromImage(image);
            Raylib.UnloadImage(image);
        }
    }

    public void Update(float deltaTime)
    {
        if (backgroundTexture.HasValue)
        {
            backgroundX1 -= scrollSpeed * deltaTime;
            backgroundX2 -= scrollSpeed * deltaTime;

            // Reset position when background scrolls off screen
            if (backgroundX1 <= -screenWidth)
            {
                backgroundX1 = backgroundX2 + screenWidth;
            }
            if (backgroundX2 <= -screenWidth)
            {
                backgroundX2 = backgroundX1 + screenWidth;
            }
        }
    }

    public void Render()
    {
        if (backgroundTexture.HasValue)
        {
            // Draw two copies of the background for seamless scrolling
            Raylib.DrawTexture(backgroundTexture.Value, (int)backgroundX1, 0, Color.White);
            Raylib.DrawTexture(backgroundTexture.Value, (int)backgroundX2, 0, Color.White);
        }
        else
        {
            // Fallback: solid color background
            Raylib.ClearBackground(Color.Black);
        }
    }

    private void Reset()
    {
        backgroundX1 = 0;
        backgroundX2 = screenWidth;
    }
}


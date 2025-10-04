using Raylib_cs;
using System;

public class FoodPellet
{
    internal float x;
    internal float y;
    public int nutrition;
    internal bool isActive = true;
    public Texture2D sprite;
    protected float fallSpeed = 30f; // pixels per second

    public FoodPellet(float startX, float startY, int nutritionValue)
    {
        x = startX;
        y = startY;
        nutrition = nutritionValue;
        sprite = Raylib.LoadTexture("img/pelletBig.png");
    }
    public virtual void Update()
    {
        if (!isActive) return;

        // Simulate gravity
        y += fallSpeed * Raylib.GetFrameTime();

        // Check if it hits the bottom of the screen
        if (y >= Raylib.GetScreenHeight())
        {
            Destroy();
        }
    }
    public virtual void Draw()
    {
        //if (!isActive) return;

        Raylib.DrawTexture(sprite, (int)x, (int)y, Color.White);
    }
    protected virtual void Destroy()
    {
        isActive = false;
    }

}
public class BigFoodPellet : FoodPellet
{
    public BigFoodPellet(float startX, float startY) : base(startX, startY, 10)
    {
        //sprite = Raylib.LoadTexture("res/pelletBig.png");
    }
}
public class SmallFoodPellet : FoodPellet
{
    public SmallFoodPellet(float startX, float startY) : base(startX, startY, 5)
    {
        //sprite = Raylib.LoadTexture("res/pelletsmall.png");
    }
}
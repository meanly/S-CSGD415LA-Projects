using System.Numerics;
using Raylib_cs;
public class Coin
{
    public Texture2D sprite;
    public float x, y;
    public float scale = 0.5f;
    public int Value { get; set; }
    public bool isActive = true;

    public Coin(float startX, float startY, int value)
    {
        sprite = Raylib.LoadTexture("res/coinSilver.png");
        x = startX; y = startY;
        Value = value;
    }

    public virtual void Update()
    {
        y += 1; // fall slowly
        if (y > Raylib.GetScreenHeight() || y < 20) isActive = false;
    }

    public void Draw()
    {
        Raylib.DrawTexture(sprite, (int)x, (int)y, Color.White);
    }

    public bool IsClicked(Vector2 mouse)
    {
        return Raylib.CheckCollisionPointRec(mouse, new Rectangle(x, y, sprite.Width, sprite.Height));
    }
}
public class SilverCoin : Coin
{
    public SilverCoin(float startX, float startY, int value) : base(startX, startY, 5) { }
}
public class GoldCoin : Coin
{
    public GoldCoin(float startX, float startY, int value) : base(startX, startY, 10) { }
}
public class Poop : Coin
{
    protected Random rand = new Random();
    public Poop(float startX, float startY, int value) : base(startX, startY, 0) { }
    override public void Update()
    {
        y += (0.03f * rand.Next(-11, 11)); // fall faster
    }
}
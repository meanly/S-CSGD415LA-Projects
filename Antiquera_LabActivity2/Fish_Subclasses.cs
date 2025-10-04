using Raylib_cs;
class BasicFish : Fish
{
    private float coinTimer = 0;

    public BasicFish(float startX, float startY, AudioHandler audioHandler) : base(startX, startY, null)
    {
        lifespan = Random.Shared.Next(80, 140);
        this.scale = 0.8f;
    }


    // public override void Update(List<Coin> coins)
    // {
    //     base.Update(coins);
    //     base.Update(coins, null, "BasicFish");
    //     // Drops a coin every 5 sec
    //     coinTimer -= Raylib.GetFrameTime();
    //     if (coinTimer <= 0)
    //     {
    //         coins.Add(new Coin(x + 20, y + 20, 25));
    //         coinTimer = 5f;
    //     }

    // }
}

class CarnivoreFish : Fish
{
    public CarnivoreFish(float startX, float startY, AudioHandler audioHandler) : base(startX, startY, null)
    {
        lifespan = 10;
    }

}
class JanitorFish : Fish
{
    public JanitorFish(float startX, float startY, AudioHandler audioHandler) : base(startX, startY, null)
    {

    }

    // public override void Update(List<Coin> coins)
    // {
    //     base.Update(coins);
    //     // TODO: Eat smaller fish â†' students implement
    // }
}

// Poro Fish Classes
class PoroGirl : Fish
{
    public PoroGirl(float startX, float startY, AudioHandler audioHandler, TextureHandler textureHandler) : base(startX, startY, audioHandler)
    {
        sprite = textureHandler.GetPoroGirlSprite();
        lifespan = Random.Shared.Next(100, 160);
        this.scale = 0.7f;
        maxHp = 40;
        hp = maxHp;
    }
}

class PoroKing : Fish
{
    public PoroKing(float startX, float startY, AudioHandler audioHandler, TextureHandler textureHandler) : base(startX, startY, audioHandler)
    {
        sprite = textureHandler.GetPoroKingSprite();
        lifespan = Random.Shared.Next(200, 300);
        this.scale = 1.2f;
        maxHp = 60;
        hp = maxHp;
    }
}

class PoroPirate : Fish
{
    public PoroPirate(float startX, float startY, AudioHandler audioHandler, TextureHandler textureHandler) : base(startX, startY, audioHandler)
    {
        sprite = textureHandler.GetPoroPirateSprite();
        lifespan = Random.Shared.Next(80, 120);
        this.scale = 0.9f;
        maxHp = 50;
        hp = maxHp;
    }
}

class PoroNerd : Fish
{
    public PoroNerd(float startX, float startY, AudioHandler audioHandler, TextureHandler textureHandler) : base(startX, startY, audioHandler)
    {
        sprite = textureHandler.GetPoroNerdSprite();
        lifespan = Random.Shared.Next(120, 180);
        this.scale = 0.8f;
        maxHp = 35;
        hp = maxHp;
    }
}
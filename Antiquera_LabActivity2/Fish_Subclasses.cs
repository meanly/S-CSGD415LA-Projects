using Raylib_cs;
class BasicFish : Fish
{
    private float coinTimer = 0;

    public BasicFish(float startX, float startY, AudioHandler audioHandler) : base(startX, startY, null)
    {
        lifespan = Random.Shared.Next(80, 140);
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
    //     // TODO: Eat smaller fish â†’ students implement
    // }
}
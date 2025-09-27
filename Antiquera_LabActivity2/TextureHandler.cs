using Raylib_cs;
using System;

class TextureHandler
{
    Texture2D bg;
    protected Texture2D normalFishSprite = Raylib.LoadTexture("img/bigbluefin.png");
    protected Texture2D carnivorousFishSprite = Raylib.LoadTexture("img/Tai.png");
    protected Texture2D janitorFishSprite = Raylib.LoadTexture("img/Aji.png");
    protected Texture2D silverCoinSprite = Raylib.LoadTexture("img/coinSilver.png");
    protected Texture2D goldCoinSprite = Raylib.LoadTexture("img/coinGold.png");
    protected Texture2D fishPooSprite = Raylib.LoadTexture("img/FishPoo.png");
    protected Texture2D smallPelletSprite = Raylib.LoadTexture("img/pelletsmall.png");
    protected Texture2D bigPelletSprite = Raylib.LoadTexture("img/pelletBig.png");

    List<Fish> fishes;
    List<Coin> coins;
    List<FoodPellet> foodPellets;

    public TextureHandler(List<Fish> fishes, List<Coin> coins, List<FoodPellet> foodPellets)
    {
        this.fishes = fishes;
        this.coins = coins;
        this.foodPellets = foodPellets;
        bg = Raylib.LoadTexture("img/background.png");//background.png
        Raylib.DrawTexture(bg, 0, 0, Color.White);

    }
    public void DrawAll(windowSize gameSize)
    {

        Raylib.DrawTexture(bg, 0, 0, Color.White);
        Raylib.DrawRectangle(0, 0, gameSize.width, 50, Color.DarkBlue);//Shop
        foreach (var fish in fishes)
        {
            if (fish is BasicFish)
                fish.sprite = normalFishSprite;
            else if (fish is CarnivoreFish)
                fish.sprite = carnivorousFishSprite;
            else
                fish.sprite = janitorFishSprite;
            fish.Draw();
        }
        foreach (var coin in coins)
        {
            if (coin is SilverCoin)
                coin.sprite = silverCoinSprite;
            else if (coin is GoldCoin)
                coin.sprite = goldCoinSprite;
            else
                coin.sprite = fishPooSprite;
            coin.Draw();
        }
        foreach (var foodPellet in foodPellets)
        {
            if (foodPellet.nutrition == 5)
            {
                foodPellet.sprite = smallPelletSprite;
            }
            else
            {
                foodPellet.sprite = bigPelletSprite;
            }
            foodPellet.Draw();
        }
    }
    public void draw(List<object> instances)
    {
        //Draw all of the sprites on the screen and on the correct coordinates
        foreach (var instance in instances)
        {
            //instance.Draw();
        }
        if (bg.Id == 0)
            Console.WriteLine("Failed to load background.png");
    }
    public void dispose(List<object> instances)
    {
        //Unload all of the textures
        foreach (var instance in instances)
        {
            // Raylib.UnloadTexture(instance.sprite);
        }
    }
    public void dispose()
    {
        Raylib.UnloadTexture(bg);
    }
}
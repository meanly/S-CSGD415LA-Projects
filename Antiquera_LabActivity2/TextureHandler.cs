using Raylib_cs;
using System;

class TextureHandler
{
    // Parallax layers
    Texture2D[] parallaxLayers = new Texture2D[16];
    float[] parallaxOffsets = new float[16];
    float[] parallaxSpeeds = new float[16];

    protected Texture2D normalFishSprite = Raylib.LoadTexture("img/poro_regular.png");
    protected Texture2D silverCoinSprite = Raylib.LoadTexture("img/coinSilver.png");
    protected Texture2D goldCoinSprite = Raylib.LoadTexture("img/coinGold.png");
    protected Texture2D fishPooSprite = Raylib.LoadTexture("img/poop.png");
    protected Texture2D smallPelletSprite = Raylib.LoadTexture("img/poro_snax.png");
    protected Texture2D bigPelletSprite = Raylib.LoadTexture("img/pelletBig.png");

    // Poro Fish Sprites
    protected Texture2D poroGirlSprite = Raylib.LoadTexture("img/poro_girl.png");
    protected Texture2D poroKingSprite = Raylib.LoadTexture("img/poro_king.png");

    protected Texture2D poroPirateSprite = Raylib.LoadTexture("img/poro_pirate.png");
    protected Texture2D poroNerdSprite = Raylib.LoadTexture("img/poro_nerd.png");

    List<Fish> fishes;
    List<Coin> coins;
    List<FoodPellet> foodPellets;

    public TextureHandler(List<Fish> fishes, List<Coin> coins, List<FoodPellet> foodPellets)
    {
        this.fishes = fishes;
        this.coins = coins;
        this.foodPellets = foodPellets;

        // Load parallax layers and set speeds
        for (int i = 0; i < 16; i++)
        {
            parallaxLayers[i] = Raylib.LoadTexture($"img/dynamic_bg/{i + 1}.png");
            parallaxOffsets[i] = 0;
            // Farther layers move slower, closer layers move faster
            parallaxSpeeds[i] = 0.2f + i * 0.1f;
        }
    }


    public void DrawAll(windowSize gameSize)
    {
        float scaleX, scaleY;
        for (int i = 0; i < parallaxLayers.Length; i++)
        {
            scaleX = (float)gameSize.width / parallaxLayers[i].Width;
            scaleY = (float)gameSize.height / parallaxLayers[i].Height;

            parallaxOffsets[i] -= parallaxSpeeds[i];
            if (parallaxOffsets[i] <= -parallaxLayers[i].Width * scaleX)
                parallaxOffsets[i] += parallaxLayers[i].Width * scaleX;

            // Draw first copy
            Raylib.DrawTextureEx(
                parallaxLayers[i],
                new System.Numerics.Vector2(parallaxOffsets[i], 0),
                0,
                MathF.Max(scaleX, scaleY),
                Color.White
            );
            // Draw second copy for seamless looping
            Raylib.DrawTextureEx(
                parallaxLayers[i],
                new System.Numerics.Vector2(parallaxOffsets[i] + parallaxLayers[i].Width * scaleX, 0),
                0,
                MathF.Max(scaleX, scaleY),
                Color.White
            );
        }

        Raylib.DrawRectangle(0, 0, gameSize.width, 50, Color.DarkBlue);//Shop

        foreach (var fish in fishes)
        {
            if (fish is BasicFish)
                fish.sprite = normalFishSprite;
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
    }

    public void DisposeTextures()
    {
        // Unload parallax layers
        for (int i = 0; i < parallaxLayers.Length; i++)
        {
            Raylib.UnloadTexture(parallaxLayers[i]);
        }

        // Unload other textures
        Raylib.UnloadTexture(normalFishSprite);
        Raylib.UnloadTexture(silverCoinSprite);
        Raylib.UnloadTexture(goldCoinSprite);
        Raylib.UnloadTexture(fishPooSprite);
        Raylib.UnloadTexture(smallPelletSprite);
        Raylib.UnloadTexture(bigPelletSprite);

        // Unload Poro sprites
        Raylib.UnloadTexture(poroGirlSprite);
        Raylib.UnloadTexture(poroKingSprite);
        Raylib.UnloadTexture(poroPirateSprite);
        Raylib.UnloadTexture(poroNerdSprite);
    }

    // Poro Fish Sprite Getters
    public Texture2D GetPoroGirlSprite() => poroGirlSprite;
    public Texture2D GetPoroKingSprite() => poroKingSprite;
    public Texture2D GetPoroPirateSprite() => poroPirateSprite;
    public Texture2D GetPoroNerdSprite() => poroNerdSprite;
}
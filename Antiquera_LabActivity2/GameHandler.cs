using Raylib_cs;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;

public class GameHandler
{
    public int money;
    public List<Fish> fishes;
    public List<Coin> coins;
    public List<FoodPellet> foodPellets;
    windowSize gameSize;
    TextureHandler textureHandler;
    AudioHandler audioHandler;
    AISystem aiSystem;
    ShopHandler shopHandler;


    public GameHandler(int _width, int _height)
    {
        gameSize = new windowSize { width = _width, height = _height };
        newGame();
    }
    public void newGame()
    {
        money = 100;
        fishes = new List<Fish>();
        coins = new List<Coin>();
        foodPellets = new List<FoodPellet>();

        textureHandler = new TextureHandler(fishes, coins, foodPellets);
        audioHandler = new AudioHandler();
        audioHandler.LoadAudio();
        aiSystem = new AISystem(fishes, foodPellets, coins);
        //shopHandler = new ShopHandler(gameSize);

        ///Add 1 Fish to start
        fishes.Add(new BasicFish(200, 200, audioHandler));
    }

    public void Update()
    {
        //Check for Game Over conditions
        if (money <= 0 || fishes.Count == 0)
        {
            // Game Over Logic
            Raylib.DrawText("GAME OVER! Press R to Restart", gameSize.width / 4, 300, 40, Color.Red);
            if (Raylib.IsKeyPressed(KeyboardKey.R))
            {
                audioHandler.UnloadAudio();
                newGame();
            }
            return;
        }

        //foreach (var fish in fishes) fish.Update(coins);
        foreach (var coin in coins) coin.Update();
        if (foodPellets.RemoveAll(fp => fp.isActive == false) >= 1)
        {
            Console.WriteLine("Food Pellet Removed");
        }
        if (fishes.RemoveAll(fp => fp.isActive == false) >= 1)
        {
            Console.WriteLine("A Fish has died");
        }
        if (coins.RemoveAll(fp => fp.isActive == false) >= 1)
        {
            Console.WriteLine("A poo has perished");
        }
        foreach (var foodPellet in foodPellets)
            foodPellet.Update();
        handleInput();
        audioHandler.Update();
        foreach (var fish in fishes)
        {
            fish.Update(coins, foodPellets, "BasicFish");
            fish.Draw();
        }
        aiSystem.Update();
        textureHandler.DrawAll(gameSize);
        Raylib.DrawText($"Money: {money}", 10, 10, 20, Color.Yellow);

        foreach (var foodPellet in foodPellets)
        {
            foodPellet.Draw();
        }
    }
    public void handleInput()
    {
        if (Raylib.IsMouseButtonPressed(MouseButton.Left))
        {
            Vector2 mousePos = Raylib.GetMousePosition();
            for (int i = coins.Count - 1; i >= 0; i--)
            {
                if (coins[i].IsClicked(mousePos))
                {
                    money += coins[i].Value;
                    //Console.WriteLine($"Coin Collected! New Money: {coins[i].Value}");
                    if (coins[i].Value == 0) audioHandler.PlaySound("drop");
                    else if (coins[i].Value == 5) audioHandler.PlaySound("coin1");
                    else audioHandler.PlaySound("coin2");
                    coins.RemoveAt(i);
                }
            }
        }
        if (Raylib.IsKeyPressed(KeyboardKey.B))
        {
            if (money >= 20)
            {
                fishes.Add(new BasicFish(100, 100, audioHandler));
                audioHandler.PlaySound("buyFish");
                money -= 20;
            }
        }
        {
            //shopHandler.OpenShop(money);
        }

        // Right-click to drop food pellet
        if (Raylib.IsMouseButtonPressed(MouseButton.Right))
        {
            Vector2 mousePos = Raylib.GetMousePosition();
            if (money >= 5) // Cost of food pellet
            {
                foodPellets.Add(new FoodPellet(mousePos.X, mousePos.Y, 5));
                audioHandler.PlaySound("click");
                money -= 3;
            }
        }
    }
}
using Raylib_cs;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;

public class GameHandler
{
    public int money;
    public List<Fish> fishes;
    public List<Coin> coins;
    public List<FoodPellet> foodPellets;
    public List<HeartParticle> heartParticles;
    public bool IsAnimatingToPoroKing => isAnimatingToPoroKing;
    windowSize gameSize;
    TextureHandler textureHandler;
    AudioHandler audioHandler;
    AISystem aiSystem;
    ShopUI shopUI;
    private float buyTextTimer = 0f;

    // Camera and animation system for PoroKing reveal
    private Vector2 cameraOffset = Vector2.Zero;
    private float cameraZoom = 1.0f;
    private bool isAnimatingToPoroKing = false;
    private float animationTimer = 0f;
    private float animationDuration = 6.0f; // 6 seconds for the zoom animation
    private Fish poroKingTarget = null;
    private bool hasPlayedFlash = false;


    public GameHandler(int _width, int _height)
    {
        gameSize = new windowSize { width = _width, height = _height };
        newGame();
    }
    public void newGame()
    {
        money = 1000;
        fishes = new List<Fish>();
        coins = new List<Coin>();
        foodPellets = new List<FoodPellet>();
        heartParticles = new List<HeartParticle>();

        textureHandler = new TextureHandler(fishes, coins, foodPellets);
        audioHandler = new AudioHandler();
        audioHandler.LoadAudio();
        aiSystem = new AISystem(fishes, foodPellets, coins, audioHandler, this);
        shopUI = new ShopUI(gameSize.width, gameSize.height);
        //shopHandler = new ShopHandler(gameSize);

        ///Add 1 Fish to start
        fishes.Add(new BasicFish(200, 200, audioHandler));
    }

    public void Update()
    {
        float deltaTime = Raylib.GetFrameTime();

        // Update PoroKing animation if active
        UpdatePoroKingAnimation(deltaTime);

        //foreach (var fish in fishes) fish.Update(coins);
        foreach (var coin in coins) coin.Update();
        if (foodPellets.RemoveAll(fp => fp.isActive == false) >= 1)
        {
            Console.WriteLine("Food Pellet Removed");
        }
        int removedFish = fishes.RemoveAll(fp => fp.isActive == false);
        if (removedFish >= 1)
        {
            Console.WriteLine($"Removed {removedFish} dead fish from game");
        }
        if (coins.RemoveAll(fp => fp.isActive == false) >= 1)
        {
            Console.WriteLine("A poo has perished");
        }
        if (heartParticles.RemoveAll(hp => hp.isActive == false) >= 1)
        {
            // Heart particles removed (no console message needed)
        }
        foreach (var foodPellet in foodPellets)
            foodPellet.Update();
        foreach (var heartParticle in heartParticles)
            heartParticle.Update();
        handleInput();
        audioHandler.Update();
        aiSystem.Update(); // Update AI first to set fish states

        // Update shop UI
        shopUI.Update();

        // Update buy text timer
        buyTextTimer += Raylib.GetFrameTime();

        // Handle shop purchases
        if (shopUI.TryBuyItem(money, out string fishType, out int cost))
        {
            BuyFish(fishType, cost);
        }
        foreach (var fish in fishes)
        {
            fish.Update(coins, foodPellets, "BasicFish");
        }

        // Apply camera transformation for PoroKing animation
        if (isAnimatingToPoroKing)
        {
            // Create camera for zoom effect
            Camera2D camera = new Camera2D();
            camera.Target = new Vector2(poroKingTarget.x, poroKingTarget.y);
            camera.Offset = new Vector2(gameSize.width / 2, gameSize.height / 2);
            camera.Rotation = 0f;
            camera.Zoom = cameraZoom;

            Raylib.BeginMode2D(camera);
        }

        // Draw all game objects with camera transformation
        foreach (var fish in fishes)
        {
            fish.Draw();
        }
        textureHandler.DrawAll(gameSize);
        foreach (var foodPellet in foodPellets)
        {
            foodPellet.Draw();
        }
        foreach (var heartParticle in heartParticles)
        {
            heartParticle.Draw();
        }

        if (isAnimatingToPoroKing)
        {
            Raylib.EndMode2D();

            // Add dramatic screen flash effect
            float flashProgress = animationTimer / 0.5f; // Flash for first 0.5 seconds
            if (flashProgress < 1.0f)
            {
                float flashAlpha = (float)(Math.Sin(flashProgress * Math.PI * 4) * (1.0 - flashProgress) * 0.8);
                // Clamp alpha value to valid range (0.0 to 1.0)
                flashAlpha = Math.Max(0.0f, Math.Min(1.0f, flashAlpha));
                int alphaValue = (int)(flashAlpha * 255);
                // Ensure alpha value is within valid byte range
                alphaValue = Math.Max(0, Math.Min(255, alphaValue));
                Color flashColor = new Color(255, 255, 255, alphaValue);
                Raylib.DrawRectangle(0, 0, gameSize.width, gameSize.height, flashColor);
            }
        }

        // Draw money text (only when not animating to PoroKing)
        if (!isAnimatingToPoroKing)
        {
            Raylib.DrawText($"Money: {money}", 10, 10, 20, Color.Yellow);
        }

        // Draw shop UI (only when not animating to PoroKing)
        if (!isAnimatingToPoroKing)
        {
            shopUI.Draw(money);

            // Draw pulsing buy text (only when shop is closed)
            if (!shopUI.IsOpen)
            {
                DrawPulsingBuyText();
            }
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
        // B key is now used for shop (handled in ShopUI)

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

    public void SpawnHeartParticles(float x, float y)
    {
        // Spawn 3-5 heart particles
        int particleCount = new Random().Next(3, 6);
        for (int i = 0; i < particleCount; i++)
        {
            heartParticles.Add(new HeartParticle(x, y));
        }
    }

    private void BuyFish(string fishType, int cost)
    {
        if (money >= cost)
        {
            money -= cost;
            audioHandler.PlaySound("buyFish");

            // Spawn fish at random position
            float spawnX = new Random().Next(100, gameSize.width - 100);
            float spawnY = new Random().Next(100, gameSize.height - 100);

            switch (fishType)
            {
                case "BasicFish":
                    fishes.Add(new BasicFish(spawnX, spawnY, audioHandler));
                    break;
                case "PoroGirl":
                    fishes.Add(new PoroGirl(spawnX, spawnY, audioHandler, textureHandler));
                    break;
                case "PoroKing":
                    var poroKing = new PoroKing(spawnX, spawnY, audioHandler, textureHandler);
                    fishes.Add(poroKing);
                    // Start zoom animation to PoroKing
                    StartPoroKingAnimation(poroKing);
                    break;
                case "PoroPirate":
                    fishes.Add(new PoroPirate(spawnX, spawnY, audioHandler, textureHandler));
                    break;
                case "PoroNerd":
                    fishes.Add(new PoroNerd(spawnX, spawnY, audioHandler, textureHandler));
                    break;
            }

            Console.WriteLine($"Bought {fishType} for {cost} coins!");
        }
    }

    private void DrawPulsingBuyText()
    {
        string buyText = "Press [B] to buy";
        int fontSize = 24;

        // Calculate pulsing alpha (0.3 to 1.0)
        float pulseSpeed = 2f; // Speed of the pulse
        float alpha = (float)(0.3 + 0.7 * (Math.Sin(buyTextTimer * pulseSpeed) + 1) / 2);
        int alphaValue = (int)(alpha * 255);

        // Create color with pulsing alpha
        Color textColor = new Color(255, 255, 255, alphaValue); // White with pulsing alpha

        // Calculate position (center bottom)
        int textWidth = Raylib.MeasureText(buyText, fontSize);
        int x = (gameSize.width - textWidth) / 2;
        int y = gameSize.height - 50; // 50 pixels from bottom

        // Draw the text
        Raylib.DrawText(buyText, x, y, fontSize, textColor);
    }

    private void StartPoroKingAnimation(Fish poroKing)
    {
        isAnimatingToPoroKing = true;
        animationTimer = 0f;
        poroKingTarget = poroKing;
        cameraOffset = Vector2.Zero;
        cameraZoom = 1.0f;
        hasPlayedFlash = false;

        // Play dramatic reveal sound
        audioHandler.PlaySound("poroKingReveal");

        Console.WriteLine($"Starting PoroKing animation at position ({poroKing.x}, {poroKing.y})");
    }

    private void UpdatePoroKingAnimation(float deltaTime)
    {
        if (!isAnimatingToPoroKing || poroKingTarget == null) return;

        animationTimer += deltaTime;
        float progress = animationTimer / animationDuration;

        if (progress >= 1.0f)
        {
            // Animation complete, transition to win screen
            isAnimatingToPoroKing = false;
            Console.WriteLine("PoroKing animation complete, transitioning to win screen");
            GameState.SetState(GameState.State.Win);
            return;
        }

        // Smooth zoom and camera movement with better easing
        float zoomProgress = (float)(1.0 - Math.Pow(1.0 - progress, 3)); // Ease out cubic
        cameraZoom = 1.0f + (zoomProgress * 3.0f); // Zoom from 1x to 4x for more dramatic effect

        // Calculate camera offset to center PoroKing on screen
        Vector2 screenCenter = new Vector2(gameSize.width / 2, gameSize.height / 2);
        Vector2 poroKingCenter = new Vector2(poroKingTarget.x, poroKingTarget.y);
        Vector2 targetOffset = screenCenter - poroKingCenter;

        cameraOffset = Vector2.Lerp(Vector2.Zero, targetOffset, zoomProgress);

        Console.WriteLine($"Animation progress: {progress:F2}, Zoom: {cameraZoom:F2}, Offset: ({cameraOffset.X:F1}, {cameraOffset.Y:F1})");
    }
}
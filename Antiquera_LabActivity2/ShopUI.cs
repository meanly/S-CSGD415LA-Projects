using Raylib_cs;
using System.Numerics;

public class ShopUI
{
    private windowSize gameSize;
    private bool isOpen;
    private int selectedItem;
    private readonly ShopItem[] shopItems;

    public ShopUI(int width, int height)
    {
        gameSize = new windowSize { width = width, height = height };
        isOpen = false;
        selectedItem = 0;

        // Initialize shop items
        shopItems = new ShopItem[]
        {
            new ShopItem("Basic Poro", "Regular poro fish", 20, "BasicFish"),
            new ShopItem("Poro Girl", "Cleans up poops automatically", 50, "PoroGirl"),
            new ShopItem("Poro Nerd", "Generates gold coins", 75, "PoroNerd"),
            new ShopItem("Poro Pirate", "Carnivore - hunts regular poros for coins", 100, "PoroPirate"),
            new ShopItem("Poro King", "WINNING CONDITION!", 500, "PoroKing")
        };
    }

    public void Update()
    {
        // Toggle shop with B key
        if (Raylib.IsKeyPressed(KeyboardKey.B))
        {
            isOpen = !isOpen;
        }

        if (!isOpen) return;

        // Navigation
        if (Raylib.IsKeyPressed(KeyboardKey.Up) || Raylib.IsKeyPressed(KeyboardKey.W))
        {
            selectedItem = (selectedItem - 1 + shopItems.Length) % shopItems.Length;
        }

        if (Raylib.IsKeyPressed(KeyboardKey.Down) || Raylib.IsKeyPressed(KeyboardKey.S))
        {
            selectedItem = (selectedItem + 1) % shopItems.Length;
        }
    }

    public void Draw(int playerMoney)
    {
        if (!isOpen) return;

        // Draw semi-transparent background
        Raylib.DrawRectangle(0, 0, gameSize.width, gameSize.height, new Color(0, 0, 0, 150));

        // Draw shop window
        int shopWidth = 600;
        int shopHeight = 500;
        int shopX = (gameSize.width - shopWidth) / 2;
        int shopY = (gameSize.height - shopHeight) / 2;

        Raylib.DrawRectangle(shopX, shopY, shopWidth, shopHeight, new Color(50, 50, 80, 255));
        Raylib.DrawRectangleLines(shopX, shopY, shopWidth, shopHeight, Color.White);

        // Draw title
        string title = "PORO SHOP";
        int titleWidth = Raylib.MeasureText(title, 50);
        Raylib.DrawText(title, shopX + (shopWidth - titleWidth) / 2, shopY + 20, 50, Color.Yellow);

        // Draw money
        string moneyText = $"Money: {playerMoney}";
        Raylib.DrawText(moneyText, shopX + 20, shopY + 80, 30, Color.Lime);

        // Draw shop items
        int startY = shopY + 120;
        int itemHeight = 70;

        for (int i = 0; i < shopItems.Length; i++)
        {
            var item = shopItems[i];
            int itemY = startY + i * itemHeight;

            // Highlight selected item
            if (i == selectedItem)
            {
                Raylib.DrawRectangle(shopX + 10, itemY - 5, shopWidth - 20, itemHeight - 10, new Color(100, 100, 150, 100));
                Raylib.DrawRectangleLines(shopX + 10, itemY - 5, shopWidth - 20, itemHeight - 10, Color.Yellow);
            }

            // Draw item name
            Color nameColor = playerMoney >= item.Price ? Color.White : Color.Red;
            Raylib.DrawText(item.Name, shopX + 20, itemY, 28, nameColor);

            // Draw price
            string priceText = $"{item.Price} coins";
            int priceWidth = Raylib.MeasureText(priceText, 24);
            Raylib.DrawText(priceText, shopX + shopWidth - priceWidth - 20, itemY, 24, Color.Gold);

            // Draw description
            Raylib.DrawText(item.Description, shopX + 20, itemY + 35, 18, Color.LightGray);
        }

        // Draw instructions
        string instructions = "UP/DOWN: Navigate | ENTER: Buy | B: Close";
        int instWidth = Raylib.MeasureText(instructions, 22);
        Raylib.DrawText(instructions, shopX + (shopWidth - instWidth) / 2, shopY + shopHeight - 35, 22, Color.White);
    }

    public bool TryBuyItem(int playerMoney, out string fishType, out int cost)
    {
        fishType = "";
        cost = 0;

        if (!isOpen) return false;

        if (Raylib.IsKeyPressed(KeyboardKey.Enter))
        {
            var selectedShopItem = shopItems[selectedItem];

            if (playerMoney >= selectedShopItem.Price)
            {
                fishType = selectedShopItem.FishType;
                cost = selectedShopItem.Price;
                return true;
            }
        }

        return false;
    }

    public bool IsOpen => isOpen;
}

public class ShopItem
{
    public string Name { get; }
    public string Description { get; }
    public int Price { get; }
    public string FishType { get; }

    public ShopItem(string name, string description, int price, string fishType)
    {
        Name = name;
        Description = description;
        Price = price;
        FishType = fishType;
    }
}

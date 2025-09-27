using Raylib_cs;
using System;
using System.Numerics;
using System.Linq;
public enum FishState
{
    Idle,
    Swim,
    Dead,
    Hungry,
}
public class AISystem
{
    private List<Fish> fishes;
    private List<FoodPellet> pellets;
    private List<Coin> coins;
    private AudioHandler audioHandler;
    private GameHandler gameHandler;

    public AISystem(List<Fish> fishes, List<FoodPellet> pellets, List<Coin> coins, AudioHandler audioHandler, GameHandler gameHandler)
    {
        this.fishes = fishes;
        this.pellets = pellets;
        this.coins = coins;
        this.audioHandler = audioHandler;
        this.gameHandler = gameHandler;
    }

    public void Update()
    {
        float deltaTime = Raylib.GetFrameTime();

        for (int i = fishes.Count - 1; i >= 0; i--)
        {
            Fish fish = fishes[i];

            // Age and HP decay
            fish.age += deltaTime;
            fish.hpTimer -= deltaTime;
            if (fish.hpTimer <= 0)
            {
                fish.hp -= 1;
                fish.hpTimer = 0.5f; // Fish lose 1 HP every 0.5 seconds (faster decay)
            }
            if (fish.hp <= fish.maxHp * 0.9f && fish.hp > 0)
            {
                fish.currentState = FishState.Hungry;
            }
            else
            {
                fish.currentState = FishState.Swim;
            }
            // Growth
            if (!fish.isAdult && fish.age >= fish.lifespan / 2f && !fish.isDead)
            {
                fish.isAdult = true;
                fish.scale = (float)(1.0 + new Random().NextDouble() * 0.2); // 1.0 to 1.2
            }

            // Coin drop (adults only)
            if (fish.isAdult && fish.coinTimer <= 0 && !fish.isDead)
            {
                coins.Add(new GoldCoin(fish.x, fish.y, 10));
                fish.coinTimer = 5f + (float)new Random().NextDouble() * 8f; // 5â€“10 sec
            }
            else if (!fish.isAdult && fish.coinTimer <= 0)
            {
                coins.Add(new SilverCoin(fish.x, fish.y, 5));
                fish.coinTimer = 7f + (float)new Random().NextDouble() * 10f; // 5â€“10 sec

            }
            else
            {
                fish.coinTimer -= deltaTime;
            }

            // Poop drop
            if (fish.poopTimer <= 0 && !fish.isDead)
            {
                fish.playSound("poop");
                coins.Add(new Poop(fish.x, fish.y, 0)); // Poop inherits Coin
                fish.poopTimer = 10f + (float)new Random().NextDouble() * 3f; // 10â€“15 sec
            }
            else
            {
                fish.poopTimer -= deltaTime;
            }
            if (fish.hp <= 0 || fish.age >= fish.lifespan)
            {
                fish.isDead = true;
                fish.currentState = FishState.Dead;
            }

            // AI Behavior
            switch (fish)
            {
                case CarnivoreFish carnivore:
                    HandleCarnivore(carnivore, deltaTime);
                    break;

                case JanitorFish janitor:
                    HandleJanitor(janitor);
                    break;

                case BasicFish basic:
                    HandleBasicFish(basic);
                    break;

                case PoroGirl poroGirl:
                    HandlePoroGirl(poroGirl);
                    break;

                case PoroKing poroKing:
                    HandlePoroKing(poroKing);
                    break;

                case PoroPirate poroPirate:
                    HandlePoroPirate(poroPirate, deltaTime);
                    break;

                case PoroNerd poroNerd:
                    HandlePoroNerd(poroNerd);
                    break;
            }
            fish.Update(coins, pellets, fish.GetType().Name);
        }

        // Handle fish eating with priority (lowest health fish eat first)
        HandleFishEatingPriority();
    }

    private void HandleBasicFish(BasicFish fish)
    {
        // Individual fish eating logic is now handled in HandleFishEatingPriority
    }

    private void HandleCarnivore(CarnivoreFish fish, float deltaTime)
    {
        fish.hungerTimer -= deltaTime;

        if (fish.hungerTimer <= 0)
        {
            fish.currentState = FishState.Hungry;
            Fish prey = FindNearestPrey(fish);
            if (prey != null)
            {
                fish.MoveTowards(prey.x, prey.y);
                if (fish.IsCollidingWith(prey))
                {
                    fish.hp += 50;
                    fishes.Remove(prey);
                    fish.hungerTimer = 15f;
                }
            }
        }
        else
        {
            fish.currentState = FishState.Swim;
        }
    }

    private void HandleJanitor(JanitorFish fish)
    {
        fish.currentState = FishState.Swim;

        Poop targetPoop = FindNearestPoop(fish);
        if (targetPoop != null)
        {
            fish.MoveTowards(targetPoop.x, targetPoop.y);
            if (fish.IsCollidingWith(targetPoop))
            {
                coins.Add(new SilverCoin(fish.x, fish.y, 5));
                coins.Remove(targetPoop);
            }
        }
    }

    private T FindNearestPellet<T>(Fish fish) where T : FoodPellet
    {
        T closest = null;
        float minDist = float.MaxValue;

        foreach (var pellet in pellets)
        {
            if (pellet is T typedPellet)
            {
                float dist = Vector2.Distance(new Vector2(fish.x, fish.y), new Vector2(pellet.x, pellet.y));
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = typedPellet;
                }
            }
        }

        return closest;
    }

    private Fish FindNearestPrey(CarnivoreFish predator)
    {
        Fish closest = null;
        float minDist = float.MaxValue;

        foreach (var fish in fishes)
        {
            if (fish is BasicFish basic && !basic.isAdult)
            {
                float dist = Vector2.Distance(new Vector2(predator.x, predator.y), new Vector2(basic.x, basic.y));
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = basic;
                }
            }
        }

        return closest;
    }

    private Poop FindNearestPoop(Fish fish)
    {
        Poop closest = null;
        float minDist = float.MaxValue;

        foreach (var coin in coins)
        {
            if (coin is Poop poop)
            {
                float dist = Vector2.Distance(new Vector2(fish.x, fish.y), new Vector2(poop.x, poop.y));
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = poop;
                }
            }
        }

        return closest;
    }

    private void HandleFishEatingPriority()
    {
        // Get all hungry fish and sort by health (lowest first)
        var hungryFish = fishes
            .Where(fish => fish.currentState == FishState.Hungry && !fish.isDead)
            .OrderBy(fish => fish.hp)
            .ToList();

        // For each pellet, find the hungriest fish that can eat it
        for (int pelletIndex = pellets.Count - 1; pelletIndex >= 0; pelletIndex--)
        {
            FoodPellet pellet = pellets[pelletIndex];
            if (!pellet.isActive) continue;

            // Find the hungriest fish that can eat this pellet
            foreach (var fish in hungryFish)
            {
                if (fish.IsCollidingWith(pellet))
                {
                    Console.WriteLine($"{fish.GetType().Name} (HP: {fish.hp:F1}) eating pellet");
                    audioHandler.PlaySound("eat");
                    fish.hp = Math.Clamp(fish.hp + 15, 0, fish.maxHp); // Restore 15 HP, cap at maxHp

                    // Spawn heart particles at the fish's position
                    gameHandler.SpawnHeartParticles(fish.x + fish.sprite.Width * fish.scale / 2, fish.y + fish.sprite.Height * fish.scale / 2);

                    pellet.isActive = false; // Remove pellet
                    break; // Only one fish can eat this pellet
                }
            }
        }
    }

    // Poro Fish Handlers
    private void HandlePoroGirl(PoroGirl poroGirl)
    {
        // Poro Girl cleans up poops
        for (int i = coins.Count - 1; i >= 0; i--)
        {
            if (coins[i] is Poop poop && poroGirl.IsCollidingWith(poop))
            {
                Console.WriteLine("Poro Girl cleaned up poop!");
                coins.RemoveAt(i);
                break; // Only clean one poop per frame
            }
        }
    }

    private void HandlePoroKing(PoroKing poroKing)
    {
        // Poro King is the winning condition - no special behavior needed
        // The game will check for PoroKing existence to determine win condition
    }

    private void HandlePoroPirate(PoroPirate poroPirate, float deltaTime)
    {
        // Poro Pirate hunts other fish (carnivore behavior)
        Fish target = FindNearestPrey(poroPirate);
        if (target != null && target != poroPirate)
        {
            poroPirate.MoveTowards(target.x, target.y);
            if (poroPirate.IsCollidingWith(target))
            {
                Console.WriteLine("Poro Pirate ate another fish!");
                poroPirate.hp = Math.Min(poroPirate.maxHp, poroPirate.hp + 30);

                // Drop coins when killing regular poro
                if (target is BasicFish)
                {
                    // Drop 2 gold coins and 1 silver coin
                    coins.Add(new GoldCoin(target.x, target.y, 10));
                    coins.Add(new GoldCoin(target.x + 20, target.y, 10));
                    coins.Add(new SilverCoin(target.x + 10, target.y + 20, 5));
                    Console.WriteLine("Regular poro dropped 2 gold coins and 1 silver coin!");
                }

                target.isActive = false; // Remove the eaten fish
            }
        }
    }

    private void HandlePoroNerd(PoroNerd poroNerd)
    {
        // Poro Nerd generates gold coins more frequently
        if (poroNerd.coinTimer <= 0 && !poroNerd.isDead)
        {
            coins.Add(new GoldCoin(poroNerd.x, poroNerd.y, 15)); // Higher value coins
            poroNerd.coinTimer = 3f; // More frequent than regular fish
        }
    }

    private Fish FindNearestPrey(Fish hunter)
    {
        Fish closest = null;
        float minDist = float.MaxValue;

        foreach (var fish in fishes)
        {
            // Only target BasicFish (regular poros), not other Poro types
            if (fish != hunter && !fish.isDead && fish is BasicFish)
            {
                float dist = Vector2.Distance(new Vector2(hunter.x, hunter.y), new Vector2(fish.x, fish.y));
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = fish;
                }
            }
        }

        return closest;
    }
}
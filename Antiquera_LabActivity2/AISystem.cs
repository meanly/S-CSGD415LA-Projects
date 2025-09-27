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

    public AISystem(List<Fish> fishes, List<FoodPellet> pellets, List<Coin> coins, AudioHandler audioHandler)
    {
        this.fishes = fishes;
        this.pellets = pellets;
        this.coins = coins;
        this.audioHandler = audioHandler;
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
                fish.hpTimer = 1f;
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
        // Get all hungry basic fish and sort by health (lowest first)
        var hungryBasicFish = fishes
            .OfType<BasicFish>()
            .Where(fish => fish.currentState == FishState.Hungry && !fish.isDead)
            .OrderBy(fish => fish.hp)
            .ToList();

        // For each pellet, find the hungriest fish that can eat it
        for (int pelletIndex = pellets.Count - 1; pelletIndex >= 0; pelletIndex--)
        {
            FoodPellet pellet = pellets[pelletIndex];
            if (!pellet.isActive) continue;

            // Find the hungriest fish that can eat this pellet
            foreach (var fish in hungryBasicFish)
            {
                if (fish.IsCollidingWith(pellet))
                {
                    Console.WriteLine($"Basic fish (HP: {fish.hp:F1}) eating pellet");
                    audioHandler.PlaySound("eat");
                    fish.hp = Math.Clamp(fish.hp + 15, 0, fish.maxHp); // Restore 15 HP, cap at maxHp
                    pellet.isActive = false; // Remove pellet
                    break; // Only one fish can eat this pellet
                }
            }
        }
    }
}
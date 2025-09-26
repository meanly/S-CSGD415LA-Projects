using System;
using System.Numerics;
using System.Collections.Generic;
using Raylib_cs;

public class Fish
{
    public Texture2D sprite = Raylib.LoadTexture("res/bigbluefin.png");

    public float x, y;
    public float hp = 20;
    public float hpTimer = 2f;

    public float age = 0;
    //virtual int lifespan = Random.Shared.Next(80, 140); // 20 to 60 seconds

    public virtual float lifespan { get; set; }
    public bool isAdult = false;
    public bool isDead = false;
    public bool isActive = true;
    public float maxHp = 30;

    public float hungerTimer = 1f; // Time until next hunger decrease

    public float scale = 0.5f;
    public int direction = 1; // 1 = right, -1 = left
    public Color color = Color.White;

    public float coinTimer = 5f;
    public float poopTimer = 10f;

    public float directionTimer = 0f;
    public bool triggered = false;

    public FishState currentState = FishState.Swim;

    protected Random rand = new Random();
    public AudioHandler audioHandler;

    public Fish(float startX, float startY, AudioHandler _audioHandler)
    {
        hp = maxHp;
        maxHp = hp;
        audioHandler = _audioHandler;
        x = startX;
        y = startY;
    }

    public virtual void Update(List<Coin> coins, List<FoodPellet> pellets, string type)
    {
        Move(pellets);

        // Clamp to screen
        // x = Math.Clamp(x, 0, Raylib.GetScreenWidth() - (sprite.Width * scale));
        y = Math.Clamp(y, 0, Raylib.GetScreenHeight() - (sprite.Height * scale));

        if (directionTimer <= 0)
        {
            directionTimer = rand.Next(5, 11); // 5â€“10 seconds
        }
    }

    public virtual void Move(List<FoodPellet> pellets)
    {
        switch (currentState)
        {
            case FishState.Swim:
                float swimSpeed = 50f * Raylib.GetFrameTime();
                x += swimSpeed * direction;

                directionTimer -= Raylib.GetFrameTime();
                if (directionTimer <= 0)
                {
                    if (rand.NextDouble() < 0.4) // 40% chance
                    {
                        direction *= -1; // Flip direction
                    }
                    directionTimer = rand.Next(5, 11); // Reset timer
                }

                // Clamp or teleport logic
                if (x > Raylib.GetScreenWidth())
                {
                    x = 0f;
                    y = rand.Next(100, 401);
                }
                else if (x < 0)
                {
                    x = Raylib.GetScreenWidth();
                    y = rand.Next(100, 401);
                }
                break;

            case FishState.Hungry:
                float hungrySpeed = 70f * Raylib.GetFrameTime(); // Faster than normal swim

                // Pellet targeting
                FoodPellet target = FindNearestPellet(pellets);
                if (target != null)
                {
                    Vector2 dir = Vector2.Normalize(new Vector2(target.x - x, target.y - y));
                    x += dir.X * hungrySpeed;
                    y += dir.Y * hungrySpeed;
                    direction = dir.X >= 0 ? 1 : -1;
                }
                else
                {
                    // Swim randomly if no pellet found
                    x += hungrySpeed * direction;

                    // Direction flip logic
                    directionTimer -= Raylib.GetFrameTime();
                    if (directionTimer <= 0)
                    {
                        if (rand.NextDouble() < 0.8) // 80% chance to flip
                        {
                            direction *= -1;
                        }
                        directionTimer = rand.Next(5, 11); // Reset timer (5â€“10 sec)
                    }

                    // Clamp or teleport logic
                    if (x > Raylib.GetScreenWidth())
                    {
                        x = 0f;
                        y = rand.Next(100, 401);
                    }
                    else if (x < 0)
                    {
                        x = Raylib.GetScreenWidth();
                        y = rand.Next(100, 401);
                    }
                }
                break;
            case FishState.Dead:
                hp = 0;
                color = Color.Green;
                direction = 1; // Flip
                y -= 20 * Raylib.GetFrameTime(); // Float upward
                if (!triggered)
                {
                    PlaySingle.PlaySound("FishDeath");
                    triggered = true;
                }
                if (y <= 30)
                {
                    isActive = false;
                }
                break;

        }
    }
    protected virtual FoodPellet FindNearestPellet(List<FoodPellet> pellets)
    {
        FoodPellet closest = null;
        float minDist = float.MaxValue;

        foreach (var pellet in pellets)
        {
            float dist = Vector2.Distance(new Vector2(x, y), new Vector2(pellet.x, pellet.y));
            if (dist < minDist)
            {
                minDist = dist;
                closest = pellet;
            }
        }

        return closest;
    }

    public virtual bool IsCollidingWith(FoodPellet pellet)
    {
        Rectangle fishRect = new Rectangle(x, y, sprite.Width * scale, sprite.Height * scale);
        Rectangle pelletRect = new Rectangle(pellet.x, pellet.y, 8, 8);
        return Raylib.CheckCollisionRecs(fishRect, pelletRect);
    }

    public virtual bool IsCollidingWith(Fish other)
    {
        Rectangle fishRect = new Rectangle(x, y, sprite.Width * scale, sprite.Height * scale);
        Rectangle otherRect = new Rectangle(other.x, other.y, other.sprite.Width * other.scale, other.sprite.Height * other.scale);
        return Raylib.CheckCollisionRecs(fishRect, otherRect);
    }
    public virtual bool IsCollidingWith(Poop other)
    {
        Rectangle fishRect = new Rectangle(x, y, sprite.Width * scale, sprite.Height * scale);
        Rectangle otherRect = new Rectangle(other.x, other.y, other.sprite.Width * other.scale, other.sprite.Height * other.scale);
        return Raylib.CheckCollisionRecs(fishRect, otherRect);
    }
    public virtual bool IsMouseOver()
    {
        Vector2 mouse = Raylib.GetMousePosition();
        Rectangle hitbox = new Rectangle(x, y, sprite.Width * scale, sprite.Height * scale);
        return Raylib.CheckCollisionPointRec(mouse, hitbox);
    }

    public virtual string GetDebugInfo()
    {
        string hunger = hp < 10 ? "Hungry" : hp > 18 ? "Full" : "Normal";
        return $"Age: {MathF.Round(age, 1)}s\nState: {currentState}\nHunger: {hunger}";
    }
    public virtual void Draw()
    {
        float flip = direction == 1 ? -sprite.Width : sprite.Width;

        Rectangle sourceRec = new Rectangle(0, 0, flip, isDead ? -sprite.Height : sprite.Height);
        Rectangle destRec = new Rectangle(x, y, sprite.Width * scale, sprite.Height * scale);
        Vector2 origin = new Vector2((sprite.Width * scale) / 2, (sprite.Height * scale) / 2);
        float rotation = 0f;

        Raylib.DrawTexturePro(sprite, sourceRec, destRec, origin, rotation, color);

        // HP Bar
        Raylib.DrawRectangle((int)x - (int)(sprite.Width * scale) / 2, (int)y + (sprite.Height / 2), 50, 5, Color.DarkGray);
        Raylib.DrawRectangle((int)x - (int)(sprite.Width * scale) / 2, (int)y + (sprite.Height / 2), (int)(Math.Clamp(50 * (hp / maxHp), 0, 50)), 5, Color.Green);
        //Lifespan
        Raylib.DrawRectangle((int)x - (int)(sprite.Width * scale) / 2, (int)y + (sprite.Height / 2) + 6, 50, 5, Color.DarkGray);
        Raylib.DrawRectangle((int)x - (int)(sprite.Width * scale) / 2, (int)y + (sprite.Height / 2) + 6, (int)(Math.Clamp(50 * (age / lifespan), 0, 50)), 5, Color.Yellow);
    }
    public virtual void MoveTowards(float targetX, float targetY)
    {
        Vector2 dir = Vector2.Normalize(new Vector2(targetX - x, targetY - y));
        x += dir.X * 2;
        y += dir.Y * 2;
        direction = dir.X >= 0 ? 1 : -1;
    }
    public void playSound(string sound)
    {
        PlaySingle.PlaySound("FishPoo");
    }
}
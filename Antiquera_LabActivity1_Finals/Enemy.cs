using Raylib_cs;
using System.Numerics;

namespace Antiquera_LabActivity1_Finals;

public class Enemy
{
    private Vector2 position;
    private Direction facingDirection;
    private float speed = 50.0f;
    private int health = 50;
    private int maxHealth = 50;
    private bool isPatrolling;
    private Vector2 patrolTarget;
    private float patrolTimer;
    private float patrolCooldown = 2.0f;
    private Vector2 lastPlayerPosition;
    
    private Animation idleAnimation = null!;
    private Animation walkAnimation = null!;
    private string assetPath = "Tiny Adventure Pack Plus/Enemies/Goblin";

    public Vector2 Position => position;
    public bool IsAlive => health > 0;

    public Enemy(Vector2 startPosition)
    {
        position = startPosition;
        facingDirection = Direction.Down;
        isPatrolling = true;
        patrolTarget = position;
        patrolTimer = 0;

        LoadAnimations();
    }

    private void LoadAnimations()
    {
        // Load goblin animations (using idle and walk - 6 frames per sprite sheet)
        idleAnimation = new Animation(
            $"{assetPath}/Goblin_4sides.png", 6, 0.2f);
        
        walkAnimation = new Animation(
            $"{assetPath}/Walk/Goblin_walk_down.png", 6, 0.15f);
    }

    public void Update(float deltaTime, Vector2 playerPosition)
    {
        if (!IsAlive) return;

        lastPlayerPosition = playerPosition;
        patrolTimer += deltaTime;

        // Simple AI: patrol or chase player
        float distanceToPlayer = Vector2.Distance(position, playerPosition);

        if (distanceToPlayer < 150.0f)
        {
            // Chase player
            Vector2 direction = Vector2.Normalize(playerPosition - position);
            position += direction * speed * deltaTime;

            // Update facing direction
            if (Math.Abs(direction.X) > Math.Abs(direction.Y))
            {
                facingDirection = direction.X > 0 ? Direction.Right : Direction.Left;
            }
            else
            {
                facingDirection = direction.Y > 0 ? Direction.Down : Direction.Up;
            }

            walkAnimation.Update(deltaTime);
        }
        else if (isPatrolling)
        {
            // Patrol behavior
            if (patrolTimer >= patrolCooldown)
            {
                Random random = new Random();
                patrolTarget = new Vector2(
                    position.X + (random.Next(-100, 100)),
                    position.Y + (random.Next(-100, 100))
                );
                patrolTimer = 0;
            }

            Vector2 direction = Vector2.Normalize(patrolTarget - position);
            if (Vector2.Distance(position, patrolTarget) > 10.0f)
            {
                position += direction * speed * 0.5f * deltaTime;
                walkAnimation.Update(deltaTime);
            }
            else
            {
                idleAnimation.Update(deltaTime);
            }
        }
        else
        {
            idleAnimation.Update(deltaTime);
        }
    }

    public void TakeDamage(int damage)
    {
        health = Math.Max(0, health - damage);
    }

    public void Draw()
    {
        if (!IsAlive) return;

        // Determine which animation to use based on state
        Animation currentAnim = walkAnimation;
        float distanceToPlayer = Vector2.Distance(position, lastPlayerPosition);
        if (distanceToPlayer >= 150.0f && !isPatrolling)
        {
            currentAnim = idleAnimation;
        }
        
        Texture2D spriteSheet = currentAnim.GetSpriteSheet();
        if (spriteSheet.Id != 0)
        {
            // Scale up the enemy sprite (3x size to match player)
            float scale = 3.0f;
            Rectangle sourceRect = currentAnim.GetCurrentFrameRect();
            int frameWidth = currentAnim.GetFrameWidth();
            int frameHeight = currentAnim.GetFrameHeight();
            
            Rectangle destRect = new Rectangle(
                position.X, 
                position.Y, 
                frameWidth * scale, 
                frameHeight * scale
            );
            Raylib.DrawTexturePro(spriteSheet, sourceRect, destRect, Vector2.Zero, 0, new Color(255, 255, 255, 255));
        }
        else
        {
            // Fallback: draw colored circle (scaled up)
            Raylib.DrawCircle((int)position.X + 48, (int)position.Y + 48, 48, new Color(255, 0, 0, 255));
        }

        // Draw health bar above enemy
        float healthPercent = (float)health / maxHealth;
        int barWidth = 32;
        int barHeight = 4;
        Raylib.DrawRectangle((int)position.X, (int)position.Y - 8, barWidth, barHeight, new Color(255, 0, 0, 255));
        Raylib.DrawRectangle((int)position.X, (int)position.Y - 8, (int)(barWidth * healthPercent), barHeight, new Color(0, 255, 0, 255));
    }

    public void Unload()
    {
        idleAnimation?.Unload();
        walkAnimation?.Unload();
    }
}


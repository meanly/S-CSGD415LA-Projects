using Raylib_cs;
using System.Numerics;
using System.Collections.Generic;

namespace Antiquera_LabActivity1_Finals;

public class Game
{
    private int screenWidth;
    private int screenHeight;
    private int tileSize;
    private Player player = null!;
    private Camera2D camera;

    // Tile map
    private int[,] tileMap = null!;
    private int mapWidth = 20;
    private int mapHeight = 15;
    private Texture2D grassTexture;

    // Optional: Enemies
    private List<Enemy> enemies;

    // Optional: Projectiles
    private List<Projectile> projectiles;

    public Game(int width, int height, int tileSize)
    {
        this.screenWidth = width;
        this.screenHeight = height;
        this.tileSize = tileSize;

        camera = new Camera2D
        {
            Target = Vector2.Zero,
            Offset = new Vector2(screenWidth / 2, screenHeight / 2),
            Rotation = 0.0f,
            Zoom = 1.0f
        };

        enemies = new List<Enemy>();
        projectiles = new List<Projectile>();
    }

    public void Initialize()
    {
        // Initialize player at center of map
        Vector2 playerStartPos = new Vector2(mapWidth * tileSize / 2, mapHeight * tileSize / 2);
        player = new Player(playerStartPos);

        // Load tile textures
        LoadTileTextures();

        // Generate simple tile map
        GenerateTileMap();

        // Optional: Spawn some enemies
        SpawnEnemies();
    }

    private void LoadTileTextures()
    {
        string tilesetPath = "Tiny Adventure Pack Plus/Tilesets";

        // Load Grass.png texture
        if (System.IO.File.Exists($"{tilesetPath}/Environment Items/Grass.png"))
        {
            grassTexture = Raylib.LoadTexture($"{tilesetPath}/Environment Items/Grass.png");
        }
        else
        {
            // Fallback: create a simple green texture
            grassTexture = new Texture2D();
        }
    }

    private void GenerateTileMap()
    {
        tileMap = new int[mapHeight, mapWidth];

        // Fill entire map with grass (0 = grass)
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                tileMap[y, x] = 0; // All grass
            }
        }
    }

    private void SpawnEnemies()
    {
        // Spawn a few enemies around the map
        Random random = new Random();
        for (int i = 0; i < 3; i++)
        {
            Vector2 enemyPos = new Vector2(
                random.Next(5, mapWidth - 5) * tileSize,
                random.Next(5, mapHeight - 5) * tileSize
            );
            enemies.Add(new Enemy(enemyPos));
        }
    }

    public void Update(float deltaTime)
    {
        // Update player
        player.Update(deltaTime);

        // Update camera to follow player
        camera.Target = player.Position;

        // Update enemies
        foreach (var enemy in enemies)
        {
            enemy.Update(deltaTime, player.Position);
        }

        // Update projectiles
        for (int i = projectiles.Count - 1; i >= 0; i--)
        {
            projectiles[i].Update(deltaTime);

            // Remove projectiles that are expired or out of bounds
            if (projectiles[i].IsExpired() ||
                projectiles[i].Position.X < 0 || projectiles[i].Position.X > mapWidth * tileSize ||
                projectiles[i].Position.Y < 0 || projectiles[i].Position.Y > mapHeight * tileSize)
            {
                projectiles[i].Unload();
                projectiles.RemoveAt(i);
                continue;
            }

            // Check collision with enemies
            foreach (var enemy in enemies)
            {
                if (enemy.IsAlive && CheckCollision(projectiles[i].Position, enemy.Position, 32))
                {
                    enemy.TakeDamage(10);
                    projectiles[i].Unload();
                    projectiles.RemoveAt(i);
                    break;
                }
            }
        }

        // Remove dead enemies
        enemies.RemoveAll(e => !e.IsAlive);

        // Check melee attack collision with enemies
        if (player.CurrentState == PlayerState.AttackingMelee)
        {
            Vector2 attackPos = GetAttackPosition();
            foreach (var enemy in enemies)
            {
                if (enemy.IsAlive && CheckCollision(attackPos, enemy.Position, 40))
                {
                    enemy.TakeDamage(20);
                }
            }
        }

        // Spawn projectile when ranged attack starts
        if (player.JustStartedRangedAttack)
        {
            Vector2 direction = player.GetAttackDirection();
            Vector2 spawnPos = player.Position + direction * 20; // Spawn slightly in front of player
            projectiles.Add(new Projectile(spawnPos, direction, 300.0f));
            player.ResetRangedAttackFlag();
        }
    }

    private Vector2 GetAttackPosition()
    {
        Vector2 offset = Vector2.Zero;
        switch (player.FacingDirection)
        {
            case Direction.Up:
                offset = new Vector2(0, -40);
                break;
            case Direction.Down:
                offset = new Vector2(0, 40);
                break;
            case Direction.Left:
                offset = new Vector2(-40, 0);
                break;
            case Direction.Right:
                offset = new Vector2(40, 0);
                break;
        }
        return player.Position + offset;
    }

    private bool CheckCollision(Vector2 pos1, Vector2 pos2, float radius)
    {
        float distance = Vector2.Distance(pos1, pos2);
        return distance < radius;
    }

    public void Draw()
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(new Color(135, 206, 235, 255));

        Raylib.BeginMode2D(camera);

        // Draw tile map
        DrawTileMap();

        // Draw enemies
        foreach (var enemy in enemies)
        {
            if (enemy.IsAlive)
            {
                enemy.Draw();
            }
        }

        // Draw projectiles
        foreach (var projectile in projectiles)
        {
            projectile.Draw();
        }

        // Draw player
        player.Draw();

        Raylib.EndMode2D();

        // Draw UI (health bar, etc.)
        DrawUI();

        Raylib.EndDrawing();
    }

    private void DrawTileMap()
    {
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                Vector2 position = new Vector2(x * tileSize, y * tileSize);

                if (grassTexture.Id != 0)
                {
                    // Scale the grass texture to fit the tile size
                    Rectangle sourceRect = new Rectangle(0, 0, grassTexture.Width, grassTexture.Height);
                    Rectangle destRect = new Rectangle(position.X, position.Y, tileSize, tileSize);
                    Raylib.DrawTexturePro(grassTexture, sourceRect, destRect, Vector2.Zero, 0, new Color(255, 255, 255, 255));
                }
                else
                {
                    // Fallback: draw colored rectangle
                    Raylib.DrawRectangle((int)position.X, (int)position.Y, tileSize, tileSize, new Color(0, 128, 0, 255));
                }
            }
        }
    }

    private void DrawUI()
    {
        // Draw health bar
        int barWidth = 200;
        int barHeight = 20;
        int barX = 20;
        int barY = 20;

        // Background
        Raylib.DrawRectangle(barX, barY, barWidth, barHeight, new Color(255, 0, 0, 255));

        // Health fill
        float healthPercent = (float)player.CurrentHealth / player.MaxHealth;
        Raylib.DrawRectangle(barX, barY, (int)(barWidth * healthPercent), barHeight, new Color(0, 255, 0, 255));

        // Border
        Raylib.DrawRectangleLines(barX, barY, barWidth, barHeight, new Color(0, 0, 0, 255));

        // Health text
        string healthText = $"Health: {player.CurrentHealth}/{player.MaxHealth}";
        Raylib.DrawText(healthText, barX, barY + 25, 16, new Color(255, 255, 255, 255));

        // Instructions
        Raylib.DrawText("WASD/Arrow Keys: Move", 20, screenHeight - 80, 16, new Color(255, 255, 255, 255));
        Raylib.DrawText("Shift: Sprint", 20, screenHeight - 60, 16, new Color(255, 255, 255, 255));
        Raylib.DrawText("Space: Melee Attack", 20, screenHeight - 40, 16, new Color(255, 255, 255, 255));
        Raylib.DrawText("E: Ranged Attack", 20, screenHeight - 20, 16, new Color(255, 255, 255, 255));
    }

    public void Cleanup()
    {
        player.Unload();

        foreach (var enemy in enemies)
        {
            enemy.Unload();
        }

        foreach (var projectile in projectiles)
        {
            projectile.Unload();
        }

        if (grassTexture.Id != 0) Raylib.UnloadTexture(grassTexture);
    }
}


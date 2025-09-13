using Raylib_cs;
using System.Numerics;
using System.Collections.Generic;

public class Game
{
    public Player player;
    public bool takeDamage = false;

    public List<Enemy> enemies;
    public List<bool> enemiesCaptured;
    Rectangle playerRect;
    List<Rectangle> enemyRects;
    bool isColliding;

    // winning condition
    List<Rectangle> goalBoxes;
    List<bool> goalBoxesCaptured; // track which goal box has captured an enemy

    public Game()
    {
        player = new Player(100, 100);

        enemies = new List<Enemy>();
        enemiesCaptured = new List<bool>();
        enemyRects = new List<Rectangle>();

        // spawn 5 enemies randomly, not too close to the player
        for (int i = 0; i < 5; i++)
        {
            Vector2 pos;
            do
            {
                pos = new Vector2(Raylib.GetRandomValue(0, 800), Raylib.GetRandomValue(0, 600));
            } while (Raymath.Vector2Distance(pos, player.Position) < 100); // avoid spawning near player

            enemies.Add(new Enemy(pos.X, pos.Y));
            enemiesCaptured.Add(false);
            enemyRects.Add(new Rectangle(pos.X, pos.Y, 35, 35));
        }

        // multiple random goal boxes
        goalBoxes = new List<Rectangle>
        {
            new Rectangle(Raylib.GetRandomValue(0, 800), Raylib.GetRandomValue(0, 600), 35, 35),
            new Rectangle(Raylib.GetRandomValue(0, 800), Raylib.GetRandomValue(0, 600), 35, 35),
            new Rectangle(Raylib.GetRandomValue(0, 800), Raylib.GetRandomValue(0, 600), 35, 35)
        };

        goalBoxesCaptured = new List<bool> { false, false, false };
        playerRect = new Rectangle(player.Position.X, player.Position.Y, player.objectSize.width, player.objectSize.height);
    }

    public void Update()
    {
        player.Move();
        playerRect = new Rectangle(player.Position.X, player.Position.Y, player.objectSize.width, player.objectSize.height);

        // Update enemies and check goal box collisions
        for (int i = 0; i < enemies.Count; i++)
        {
            if (!enemiesCaptured[i])
            {
                enemies[i].Update(player.Position);
            }

            enemyRects[i] = new Rectangle(enemies[i].Position.X, enemies[i].Position.Y, 35, 35);

            for (int j = 0; j < goalBoxes.Count; j++)
            {
                if (!enemiesCaptured[i] && Raylib.CheckCollisionRecs(enemyRects[i], goalBoxes[j]))
                {
                    enemiesCaptured[i] = true;
                    enemies[i].Position = new Vector2(-100, -100); // disappear
                    goalBoxesCaptured[j] = true; // mark the goal box as captured
                    break;
                }
            }
        }

        // Check if player collides with any enemy
        isColliding = false;
        for (int i = 0; i < enemyRects.Count; i++)
        {
            if (!enemiesCaptured[i] && Raylib.CheckCollisionRecs(playerRect, enemyRects[i]))
            {
                isColliding = true;
                if (!takeDamage && player.hp > 20)
                {
                    takeDamage = true;
                    player.hp -= 20;
                    enemies[i].Position = new Vector2(1000, 1000);
                    enemiesCaptured[i] = true;
                }
                break;
            }
        }

        if (!isColliding)
            takeDamage = false;
    }

    public void Draw()
    {
        Raylib.DrawText("Jeremy's Game", 10, 10, 30, Color.RayWhite);
        Raylib.DrawText("Health: " + player.hp, 10, 50, 20, Color.RayWhite);

        // Draw goal boxes
        for (int i = 0; i < goalBoxes.Count; i++)
        {
            Color boxColor = goalBoxesCaptured[i] ? Color.Green : Color.Yellow;
            Raylib.DrawRectangleRec(goalBoxes[i], boxColor);
        }

        if (isColliding)
            Raylib.DrawText("Enemy colliding!", 10, 80, 20, Color.RayWhite);

        // Draw enemies
        foreach (var enemy in enemies)
            enemy.Draw();

        player.Draw();

        // Win / lose conditions
        if (player.hp <= 20)
            Raylib.DrawText("GAME OVER", 400, 300, 40, Color.Red);
        else if (!enemiesCaptured.Contains(false))
            Raylib.DrawText("YOU WIN!", 400, 300, 40, Color.Green);
    }
}

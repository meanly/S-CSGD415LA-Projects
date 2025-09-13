using Raylib_cs;
using System.Numerics;
using System.Collections.Generic;

public class Game
{
    // border
    int borderThickness = 10;
    int screenWidth = 1280;
    int screenHeight = 720;

    public Player player;
    public bool takeDamage = false;

    public List<Enemy> enemies;          // list of enemies instead of variables
    public List<bool> enemiesCaptured;   // tracks if an enemy reached a goal box
    Rectangle playerRect;
    List<Rectangle> enemyRects;
    bool isColliding;

    // Winning condition
    List<Rectangle> goalBoxes;          // list of goal boxes
    List<bool> goalBoxesCaptured;       // tracks which goal box captured an enemy

    public Game()
    {
        // Initialize player at starting position
        player = new Player(100, 100);

        enemies = new List<Enemy>();
        enemiesCaptured = new List<bool>();
        enemyRects = new List<Rectangle>();

        int enemyCount = 7;
        int boxSize = 35;

        // keeps spawn range of enemies and goal boxes inside the border
        int spawnMinX = borderThickness;
        int spawnMaxX = screenWidth - borderThickness - boxSize;
        int spawnMinY = borderThickness;
        int spawnMaxY = screenHeight - borderThickness - boxSize;

        // spawn enemies randomly, but not too close to the player
        for (int i = 0; i < enemyCount; i++)
        {
            Vector2 pos;
            bool validPosition = false;
            do
            {
                pos = new Vector2(
                    Raylib.GetRandomValue(spawnMinX, spawnMaxX),
                    Raylib.GetRandomValue(spawnMinY, spawnMaxY)
                );
                validPosition = Raymath.Vector2Distance(pos, player.Position) >= 150; // buffer from player
            } while (!validPosition);

            enemies.Add(new Enemy(pos.X, pos.Y));
            enemiesCaptured.Add(false);
            enemyRects.Add(new Rectangle(pos.X, pos.Y, boxSize, boxSize));
        }

        // spawn goal boxes randomly inside borders
        goalBoxes = new List<Rectangle>
        {
            new Rectangle(Raylib.GetRandomValue(spawnMinX, spawnMaxX), Raylib.GetRandomValue(spawnMinY, spawnMaxY), boxSize, boxSize),
            new Rectangle(Raylib.GetRandomValue(spawnMinX, spawnMaxX), Raylib.GetRandomValue(spawnMinY, spawnMaxY), boxSize, boxSize),
            new Rectangle(Raylib.GetRandomValue(spawnMinX, spawnMaxX), Raylib.GetRandomValue(spawnMinY, spawnMaxY), boxSize, boxSize)
        };

        goalBoxesCaptured = new List<bool> { false, false, false };

        // player rectangle for collision detection
        playerRect = new Rectangle(player.Position.X, player.Position.Y, player.objectSize.width, player.objectSize.height);
    }

    public void Update()
    {
        // player movement
        player.Move();
        playerRect = new Rectangle(player.Position.X, player.Position.Y, player.objectSize.width, player.objectSize.height);

        // update enemies
        for (int i = 0; i < enemies.Count; i++)
        {
            if (!enemiesCaptured[i])
                enemies[i].Update(player.Position);

            enemyRects[i] = new Rectangle(enemies[i].Position.X, enemies[i].Position.Y, 35, 35);

            // check if enemy enters any goal box
            for (int j = 0; j < goalBoxes.Count; j++)
            {
                if (!enemiesCaptured[i] && Raylib.CheckCollisionRecs(enemyRects[i], goalBoxes[j]))
                {
                    enemiesCaptured[i] = true;
                    enemies[i].Position = new Vector2(-100, -100); // remove enemy from screen
                    goalBoxesCaptured[j] = true; // mark goal box green
                    break;
                }
            }
        }

        // check player collision with enemies
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
                    enemies[i].Position = new Vector2(1000, 1000); // temporarily remove enemy
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
        // hud
        Raylib.DrawText("Jeremy's Game | S-CSGD415LA LabAct1", 10, 10, 30, Color.RayWhite);
        Raylib.DrawText("Health: " + player.hp, 10, 50, 20, Color.RayWhite);

        // Draw goal boxes; turn green if captured
        for (int i = 0; i < goalBoxes.Count; i++)
        {
            Color boxColor = goalBoxesCaptured[i] ? Color.Green : Color.Yellow;
            Raylib.DrawRectangleRec(goalBoxes[i], boxColor);
        }

        // Draw collision text
        if (isColliding)
            Raylib.DrawText("Enemy colliding!", 10, 80, 20, Color.RayWhite);

        // Draw enemies
        foreach (var enemy in enemies)
            enemy.Draw();

        // Draw player
        player.Draw();

        // Draw red border (just visual, no collision)
        Raylib.DrawRectangleLinesEx(new Rectangle(0, 0, screenWidth, screenHeight), borderThickness, Color.Red);

        // Win / lose conditions
        if (player.hp <= 20)
            Raylib.DrawText("GAME OVER", 400, 300, 40, Color.Red);
        else if (!enemiesCaptured.Contains(false))
            Raylib.DrawText("YOU WIN!", 400, 300, 40, Color.Green);
    }
}

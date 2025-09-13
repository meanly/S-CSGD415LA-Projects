using Raylib_cs;

public class Game
{
    public Player player;
    public bool takeDamage =false;

    public Enemy enemy, enemy1, enemy2;
    Rectangle playerRect;
    Rectangle enemyRect, enemy1Rect, enemy2Rect;
    bool isColliding;

    // winning condition
    Rectangle goalBox;
    bool enemyCaptured, enemy1Captured, enemy2Captured;

    public Game()
    {
        player = new Player(100, 100);
        float speed = player.Speed;

        enemy = new Enemy(Raylib.GetRandomValue(0, 800), Raylib.GetRandomValue(0, 600));
        enemy1 = new Enemy(Raylib.GetRandomValue(0, 800), Raylib.GetRandomValue(0, 600));
        enemy2 = new Enemy(Raylib.GetRandomValue(0, 800), Raylib.GetRandomValue(0, 600));

        Rectangle playerRect = new Rectangle(player.Position.X, player.Position.Y, player.objectSize.width, player.objectSize.height);
        Rectangle enemyRect = new Rectangle(enemy.Position.X, enemy.Position.Y, 35, 35);
        enemy1Rect = new Rectangle(enemy1.Position.X, enemy1.Position.Y, 35, 35);
        enemy2Rect = new Rectangle(enemy2.Position.X, enemy2.Position.Y, 35, 35);

        // win box
        goalBox = new Rectangle(300, 200, 50, 50);

    }

    public void Update()
    {
        player.Move();

        // Only move enemies if not captured
        if (!enemyCaptured) enemy.Update(player.Position);
        if (!enemy1Captured) enemy1.Update(player.Position);
        if (!enemy2Captured) enemy2.Update(player.Position);

        // Check if enemies entered the goal box
        if (Raylib.CheckCollisionRecs(enemyRect, goalBox)) enemyCaptured = true;
        if (Raylib.CheckCollisionRecs(enemy1Rect, goalBox)) enemy1Captured = true;
        if (Raylib.CheckCollisionRecs(enemy2Rect, goalBox)) enemy2Captured = true;

        playerRect = new Rectangle(player.Position.X, player.Position.Y, player.objectSize.width, player.objectSize.height);
        enemyRect = new Rectangle(enemy.Position.X, enemy.Position.Y, 35, 35);
        enemy1Rect = new Rectangle(enemy1.Position.X, enemy1.Position.Y, 35, 35);
        enemy2Rect = new Rectangle(enemy2.Position.X, enemy2.Position.Y, 35, 35);

        //check if player collides with enemy
        // Check if enemies entered the goal box → capture them
        if (Raylib.CheckCollisionRecs(enemyRect, goalBox))  enemyCaptured  = true;
        if (Raylib.CheckCollisionRecs(enemy1Rect, goalBox)) enemy1Captured = true;
        if (Raylib.CheckCollisionRecs(enemy2Rect, goalBox)) enemy2Captured = true;

        isColliding = Raylib.CheckCollisionRecs(playerRect, enemyRect) ||
                  Raylib.CheckCollisionRecs(playerRect, enemy1Rect) ||
                  Raylib.CheckCollisionRecs(playerRect, enemy2Rect);

        if (isColliding && !takeDamage && player.hp > 20)
            {
            takeDamage = true;
            player.hp -= 20;

            if (Raylib.CheckCollisionRecs(playerRect, enemyRect))
                enemy.Position = new System.Numerics.Vector2(1000, 1000);
             else if (Raylib.CheckCollisionRecs(playerRect, enemy1Rect))
                enemy1.Position = new System.Numerics.Vector2(1000, 1000);
            else if (Raylib.CheckCollisionRecs(playerRect, enemy2Rect))
                enemy2.Position = new System.Numerics.Vector2(1000, 1000);
            }
            else if (!isColliding)
            {
                takeDamage = false;
            } 
    }

    public void Draw()
    {
        Raylib.DrawText("Jeremy's Game", 10, 10, 30, Color.RayWhite);
        Raylib.DrawText("Health: " + player.hp, 10, 50, 20, Color.RayWhite);

        // Draw yellow goal box
        Raylib.DrawRectangleRec(goalBox, Color.Yellow);

        if (isColliding)
        {

            Raylib.DrawText("Enemy colliding!", 10, 80, 20, Color.RayWhite);
        }
        else
        {

            //Raylib.DrawText("Jeremy's Game", 10, 10, 20, Color.RayWhite);
        }

        player.Draw();
        enemy.Draw();
        enemy1.Draw();
        enemy2.Draw();

                        // Win / lose conditions
        if (player.hp <= 20)
        {
            Raylib.DrawText("GAME OVER", 400, 300, 40, Color.Red);
        }
        else if (enemyCaptured && enemy1Captured && enemy2Captured)
        {
            Raylib.DrawText("YOU WIN!", 400, 300, 40, Color.Green);
        }

    }
}

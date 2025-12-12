using Finals_SpaceFlap.Systems;
using Raylib_cs;

const int screenWidth = 800;
const int screenHeight = 600;

Raylib.InitWindow(screenWidth, screenHeight, "Space Flap");
Raylib.SetTargetFPS(60);

GameManager gameManager = new GameManager(screenWidth, screenHeight);

while (!Raylib.WindowShouldClose())
{
    float deltaTime = Raylib.GetFrameTime();

    gameManager.Update(deltaTime);

    Raylib.BeginDrawing();
    gameManager.Render();
    Raylib.EndDrawing();
}

Raylib.CloseWindow();

using Raylib_cs;
using System.Numerics;

namespace Antiquera_LabActivity3
{
    // Main game class
    public class BlockBlastGame
    {
        private GameManager gameManager;
        private const int WINDOW_WIDTH = 1200;
        private const int WINDOW_HEIGHT = 800;
        private const int TILE_SIZE = 60;
        private const int BOARD_OFFSET_X = 50;
        private const int BOARD_OFFSET_Y = 50;
        private const int PATTERN_OFFSET_X = 600;
        private const int PATTERN_OFFSET_Y = 50;

        private Vector2 selectedPatternPos;
        private int selectedPatternIndex = -1;
        private bool isDragging = false;
        private List<TilePattern> currentPatterns = null!;
        private List<bool> patternUsed = null!;
        private Vector2 previewPosition;

        public BlockBlastGame()
        {
            gameManager = new GameManager();
            InitializePatterns();
        }

        private void InitializePatterns()
        {
            currentPatterns = gameManager.PatternManager.GetRandomPatterns(3);
            patternUsed = new List<bool> { false, false, false };
        }

        public void Run()
        {
            Raylib.InitWindow(WINDOW_WIDTH, WINDOW_HEIGHT, "Block Blast Clone");
            Raylib.SetTargetFPS(60);

            while (!Raylib.WindowShouldClose())
            {
                Update();
                Draw();
            }

            Raylib.CloseWindow();
        }

        private void Update()
        {
            gameManager.Update();
            HandleInput();
        }

        private void HandleInput()
        {
            if (gameManager.IsGameOver)
            {
                if (Raylib.IsKeyPressed(KeyboardKey.R))
                {
                    gameManager.NewGame();
                    InitializePatterns();
                }
                return;
            }

            if (Raylib.IsKeyPressed(KeyboardKey.P))
            {
                gameManager.Pause();
            }

            if (Raylib.IsKeyPressed(KeyboardKey.S))
            {
                gameManager.SaveGame();
            }

            if (Raylib.IsKeyPressed(KeyboardKey.L))
            {
                gameManager.LoadGame();
            }

            HandleMouseInput();
        }

        private void HandleMouseInput()
        {
            Vector2 mousePos = Raylib.GetMousePosition();

            if (Raylib.IsMouseButtonPressed(MouseButton.Left))
            {
                // Check if clicking on a pattern (only if not used)
                for (int i = 0; i < currentPatterns.Count; i++)
                {
                    if (!patternUsed[i]) // Only allow selection of unused patterns
                    {
                        Rectangle patternRect = new Rectangle(
                            PATTERN_OFFSET_X,
                            PATTERN_OFFSET_Y + i * 150,
                            200,
                            120
                        );

                        if (Raylib.CheckCollisionPointRec(mousePos, patternRect))
                        {
                            selectedPatternIndex = i;
                            isDragging = true;
                            break;
                        }
                    }
                }
            }

            // Update preview position while dragging
            if (isDragging && selectedPatternIndex >= 0)
            {
                // Convert mouse position to board coordinates
                int boardX = (int)((mousePos.X - BOARD_OFFSET_X) / TILE_SIZE);
                int boardY = (int)((mousePos.Y - BOARD_OFFSET_Y) / TILE_SIZE);

                // Clamp to valid board positions
                boardX = Math.Max(0, Math.Min(boardX, TileBoard.BOARD_SIZE - 1));
                boardY = Math.Max(0, Math.Min(boardY, TileBoard.BOARD_SIZE - 1));

                previewPosition = new Vector2(boardX, boardY);
            }

            if (Raylib.IsMouseButtonReleased(MouseButton.Left) && isDragging)
            {
                isDragging = false;

                if (selectedPatternIndex >= 0)
                {
                    // Convert mouse position to board coordinates
                    int boardX = (int)((mousePos.X - BOARD_OFFSET_X) / TILE_SIZE);
                    int boardY = (int)((mousePos.Y - BOARD_OFFSET_Y) / TILE_SIZE);

                    if (boardX >= 0 && boardX < TileBoard.BOARD_SIZE &&
                        boardY >= 0 && boardY < TileBoard.BOARD_SIZE)
                    {
                        if (selectedPatternIndex < currentPatterns.Count && !patternUsed[selectedPatternIndex])
                        {
                            bool patternPlaced = gameManager.TryPlacePattern(currentPatterns[selectedPatternIndex], boardX, boardY);
                            if (patternPlaced)
                            {
                                // Mark this pattern as used
                                patternUsed[selectedPatternIndex] = true;

                                // Check if all patterns are used
                                if (patternUsed.All(used => used))
                                {
                                    // All patterns used, generate new ones
                                    InitializePatterns();
                                }

                                // Check if game is over (no valid placements for any unused pattern)
                                if (gameManager.CheckGameOver(currentPatterns.Where((pattern, index) => !patternUsed[index]).ToList()))
                                {
                                    gameManager.EndGame();
                                }
                            }
                        }
                    }
                }

                selectedPatternIndex = -1;
            }
        }

        private void Draw()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.White);

            DrawBoard();
            DrawPatterns();
            DrawPreview();
            DrawUI();

            Raylib.EndDrawing();
        }

        private void DrawBoard()
        {
            // Draw board background
            Raylib.DrawRectangle(BOARD_OFFSET_X - 5, BOARD_OFFSET_Y - 5,
                TileBoard.BOARD_SIZE * TILE_SIZE + 10, TileBoard.BOARD_SIZE * TILE_SIZE + 10, Color.Black);

            // Draw tiles with grid
            for (int x = 0; x < TileBoard.BOARD_SIZE; x++)
            {
                for (int y = 0; y < TileBoard.BOARD_SIZE; y++)
                {
                    int posX = BOARD_OFFSET_X + x * TILE_SIZE;
                    int posY = BOARD_OFFSET_Y + y * TILE_SIZE;

                    // Draw tile background (light gray for empty, colored for filled)
                    Color tileColor = gameManager.TileBoard.IsTileEmpty(x, y) ? Color.LightGray : GetTileColor(gameManager.TileBoard.GetTileColor(x, y));
                    Raylib.DrawRectangle(posX, posY, TILE_SIZE - 1, TILE_SIZE - 1, tileColor);

                    // Draw grid lines
                    Raylib.DrawRectangleLines(posX, posY, TILE_SIZE - 1, TILE_SIZE - 1, Color.DarkGray);
                }
            }
        }

        private void DrawPatterns()
        {
            for (int i = 0; i < currentPatterns.Count; i++)
            {
                int patternY = PATTERN_OFFSET_Y + i * 150;

                // Draw pattern background - different colors for used/unused patterns
                Color bgColor;
                if (patternUsed[i])
                {
                    bgColor = Color.DarkGray; // Used pattern
                }
                else if (selectedPatternIndex == i)
                {
                    bgColor = Color.LightGray; // Selected pattern
                }
                else
                {
                    bgColor = Color.Gray; // Available pattern
                }

                Raylib.DrawRectangle(PATTERN_OFFSET_X, patternY, 200, 120, bgColor);

                // Draw pattern tiles (only if not used)
                if (!patternUsed[i])
                {
                    Color patternColor = GetTileColor(currentPatterns[i].Color);
                    foreach (var pos in currentPatterns[i].Positions)
                    {
                        int tileX = PATTERN_OFFSET_X + 20 + (int)pos.X * 30;
                        int tileY = patternY + 20 + (int)pos.Y * 30;
                        Raylib.DrawRectangle(tileX, tileY, 28, 28, patternColor);
                    }
                }
                else
                {
                    // Draw "USED" text for used patterns
                    Raylib.DrawText("USED", PATTERN_OFFSET_X + 60, patternY + 50, 20, Color.White);
                }
            }
        }

        private void DrawPreview()
        {
            // Only draw preview if dragging and pattern is selected
            if (isDragging && selectedPatternIndex >= 0 && !patternUsed[selectedPatternIndex])
            {
                var pattern = currentPatterns[selectedPatternIndex];
                int previewX = (int)previewPosition.X;
                int previewY = (int)previewPosition.Y;

                // Check if the pattern can be placed at this position
                bool canPlace = CanPlacePatternAt(pattern, previewX, previewY);

                // Draw preview tiles
                foreach (var pos in pattern.Positions)
                {
                    int x = previewX + (int)pos.X;
                    int y = previewY + (int)pos.Y;

                    // Only draw if within board bounds
                    if (x >= 0 && x < TileBoard.BOARD_SIZE && y >= 0 && y < TileBoard.BOARD_SIZE)
                    {
                        int screenX = BOARD_OFFSET_X + x * TILE_SIZE;
                        int screenY = BOARD_OFFSET_Y + y * TILE_SIZE;

                        // Use different colors based on whether placement is valid
                        Color previewColor = canPlace ? Color.White : Color.Red;
                        Color borderColor = canPlace ? Color.Green : Color.Red;

                        // Draw semi-transparent preview tile
                        Raylib.DrawRectangle(screenX, screenY, TILE_SIZE - 1, TILE_SIZE - 1, previewColor);
                        Raylib.DrawRectangleLines(screenX, screenY, TILE_SIZE - 1, TILE_SIZE - 1, borderColor);

                        // Draw a small version of the pattern color in the center
                        Color patternColor = GetTileColor(pattern.Color);
                        int centerX = screenX + (TILE_SIZE - 20) / 2;
                        int centerY = screenY + (TILE_SIZE - 20) / 2;
                        Raylib.DrawRectangle(centerX, centerY, 20, 20, patternColor);
                    }
                }
            }
        }

        private bool CanPlacePatternAt(TilePattern pattern, int boardX, int boardY)
        {
            foreach (var pos in pattern.Positions)
            {
                int x = boardX + (int)pos.X;
                int y = boardY + (int)pos.Y;

                if (!gameManager.TileBoard.IsValidPosition(x, y) || !gameManager.TileBoard.IsTileEmpty(x, y))
                {
                    return false;
                }
            }
            return true;
        }

        private void DrawUI()
        {
            // Draw score
            Raylib.DrawText($"Score: {gameManager.ScoreManager.GetScore()}", 10, 10, 20, Color.Black);
            Raylib.DrawText($"Combo: {gameManager.ScoreManager.Combo}", 10, 35, 20, Color.Black);

            // Draw instructions
            Raylib.DrawText("Controls:", 10, WINDOW_HEIGHT - 120, 16, Color.Black);
            Raylib.DrawText("P - Pause", 10, WINDOW_HEIGHT - 100, 14, Color.Black);
            Raylib.DrawText("S - Save Game", 10, WINDOW_HEIGHT - 80, 14, Color.Black);
            Raylib.DrawText("L - Load Game", 10, WINDOW_HEIGHT - 60, 14, Color.Black);
            Raylib.DrawText("Click and drag patterns to place", 10, WINDOW_HEIGHT - 40, 14, Color.Black);

            if (gameManager.IsPaused)
            {
                Raylib.DrawText("PAUSED", WINDOW_WIDTH / 2 - 50, WINDOW_HEIGHT / 2, 30, Color.Red);
            }

            if (gameManager.IsGameOver)
            {
                Raylib.DrawText("GAME OVER", WINDOW_WIDTH / 2 - 80, WINDOW_HEIGHT / 2 - 30, 30, Color.Red);
                Raylib.DrawText("Press R to restart", WINDOW_WIDTH / 2 - 100, WINDOW_HEIGHT / 2 + 10, 20, Color.Black);
            }
        }

        private Color GetTileColor(TileColor tileColor)
        {
            return tileColor switch
            {
                TileColor.Black => Color.Black,
                TileColor.Yellow => Color.Yellow,
                TileColor.Blue => Color.Blue,
                TileColor.Red => Color.Red,
                TileColor.LimeGreen => Color.Lime,
                TileColor.Violet => Color.Violet,
                TileColor.Orange => Color.Orange,
                TileColor.Cyan => Color.SkyBlue,
                TileColor.Pink => Color.Pink,
                _ => Color.White
            };
        }
    }
}

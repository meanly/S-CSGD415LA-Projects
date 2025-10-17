using Raylib_cs;
using System;

namespace MemoryGame
{
    public class Game
    {
        public enum GameState { Ongoing, Paused, GameOver, Win }
        
        public GameState State = GameState.Ongoing;
        public int Health = 3;
        public int Score = 0;
        public float Timer = 30.0f;
        
        private Tile[,] tiles = new Tile[4, 7];
        private Tile firstTile = null;
        private Tile secondTile = null;
        private float revealTimer = 0;
        private bool showingAll = false;
        
        public Game()
        {
            NewGame();
        }
        
        public void NewGame()
        {
            State = GameState.Ongoing;
            Health = 3;
            Score = 0;
            Timer = 30.0f;
            firstTile = null;
            secondTile = null;
            revealTimer = 0;
            showingAll = false;
            
            // Create pairs of tiles
            int[] values = new int[28];
            for (int i = 0; i < 14; i++)
            {
                values[i * 2] = i;
                values[i * 2 + 1] = i;
            }
            
            // Shuffle
            Random rand = new Random();
            for (int i = 0; i < 28; i++)
            {
                int j = rand.Next(i, 28);
                int temp = values[i];
                values[i] = values[j];
                values[j] = temp;
            }
            
            // Create tiles
            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 7; y++)
                {
                    tiles[x, y] = new Tile(x, y, values[x * 7 + y]);
                }
            }
        }
        
        public void Update()
        {
            if (State == GameState.Ongoing)
            {
                // Show all tiles for 3 seconds at start
                if (!showingAll)
                {
                    revealTimer += Raylib.GetFrameTime();
                    if (revealTimer >= 3.0f)
                    {
                        showingAll = true;
                        HideAllTiles();
                    }
                }
                else
                {
                    Timer -= Raylib.GetFrameTime();
                    
                    // Handle input
                    if (Raylib.IsMouseButtonPressed(MouseButton.Left))
                    {
                        Vector2 mousePos = Raylib.GetMousePosition();
                        Tile clickedTile = GetTileAtPosition(mousePos.X, mousePos.Y);
                        
                        if (clickedTile != null && !clickedTile.IsFlipped && !clickedTile.IsMatched)
                        {
                            clickedTile.Flip();
                            
                            if (firstTile == null)
                            {
                                firstTile = clickedTile;
                            }
                            else if (secondTile == null)
                            {
                                secondTile = clickedTile;
                                
                                // Check for match
                                if (firstTile.Value == secondTile.Value)
                                {
                                    // Match!
                                    firstTile.Match();
                                    secondTile.Match();
                                    Score += 10;
                                    Timer = 30.0f;
                                    firstTile = null;
                                    secondTile = null;
                                    
                                    // Check win
                                    if (AllTilesMatched())
                                    {
                                        State = GameState.Win;
                                    }
                                }
                                else
                                {
                                    // No match - unflip after delay
                                    Timer = 30.0f;
                                    Health--;
                                    if (Health <= 0)
                                    {
                                        State = GameState.GameOver;
                                    }
                                }
                            }
                        }
                    }
                    
                    // Reset tiles after delay
                    if (firstTile != null && secondTile != null && firstTile.Value != secondTile.Value)
                    {
                        if (revealTimer >= 1.0f)
                        {
                            firstTile.Unflip();
                            secondTile.Unflip();
                            firstTile = null;
                            secondTile = null;
                            revealTimer = 0;
                        }
                        else
                        {
                            revealTimer += Raylib.GetFrameTime();
                        }
                    }
                    else
                    {
                        revealTimer = 0;
                    }
                    
                    // Timer ran out
                    if (Timer <= 0)
                    {
                        Health--;
                        Timer = 30.0f;
                        if (Health <= 0)
                        {
                            State = GameState.GameOver;
                        }
                    }
                }
            }
            else if (State == GameState.GameOver && Raylib.IsKeyPressed(KeyboardKey.R))
            {
                NewGame();
            }
            else if (State == GameState.Win && Raylib.IsKeyPressed(KeyboardKey.R))
            {
                NewGame();
            }
        }
        
        public void Draw()
        {
            // Draw background
            Raylib.DrawRectangle(0, 0, 800, 600, Color.LightGray);
            
            if (State == GameState.Ongoing)
            {
                // Draw tiles
                for (int x = 0; x < 4; x++)
                {
                    for (int y = 0; y < 7; y++)
                    {
                        tiles[x, y].Draw();
                    }
                }
                
                // Draw UI
                Raylib.DrawText($"Health: {Health}", 10, 10, 20, Color.Black);
                Raylib.DrawText($"Score: {Score}", 10, 40, 20, Color.Black);
                Raylib.DrawText($"Timer: {(int)Timer}", 10, 70, 20, Color.Black);
            }
            else if (State == GameState.GameOver)
            {
                Raylib.DrawText("GAME OVER", 300, 250, 40, Color.Red);
                Raylib.DrawText("Press R to restart", 280, 300, 20, Color.Black);
            }
            else if (State == GameState.Win)
            {
                Raylib.DrawText("YOU WIN!", 300, 250, 40, Color.Green);
                Raylib.DrawText("Press R to restart", 280, 300, 20, Color.Black);
            }
        }
        
        private void HideAllTiles()
        {
            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 7; y++)
                {
                    tiles[x, y].Unflip();
                }
            }
        }
        
        private Tile GetTileAtPosition(float x, float y)
        {
            for (int tx = 0; tx < 4; tx++)
            {
                for (int ty = 0; ty < 7; ty++)
                {
                    if (tiles[tx, ty].IsPointInside(x, y))
                    {
                        return tiles[tx, ty];
                    }
                }
            }
            return null;
        }
        
        private bool AllTilesMatched()
        {
            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 7; y++)
                {
                    if (!tiles[x, y].IsMatched)
                        return false;
                }
            }
            return true;
        }
    }
}

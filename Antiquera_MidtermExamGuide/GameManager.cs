using Raylib_cs;
using System;

namespace MemoryGame
{
    public enum GameState { Ongoing = 1, Paused = 2, GameOver = 3, Win = 4 }

    public class GameManager
    {
        public GameState State { get; set; } = GameState.Ongoing;
        public TileManager TileManager { get; private set; } = new TileManager();
        public SoundManager? SoundManager { get; set; }
        public int Health { get; set; } = 3;
        public int Score { get; set; } = 0;
        public float Timer { get; set; } = 30.0f;
        public float ShowTilesTimer { get; set; }
        public bool GameStarted { get; set; }
        public bool ShouldUnflip { get; set; }
        public float UnflipDelay { get; set; }

        public void NewGame()
        {
            State = GameState.Ongoing;
            Health = 3;
            Score = 0;
            Timer = 30.0f;
            ShowTilesTimer = 0;
            GameStarted = false;
            ShouldUnflip = false;
            UnflipDelay = 0;
            TileManager.NewGame();
        }

        public void Update()
        {
            if (State == GameState.Ongoing) UpdateOngoing();
            else if (State == GameState.Paused && Raylib.IsKeyPressed(KeyboardKey.Space)) State = GameState.Ongoing;
            else if (State == GameState.GameOver && Raylib.IsKeyPressed(KeyboardKey.R)) NewGame();
            else if (State == GameState.Win && Raylib.IsKeyPressed(KeyboardKey.R)) NewGame();
        }

        private void UpdateOngoing()
        {
            // Skip the initial reveal - start game immediately
            GameStarted = true;

            Timer -= Raylib.GetFrameTime();
            if (ShouldUnflip) { UnflipDelay += Raylib.GetFrameTime(); if (UnflipDelay >= 1.0f) { TileManager.UnflipAllTiles(); ShouldUnflip = false; UnflipDelay = 0; } }
            if (Timer <= 0) { Health--; TileManager.UnflipAllTiles(); Timer = 30.0f; if (Health <= 0) State = GameState.GameOver; }
            if (Raylib.IsKeyPressed(KeyboardKey.P)) State = GameState.Paused;
            if (Raylib.IsMouseButtonPressed(MouseButton.Left))
            {
                var mousePos = Raylib.GetMousePosition();
                var tile = TileManager.GetTileAtPosition(mousePos.X, mousePos.Y);
                if (tile != null && !tile.IsFlipped)
                {
                    TileManager.FlipTile(tile);
                    if (TileManager.HasTwoFlipped())
                    {
                        if (TileManager.CompareFlippedTiles()) { Score += 10; Timer = 30.0f; }
                        else { Health--; ShouldUnflip = true; Timer = 30.0f; if (Health <= 0) State = GameState.GameOver; }
                    }
                }
            }

            // Only check for victory after game has started and tiles are actually matched
            if (TileManager.AllTilesMatched()) State = GameState.Win;
        }

        public float GetHealthPercentage() => (float)Health / 3.0f;
        public float GetTimerPercentage() => Timer / 30.0f;
    }
}
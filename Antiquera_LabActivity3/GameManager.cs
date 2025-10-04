using Raylib_cs;
using System.Numerics;

namespace Antiquera_LabActivity3
{
    // GameManager class for overall game management
    public class GameManager
    {
        public bool IsPaused { get; private set; }
        public bool IsGameOver { get; private set; }
        public TileBoard TileBoard { get; private set; } = null!;
        public TilePatternManager PatternManager { get; private set; } = null!;
        public TileChecker TileChecker { get; private set; } = null!;
        public TilePurger TilePurger { get; private set; } = null!;
        public ScoreManager ScoreManager { get; private set; } = null!;
        public SoundManager SoundManager { get; private set; } = null!;

        private List<TilePattern> currentPatterns = null!;
        private const string SAVE_FILE = "savegame.dat";
        private const string SCORES_FILE = "scores.dat";

        public GameManager()
        {
            InitializeGame();
        }

        private void InitializeGame()
        {
            TileBoard = new TileBoard();
            PatternManager = new TilePatternManager();
            ScoreManager = new ScoreManager();
            SoundManager = new SoundManager();
            TileChecker = new TileChecker(TileBoard);
            TilePurger = new TilePurger(TileBoard, ScoreManager);

            IsPaused = false;
            IsGameOver = false;

            // Get initial patterns
            currentPatterns = PatternManager.GetRandomPatterns(3);
        }

        public void Pause()
        {
            IsPaused = !IsPaused;
        }

        public void EndGame()
        {
            IsGameOver = true;
            SaveScore();
        }

        public void NewGame()
        {
            InitializeGame();
        }

        public void SaveGame()
        {
            // Simple save implementation
            using (var writer = new BinaryWriter(File.OpenWrite(SAVE_FILE)))
            {
                writer.Write(ScoreManager.GetScore());
                writer.Write(ScoreManager.Combo);

                // Save tile board
                for (int x = 0; x < TileBoard.BOARD_SIZE; x++)
                {
                    for (int y = 0; y < TileBoard.BOARD_SIZE; y++)
                    {
                        writer.Write((int)TileBoard.GetTileColor(x, y));
                    }
                }
            }
        }

        public void LoadGame()
        {
            if (!File.Exists(SAVE_FILE)) return;

            try
            {
                using (var reader = new BinaryReader(File.OpenRead(SAVE_FILE)))
                {
                    ScoreManager = new ScoreManager();
                    int score = reader.ReadInt32();
                    int combo = reader.ReadInt32();

                    // Load tile board
                    for (int x = 0; x < TileBoard.BOARD_SIZE; x++)
                    {
                        for (int y = 0; y < TileBoard.BOARD_SIZE; y++)
                        {
                            TileColor color = (TileColor)reader.ReadInt32();
                            TileBoard.SetTile(x, y, color);
                        }
                    }
                }
            }
            catch
            {
                // If loading fails, start new game
                NewGame();
            }
        }

        public void SaveScore()
        {
            var scores = LoadScores();
            scores.Add(ScoreManager.GetScore());
            scores = scores.OrderByDescending(s => s).Take(10).ToList();

            using (var writer = new BinaryWriter(File.OpenWrite(SCORES_FILE)))
            {
                writer.Write(scores.Count);
                foreach (var score in scores)
                {
                    writer.Write(score);
                }
            }
        }

        public List<int> LoadScores()
        {
            var scores = new List<int>();

            if (!File.Exists(SCORES_FILE)) return scores;

            try
            {
                using (var reader = new BinaryReader(File.OpenRead(SCORES_FILE)))
                {
                    int count = reader.ReadInt32();
                    for (int i = 0; i < count; i++)
                    {
                        scores.Add(reader.ReadInt32());
                    }
                }
            }
            catch
            {
                // Return empty list if loading fails
            }

            return scores;
        }

        public bool TryPlacePattern(TilePattern pattern, int boardX, int boardY)
        {
            // Check if pattern can be placed
            foreach (var pos in pattern.Positions)
            {
                int x = (int)(boardX + pos.X);
                int y = (int)(boardY + pos.Y);

                if (!TileBoard.IsValidPosition(x, y) || !TileBoard.IsTileEmpty(x, y))
                {
                    return false;
                }
            }

            // Place the pattern
            foreach (var pos in pattern.Positions)
            {
                int x = (int)(boardX + pos.X);
                int y = (int)(boardY + pos.Y);
                TileBoard.SetTile(x, y, pattern.Color);
            }

            // Check for completed rows/columns
            var completedTiles = TileChecker.CheckCompletedTiles();
            if (completedTiles.Count > 0)
            {
                TilePurger.PurgeTiles(completedTiles);
            }

            return true;
        }

        private bool CanPlaceAnyPattern()
        {
            foreach (var pattern in currentPatterns)
            {
                for (int x = 0; x < TileBoard.BOARD_SIZE; x++)
                {
                    for (int y = 0; y < TileBoard.BOARD_SIZE; y++)
                    {
                        if (CanPlacePatternAt(pattern, x, y))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool CanPlacePatternAt(TilePattern pattern, int boardX, int boardY)
        {
            foreach (var pos in pattern.Positions)
            {
                int x = (int)(boardX + pos.X);
                int y = (int)(boardY + pos.Y);

                if (!TileBoard.IsValidPosition(x, y) || !TileBoard.IsTileEmpty(x, y))
                {
                    return false;
                }
            }
            return true;
        }

        public void Update()
        {
            if (!IsPaused && !IsGameOver)
            {
                ScoreManager.Update();
            }
        }

        public bool CheckGameOver(List<TilePattern> currentPatterns)
        {
            // Check if game is over (no valid placements)
            foreach (var pattern in currentPatterns)
            {
                for (int x = 0; x < TileBoard.BOARD_SIZE; x++)
                {
                    for (int y = 0; y < TileBoard.BOARD_SIZE; y++)
                    {
                        if (CanPlacePatternAt(pattern, x, y))
                        {
                            return false; // Found a valid placement
                        }
                    }
                }
            }
            return true; // No valid placements found
        }
    }
}

using Raylib_cs;
using System.Numerics;

namespace Antiquera_LabActivity3
{
    // TileBoard class managing the 8x8 grid
    public class TileBoard
    {
        public const int BOARD_SIZE = 8;
        private Tile[,] tiles;

        public TileBoard()
        {
            tiles = new Tile[BOARD_SIZE, BOARD_SIZE];
            InitializeBoard();
        }

        private void InitializeBoard()
        {
            for (int x = 0; x < BOARD_SIZE; x++)
            {
                for (int y = 0; y < BOARD_SIZE; y++)
                {
                    tiles[x, y] = new Tile(TileColor.Black);
                }
            }
        }

        public Tile? GetTile(int x, int y)
        {
            if (IsValidPosition(x, y))
                return tiles[x, y];
            return null;
        }

        public void SetTile(int x, int y, TileColor color)
        {
            if (IsValidPosition(x, y))
                tiles[x, y].Color = color;
        }

        public bool IsValidPosition(int x, int y)
        {
            return x >= 0 && x < BOARD_SIZE && y >= 0 && y < BOARD_SIZE;
        }

        public bool IsTileEmpty(int x, int y)
        {
            if (!IsValidPosition(x, y)) return false;
            return tiles[x, y].IsEmpty;
        }

        public TileColor GetTileColor(int x, int y)
        {
            if (!IsValidPosition(x, y)) return TileColor.Black;
            return tiles[x, y].Color;
        }

        public void ClearTile(int x, int y)
        {
            if (IsValidPosition(x, y))
                tiles[x, y].Color = TileColor.Black;
        }
    }

    // TileChecker class for checking completed rows/columns
    public class TileChecker
    {
        private TileBoard tileBoard;

        public TileChecker(TileBoard board)
        {
            tileBoard = board;
        }

        public List<Vector2> CheckRows()
        {
            var completedTiles = new List<Vector2>();

            for (int y = 0; y < TileBoard.BOARD_SIZE; y++)
            {
                bool isRowComplete = true;
                var rowTiles = new List<Vector2>();

                for (int x = 0; x < TileBoard.BOARD_SIZE; x++)
                {
                    if (tileBoard.IsTileEmpty(x, y))
                    {
                        isRowComplete = false;
                        break;
                    }
                    rowTiles.Add(new Vector2(x, y));
                }

                if (isRowComplete)
                {
                    completedTiles.AddRange(rowTiles);
                }
            }

            return completedTiles;
        }

        public List<Vector2> CheckColumns()
        {
            var completedTiles = new List<Vector2>();

            for (int x = 0; x < TileBoard.BOARD_SIZE; x++)
            {
                bool isColumnComplete = true;
                var columnTiles = new List<Vector2>();

                for (int y = 0; y < TileBoard.BOARD_SIZE; y++)
                {
                    if (tileBoard.IsTileEmpty(x, y))
                    {
                        isColumnComplete = false;
                        break;
                    }
                    columnTiles.Add(new Vector2(x, y));
                }

                if (isColumnComplete)
                {
                    completedTiles.AddRange(columnTiles);
                }
            }

            return completedTiles;
        }

        public List<Vector2> CheckCompletedTiles()
        {
            var completedTiles = new List<Vector2>();
            completedTiles.AddRange(CheckRows());
            completedTiles.AddRange(CheckColumns());

            // Remove duplicates
            return completedTiles.Distinct().ToList();
        }
    }

    // TilePurger class for clearing completed tiles
    public class TilePurger
    {
        private TileBoard tileBoard;
        private ScoreManager scoreManager;

        public TilePurger(TileBoard board, ScoreManager scoreManager)
        {
            this.tileBoard = board;
            this.scoreManager = scoreManager;
        }

        public void PurgeTiles(List<Vector2> completedTiles)
        {
            if (completedTiles.Count == 0) return;

            // Clear the tiles
            foreach (var tile in completedTiles)
            {
                tileBoard.ClearTile((int)tile.X, (int)tile.Y);
            }

            // Calculate score
            int baseScore = completedTiles.Count * 10;
            scoreManager.AddScore(baseScore);

            // Check for combo (more than 8 tiles cleared)
            if (completedTiles.Count > 8)
            {
                int comboMultiplier = completedTiles.Count / 8;
                scoreManager.SetCombo(comboMultiplier);
            }
        }
    }
}

using Raylib_cs;
using System.Numerics;
using System.Collections;

namespace Antiquera_LabActivity3
{
    // TileBoard class managing the 8x8 grid using tileset 2D array
    public class TileBoard
    {
        public const int BOARD_SIZE = 8;
        private int[,] tileset; // 2D array containing tile values

        public TileBoard()
        {
            tileset = new int[BOARD_SIZE, BOARD_SIZE];
            InitializeBoard();
        }

        private void InitializeBoard()
        {
            // At the start of the game, all tileset elements are set to 0 (blank tiles)
            for (int x = 0; x < BOARD_SIZE; x++)
            {
                for (int y = 0; y < BOARD_SIZE; y++)
                {
                    tileset[x, y] = 0; // 0 = blank/empty tile
                }
            }
        }

        public int TileCheck(int x, int y)
        {
            // Returns the value of a tile in the TileBoard
            if (IsValidPosition(x, y))
                return tileset[x, y];
            return 0;
        }

        public void SetTile(int x, int y, int value)
        {
            if (IsValidPosition(x, y))
                tileset[x, y] = value;
        }

        public bool IsValidPosition(int x, int y)
        {
            return x >= 0 && x < BOARD_SIZE && y >= 0 && y < BOARD_SIZE;
        }

        public bool IsTileEmpty(int x, int y)
        {
            // Returns true if the tile in the tileset's value is zero, otherwise returns false
            if (!IsValidPosition(x, y)) return false;
            return tileset[x, y] == 0;
        }

        public void ClearTile(int x, int y)
        {
            // Sets the tile values back to zero
            if (IsValidPosition(x, y))
                tileset[x, y] = 0;
        }

        // Helper method to get tile color for rendering
        public TileColor GetTileColor(int x, int y)
        {
            if (!IsValidPosition(x, y)) return TileColor.Black;
            return (TileColor)tileset[x, y];
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

        public int CheckRows(ArrayList completedTiles)
        {
            // Checks for a completed row, if there is, return the n number of completed rows, 
            // then adds all the completed tile coordinates into an arraylist
            int completedRowCount = 0;

            for (int y = 0; y < TileBoard.BOARD_SIZE; y++)
            {
                bool isRowComplete = true;

                for (int x = 0; x < TileBoard.BOARD_SIZE; x++)
                {
                    if (tileBoard.IsTileEmpty(x, y))
                    {
                        isRowComplete = false;
                        break;
                    }
                }

                if (isRowComplete)
                {
                    completedRowCount++;
                    // Add all tiles in this row to the completedTiles arraylist
                    for (int x = 0; x < TileBoard.BOARD_SIZE; x++)
                    {
                        completedTiles.Add(new Vector2(x, y));
                    }
                }
            }

            return completedRowCount;
        }

        public int CheckColumns(ArrayList completedTiles)
        {
            // Checks for a completed column, if there is, return the n number of completed columns, 
            // then adds all the completed tile coordinates into an arraylist
            int completedColumnCount = 0;

            for (int x = 0; x < TileBoard.BOARD_SIZE; x++)
            {
                bool isColumnComplete = true;

                for (int y = 0; y < TileBoard.BOARD_SIZE; y++)
                {
                    if (tileBoard.IsTileEmpty(x, y))
                    {
                        isColumnComplete = false;
                        break;
                    }
                }

                if (isColumnComplete)
                {
                    completedColumnCount++;
                    // Add all tiles in this column to the completedTiles arraylist
                    for (int y = 0; y < TileBoard.BOARD_SIZE; y++)
                    {
                        completedTiles.Add(new Vector2(x, y));
                    }
                }
            }

            return completedColumnCount;
        }

        public ArrayList CheckCompletedTiles()
        {
            var completedTiles = new ArrayList();
            CheckRows(completedTiles);
            CheckColumns(completedTiles);
            return completedTiles;
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

        public void PurgeTiles(ArrayList completedTiles)
        {
            // Purges all the tiles with the coordinates in the completedTiles arraylist
            if (completedTiles.Count == 0) return;

            // Clear the tiles (sets tile values back to zero)
            foreach (Vector2 tile in completedTiles)
            {
                tileBoard.ClearTile((int)tile.X, (int)tile.Y);
            }

            // Add a score based on the number of purged tiles
            int baseScore = completedTiles.Count * 10;
            scoreManager.AddScore(baseScore);

            // If the arraylist count is greater than 8, divide the values by 8 and add that to the combo
            if (completedTiles.Count > 8)
            {
                int comboMultiplier = completedTiles.Count / 8;
                scoreManager.SetCombo(comboMultiplier);
            }

            // TODO: May possibly show an animation and play a sound
        }
    }
}

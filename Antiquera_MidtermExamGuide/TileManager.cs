using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Raylib_cs;

namespace MemoryGame
{
    public class TileManager
    {
        private List<Tile> tiles = new();
        private List<Tile> revealedThisTurn = new();
        private Random rng = new();
        private int lastHoverIndex = -1;
        private float mismatchRevealTime = 0.8f;
        private float mismatchTimer = 0f;
        private int cols;
        private int rows;

        public TileManager(int columns, int rows)
        {
            this.cols = columns;
            this.rows = rows;
        }

        public void CreateTiles(int screenW, int screenH)
        {
            tiles.Clear();
            revealedThisTurn.Clear();
            mismatchTimer = 0f;

            // Character names for the tiles
            string[] characterNames = {
                "Aren", "Cisco", "ENGage", "Euriepidies", "Jiyo", "Kuzuri",
                "Marky", "Meanly", "Moon^2", "N1by", "Nyte", "Proksy", "Sia", "Zakkiyan"
            };

            // prepare pairs
            int pairs = cols * rows / 2;
            var characterPairs = new List<string>();
            for (int i = 0; i < pairs; i++)
            {
                characterPairs.Add(characterNames[i]);
                characterPairs.Add(characterNames[i]);
            }

            // shuffle
            characterPairs = characterPairs.OrderBy(x => rng.Next()).ToList();

            // layout - create a proper grid with good spacing
            float padding = 15; // Increased spacing between tiles
            float margin = 50; // Margin from screen edges
            float gridWAvail = screenW - 2 * margin;
            float gridHAvail = screenH - 200; // Leave space for UI at top and bottom

            // Calculate tile size to fit the grid nicely
            float maxTileW = (gridWAvail - (cols - 1) * padding) / cols;
            float maxTileH = (gridHAvail - (rows - 1) * padding) / rows;
            float tileSize = Math.Min(maxTileW, maxTileH);

            // Calculate grid dimensions
            float gridW = cols * tileSize + (cols - 1) * padding;
            float gridH = rows * tileSize + (rows - 1) * padding;

            // Center the grid on screen
            float startX = (screenW - gridW) / 2;
            float startY = 120; // Start below the UI area

            int idx = 0;
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    var t = new Tile();
                    t.PairId = idx; // Keep PairId for matching logic
                    t.CharacterName = characterPairs[idx++];
                    t.State = TileState.Closed;

                    // Calculate position with proper spacing
                    float x = startX + c * (tileSize + padding);
                    float y = startY + r * (tileSize + padding);

                    t.Rect = new Rectangle(x, y, tileSize, tileSize);
                    t.X = x;
                    t.Y = y;
                    t.W = tileSize;
                    t.H = tileSize;
                    tiles.Add(t);
                }
            }
        }

        public void UpdateMismatchTimer(float dt)
        {
            if (mismatchTimer > 0f)
            {
                mismatchTimer -= dt;
                if (mismatchTimer <= 0f)
                {
                    foreach (var t in revealedThisTurn)
                    {
                        if (t.State != TileState.Matched)
                        {
                            t.State = TileState.Closed;
                            t.FlipCount++;
                        }
                    }
                    revealedThisTurn.Clear();
                }
            }
        }

        public bool HandleTileClick(Vector2 mousePos, Action onMatch, Action onMismatch)
        {
            if (mismatchTimer > 0f) return false;

            foreach (var t in tiles)
            {
                if (Raylib.CheckCollisionPointRec(mousePos, t.Rect))
                {
                    if (t.State == TileState.Closed)
                    {
                        t.State = TileState.Revealed;
                        revealedThisTurn.Add(t);

                        if (revealedThisTurn.Count == 2)
                        {
                            var a = revealedThisTurn[0];
                            var b = revealedThisTurn[1];

                            if (a.PairId == b.PairId)
                            {
                                a.State = TileState.Matched;
                                b.State = TileState.Matched;
                                onMatch();
                                revealedThisTurn.Clear();
                            }
                            else
                            {
                                mismatchTimer = mismatchRevealTime;
                                onMismatch();
                            }
                        }
                        return true;
                    }
                    break;
                }
            }
            return false;
        }

        public int UpdateHover(Vector2 mousePos)
        {
            int hoverIndex = -1;
            for (int i = 0; i < tiles.Count; i++)
            {
                if (Raylib.CheckCollisionPointRec(mousePos, tiles[i].Rect))
                {
                    hoverIndex = i;
                    break;
                }
            }

            int previousHoverIndex = lastHoverIndex;
            lastHoverIndex = hoverIndex;
            return (hoverIndex != previousHoverIndex && hoverIndex >= 0 &&
                   tiles[hoverIndex].State == TileState.Closed && mismatchTimer <= 0f) ? hoverIndex : -1;
        }

        public void ShowAllTiles() => tiles.ForEach(t => t.State = TileState.Revealed);
        public void HideAllTiles() => tiles.ForEach(t => t.State = TileState.Closed);
        public void ResetRevealedTiles()
        {
            foreach (var t in tiles.Where(x => x.State == TileState.Revealed))
            {
                t.State = TileState.Closed;
                t.FlipCount++;
            }
            revealedThisTurn.Clear();
        }

        public bool CheckVictory() => tiles.All(x => x.State == TileState.Matched);
        public List<Tile> GetTiles() => tiles;
        public int GetLastHoverIndex() => lastHoverIndex;
    }
}
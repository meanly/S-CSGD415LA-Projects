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
        private List<Tile> revealedTiles = new();
        private Random random = new();
        private int lastHoverIndex = -1;
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
            revealedTiles.Clear();
            mismatchTimer = 0f;

            // Character names
            string[] characters = {
                "Aren", "Cisco", "ENGage", "Euriepidies", "Jiyo", "Kuzuri",
                "Marky", "Meanly", "Moon^2", "N1by", "Nyte", "Proksy", "Sia", "Zakkiyan"
            };

            // Create pairs
            var pairs = new List<string>();
            for (int i = 0; i < 14; i++)
            {
                pairs.Add(characters[i]);
                pairs.Add(characters[i]);
            }

            // Shuffle pairs
            pairs = pairs.OrderBy(x => random.Next()).ToList();

            // Calculate tile size and position
            float padding = 15;
            float margin = 50;
            float availableWidth = screenW - 2 * margin;
            float availableHeight = screenH - 200;
            float tileSize = Math.Min(
                (availableWidth - (cols - 1) * padding) / cols,
                (availableHeight - (rows - 1) * padding) / rows
            );

            float gridWidth = cols * tileSize + (cols - 1) * padding;
            float gridHeight = rows * tileSize + (rows - 1) * padding;
            float startX = (screenW - gridWidth) / 2;
            float startY = 120;

            // Create tiles
            int index = 0;
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    var tile = new Tile();
                    tile.PairId = index;
                    tile.CharacterName = pairs[index++];
                    tile.State = TileState.Closed;

                    float x = startX + col * (tileSize + padding);
                    float y = startY + row * (tileSize + padding);

                    tile.Rect = new Rectangle(x, y, tileSize, tileSize);
                    tile.X = x;
                    tile.Y = y;
                    tile.W = tileSize;
                    tile.H = tileSize;
                    tiles.Add(tile);
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
                    foreach (var tile in revealedTiles)
                    {
                        if (tile.State != TileState.Matched)
                        {
                            tile.State = TileState.Closed;
                            tile.FlipCount++;
                        }
                    }
                    revealedTiles.Clear();
                }
            }
        }

        public bool HandleTileClick(Vector2 mousePos, Action onMatch, Action onMismatch)
        {
            if (mismatchTimer > 0f) return false;

            foreach (var tile in tiles)
            {
                if (Raylib.CheckCollisionPointRec(mousePos, tile.Rect))
                {
                    if (tile.State == TileState.Closed)
                    {
                        tile.State = TileState.Revealed;
                        revealedTiles.Add(tile);

                        if (revealedTiles.Count == 2)
                        {
                            var tile1 = revealedTiles[0];
                            var tile2 = revealedTiles[1];

                            if (tile1.PairId == tile2.PairId)
                            {
                                tile1.State = TileState.Matched;
                                tile2.State = TileState.Matched;
                                onMatch();
                                revealedTiles.Clear();
                            }
                            else
                            {
                                mismatchTimer = 0.8f;
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

            int previousHover = lastHoverIndex;
            lastHoverIndex = hoverIndex;
            return (hoverIndex != previousHover && hoverIndex >= 0 &&
                   tiles[hoverIndex].State == TileState.Closed && mismatchTimer <= 0f) ? hoverIndex : -1;
        }

        public void ShowAllTiles() => tiles.ForEach(t => t.State = TileState.Revealed);
        public void HideAllTiles() => tiles.ForEach(t => t.State = TileState.Closed);

        public void ResetRevealedTiles()
        {
            foreach (var tile in tiles.Where(x => x.State == TileState.Revealed))
            {
                tile.State = TileState.Closed;
                tile.FlipCount++;
            }
            revealedTiles.Clear();
        }

        public bool CheckVictory() => tiles.All(x => x.State == TileState.Matched);
        public List<Tile> GetTiles() => tiles;
        public int GetLastHoverIndex() => lastHoverIndex;
    }
}
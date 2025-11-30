<<<<<<< HEAD
=======
// ...existing code...
>>>>>>> e529e2be160b9e2c672146ba7a232c99408d5735
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
<<<<<<< HEAD
        private List<Tile> revealedTiles = new();
        private Random random = new();
        private int lastHoverIndex = -1;
        private float mismatchTimer = 0f;
        private int cols;
        private int rows;
=======
        private List<Tile> revealedThisTurn = new();
        private Random rng = new();
        private int lastHoverIndex = -1;
        private float mismatchRevealTime = 0.8f;
        private float mismatchTimer = 0f;
        private int cols;
        private int rows;
        private int score = 0;
>>>>>>> e529e2be160b9e2c672146ba7a232c99408d5735

        public TileManager(int columns, int rows)
        {
            this.cols = columns;
            this.rows = rows;
        }

<<<<<<< HEAD
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
=======
        public int Score => score;

        public void CreateTiles(int screenW, int screenH)
        {
            tiles.Clear();
            revealedThisTurn.Clear();
            mismatchTimer = 0f;
            score = 0;

            // Character names for the tiles
            string[] characterNames =
            {
                "Aren",
                "Cisco",
                "ENGage",
                "Euriepidies",
                "Jiyo",
                "Kuzuri",
                "Marky",
                "Meanly",
                "Moon^2",
                "N1by",
                "Nyte",
                "Proksy",
                "Sia",
                "Zakkiyan",
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

            // assign consistent pair ids by name
            var nameToPairId = new Dictionary<string, int>();
            int nextPairId = 0;

            // layout
            float padding = 15;
            float margin = 50;
            float gridWAvail = screenW - 2 * margin;
            float gridHAvail = screenH - 200;

            float maxTileW = (gridWAvail - (cols - 1) * padding) / cols;
            float maxTileH = (gridHAvail - (rows - 1) * padding) / rows;
            float tileSize = Math.Min(maxTileW, maxTileH);
            float gridW = cols * tileSize + (cols - 1) * padding;
            float gridH = rows * tileSize + (rows - 1) * padding;

            // center the grid
            float startX = (screenW - gridW) / 2;
            float startY = 120; 

            int idx = 0;
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    var t = new Tile();
                    // lookup/assign pair id by character name
                    t.CharacterName = characterPairs[idx];
                    if (!nameToPairId.TryGetValue(t.CharacterName, out var pid))
                    {
                        pid = nextPairId++;
                        nameToPairId[t.CharacterName] = pid;
                    }
                    t.PairId = pid;
                    idx++;

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
>>>>>>> e529e2be160b9e2c672146ba7a232c99408d5735
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
<<<<<<< HEAD
                    foreach (var tile in revealedTiles)
                    {
                        if (tile.State != TileState.Matched)
                        {
                            tile.State = TileState.Closed;
                            tile.FlipCount++;
                        }
                    }
                    revealedTiles.Clear();
=======
                    foreach (var t in revealedThisTurn)
                    {
                        if (t.State != TileState.Matched)
                        {
                            t.State = TileState.Closed;
                            t.FlipCount++;
                        }
                    }
                    revealedThisTurn.Clear();
>>>>>>> e529e2be160b9e2c672146ba7a232c99408d5735
                }
            }
        }

        public bool HandleTileClick(Vector2 mousePos, Action onMatch, Action onMismatch)
        {
<<<<<<< HEAD
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
=======
            if (mismatchTimer > 0f)
                return false;

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
                                score++; // increment score on successful match
                                onMatch();
                                revealedThisTurn.Clear();
                            }
                            else
                            {
                                mismatchTimer = mismatchRevealTime;
>>>>>>> e529e2be160b9e2c672146ba7a232c99408d5735
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

<<<<<<< HEAD
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
=======
            int previousHoverIndex = lastHoverIndex;
            lastHoverIndex = hoverIndex;
            return (
                hoverIndex != previousHoverIndex
                && hoverIndex >= 0
                && tiles[hoverIndex].State == TileState.Closed
                && mismatchTimer <= 0f
            )
                ? hoverIndex
                : -1;
        }

        public void ShowAllTiles() => tiles.ForEach(t => { if (t.State != TileState.Matched) t.State = TileState.Revealed; });

        public void HideAllTiles() => tiles.ForEach(t => { if (t.State != TileState.Matched) t.State = TileState.Closed; });

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

>>>>>>> e529e2be160b9e2c672146ba7a232c99408d5735
        public int GetLastHoverIndex() => lastHoverIndex;
    }
}
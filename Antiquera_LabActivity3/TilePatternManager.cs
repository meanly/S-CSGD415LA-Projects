using Raylib_cs;
using System.Numerics;

namespace Antiquera_LabActivity3
{
    // TilePatternManager class managing all 20 patterns
    public class TilePatternManager
    {
        private List<TilePattern> patterns;
        private Random random;

        public TilePatternManager()
        {
            patterns = new List<TilePattern>();
            random = new Random();
            InitializePatterns();
        }

        private void InitializePatterns()
        {
            // Pattern 1: Single tile
            patterns.Add(new TilePattern(1, "Single", new List<Vector2> { new Vector2(0, 0) }, GetRandomColor()));

            // Pattern 2: 2x2 square
            patterns.Add(new TilePattern(2, "Square", new List<Vector2>
            {
                new Vector2(0, 0), new Vector2(1, 0),
                new Vector2(0, 1), new Vector2(1, 1)
            }, GetRandomColor()));

            // Pattern 3: Vertical line (4 tiles)
            patterns.Add(new TilePattern(3, "Vertical Line", new List<Vector2>
            {
                new Vector2(0, 0), new Vector2(0, 1),
                new Vector2(0, 2), new Vector2(0, 3)
            }, GetRandomColor()));

            // Pattern 4: Horizontal line (4 tiles)
            patterns.Add(new TilePattern(4, "Horizontal Line", new List<Vector2>
            {
                new Vector2(0, 0), new Vector2(1, 0),
                new Vector2(2, 0), new Vector2(3, 0)
            }, GetRandomColor()));

            // Pattern 5: L-shape (right)
            patterns.Add(new TilePattern(5, "L-Right", new List<Vector2>
            {
                new Vector2(0, 0), new Vector2(0, 1),
                new Vector2(1, 1), new Vector2(2, 1)
            }, GetRandomColor()));

            // Pattern 6: L-shape (down)
            patterns.Add(new TilePattern(6, "L-Down", new List<Vector2>
            {
                new Vector2(0, 0), new Vector2(1, 0),
                new Vector2(0, 1), new Vector2(0, 2)
            }, GetRandomColor()));

            // Pattern 7: L-shape (left)
            patterns.Add(new TilePattern(7, "L-Left", new List<Vector2>
            {
                new Vector2(0, 0), new Vector2(1, 0),
                new Vector2(2, 0), new Vector2(2, 1)
            }, GetRandomColor()));

            // Pattern 8: T-shape (down)
            patterns.Add(new TilePattern(8, "T-Down", new List<Vector2>
            {
                new Vector2(1, 0), new Vector2(1, 1),
                new Vector2(0, 2), new Vector2(1, 2)
            }, GetRandomColor()));

            // Pattern 9: L-shape (up)
            patterns.Add(new TilePattern(9, "L-Up", new List<Vector2>
            {
                new Vector2(0, 0), new Vector2(1, 0),
                new Vector2(2, 0), new Vector2(0, 1)
            }, GetRandomColor()));

            // Pattern 10: Corner (top-right)
            patterns.Add(new TilePattern(10, "Corner-TR", new List<Vector2>
            {
                new Vector2(0, 0), new Vector2(0, 1),
                new Vector2(0, 2), new Vector2(1, 2)
            }, GetRandomColor()));

            // Pattern 11: Corner (bottom-right)
            patterns.Add(new TilePattern(11, "Corner-BR", new List<Vector2>
            {
                new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(2, 1), new Vector2(2, 0)
            }, GetRandomColor()));

            // Pattern 12: T-shape (right)
            patterns.Add(new TilePattern(12, "T-Right", new List<Vector2>
            {
                new Vector2(0, 0), new Vector2(1, 0),
                new Vector2(1, 1), new Vector2(1, 2)
            }, GetRandomColor()));

            // Pattern 13: Z-shape (horizontal)
            patterns.Add(new TilePattern(13, "Z-Horizontal", new List<Vector2>
            {
                new Vector2(0, 0), new Vector2(1, 0),
                new Vector2(1, 1), new Vector2(2, 1)
            }, GetRandomColor()));

            // Pattern 14: Cross
            patterns.Add(new TilePattern(14, "Cross", new List<Vector2>
            {
                new Vector2(1, 0), new Vector2(0, 1),
                new Vector2(0, 2), new Vector2(1, 1)
            }, GetRandomColor()));

            // Pattern 15: S-shape (horizontal)
            patterns.Add(new TilePattern(15, "S-Horizontal", new List<Vector2>
            {
                new Vector2(0, 0), new Vector2(0, 1),
                new Vector2(1, 1), new Vector2(1, 2)
            }, GetRandomColor()));

            // Pattern 16: Corner (bottom-left)
            patterns.Add(new TilePattern(16, "Corner-BL", new List<Vector2>
            {
                new Vector2(1, 0), new Vector2(2, 0),
                new Vector2(1, 1), new Vector2(0, 1)
            }, GetRandomColor()));

            // Pattern 17: T-shape (up)
            patterns.Add(new TilePattern(17, "T-Up", new List<Vector2>
            {
                new Vector2(0, 0), new Vector2(1, 0),
                new Vector2(2, 0), new Vector2(1, 1)
            }, GetRandomColor()));

            // Pattern 18: Corner (top-left)
            patterns.Add(new TilePattern(18, "Corner-TL", new List<Vector2>
            {
                new Vector2(0, 0), new Vector2(0, 1),
                new Vector2(1, 1), new Vector2(0, 2)
            }, GetRandomColor()));

            // Pattern 19: T-shape (left)
            patterns.Add(new TilePattern(19, "T-Left", new List<Vector2>
            {
                new Vector2(1, 0), new Vector2(0, 1),
                new Vector2(1, 1), new Vector2(2, 1)
            }, GetRandomColor()));

            // Pattern 20: Plus
            patterns.Add(new TilePattern(20, "Plus", new List<Vector2>
            {
                new Vector2(1, 0), new Vector2(1, 1),
                new Vector2(0, 1), new Vector2(1, 2)
            }, GetRandomColor()));
        }

        private TileColor GetRandomColor()
        {
            var colors = Enum.GetValues<TileColor>().Where(c => c != TileColor.Black).ToArray();
            return colors[random.Next(colors.Length)];
        }

        public List<TilePattern> GetRandomPatterns(int count)
        {
            var shuffled = patterns.OrderBy(x => random.Next()).ToList();
            return shuffled.Take(count).ToList();
        }

        public TilePattern? GetPattern(int id)
        {
            return patterns.FirstOrDefault(p => p.Id == id);
        }
    }
}

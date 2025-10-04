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
            // Pattern 1: Single tile - tileSet[x][y]
            var pattern1 = new TileColor[1, 1];
            pattern1[0, 0] = TileColor.Violet;
            patterns.Add(new TilePattern(1, "Single", pattern1));

            // Pattern 2: 2x2 square
            // tileSet[x][y], tileSet[x+1][y], tileSet[x][y+1], tileSet[x+1][y+1]
            var pattern2 = new TileColor[2, 2];
            var color2 = TileColor.Orange;
            pattern2[0, 0] = color2; pattern2[1, 0] = color2;
            pattern2[0, 1] = color2; pattern2[1, 1] = color2;
            patterns.Add(new TilePattern(2, "Square", pattern2));

            // Pattern 3: Vertical line (4 tiles)
            // tileSet[x][y], tileSet[x][y+1], tileSet[x][y+2], tileSet[x][y+3]
            var pattern3 = new TileColor[1, 4];
            var color3 = TileColor.Red;
            pattern3[0, 0] = color3; pattern3[0, 1] = color3;
            pattern3[0, 2] = color3; pattern3[0, 3] = color3;
            patterns.Add(new TilePattern(3, "Vertical Line", pattern3));

            // Pattern 4: Horizontal line (4 tiles)
            // tileSet[x][y], tileSet[x+1][y], tileSet[x+2][y], tileSet[x+3][y]
            var pattern4 = new TileColor[4, 1];
            var color4 = TileColor.Red;
            pattern4[0, 0] = color4; pattern4[1, 0] = color4;
            pattern4[2, 0] = color4; pattern4[3, 0] = color4;
            patterns.Add(new TilePattern(4, "Horizontal Line", pattern4));

            // Pattern 5: L-shape (right)
            // tileSet[x][y], tileSet[x][y+1], tileSet[x+1][y+1], tileSet[x+2][y+1]
            var pattern5 = new TileColor[3, 2];
            var color5 = TileColor.Orange;
            pattern5[0, 0] = color5; pattern5[0, 1] = color5;
            pattern5[1, 1] = color5; pattern5[2, 1] = color5;
            patterns.Add(new TilePattern(5, "L-Right", pattern5));

            // Pattern 6: L-shape (down)
            // tileSet[x][y], tileSet[x+1][y], tileSet[x][y+1], tileSet[x][y+2]
            var pattern6 = new TileColor[2, 3];
            var color6 = TileColor.Blue;
            pattern6[0, 0] = color6; pattern6[1, 0] = color6;
            pattern6[0, 1] = color6; pattern6[0, 2] = color6;
            patterns.Add(new TilePattern(6, "L-Down", pattern6));

            // Pattern 7: L-shape (left)
            // tileSet[x][y], tileSet[x+1][y], tileSet[x+2][y], tileSet[x+2][y+1]
            var pattern7 = new TileColor[3, 2];
            var color7 = TileColor.Blue;
            pattern7[0, 0] = color7; pattern7[1, 0] = color7;
            pattern7[2, 0] = color7; pattern7[2, 1] = color7;
            patterns.Add(new TilePattern(7, "L-Left", pattern7));

            // Pattern 8: T-shape (down)
            // tileSet[x+1][y], tileSet[x+1][y+1], tileSet[x][y+2], tileSet[x+1][y+2]
            var pattern8 = new TileColor[2, 3];
            var color8 = TileColor.Pink;
            pattern8[1, 0] = color8; pattern8[1, 1] = color8;
            pattern8[0, 2] = color8; pattern8[1, 2] = color8;
            patterns.Add(new TilePattern(8, "T-Down", pattern8));

            // Pattern 9: L-shape (up)
            // tileSet[x][y], tileSet[x+1][y], tileSet[x+2][y], tileSet[x][y+1]
            var pattern9 = new TileColor[3, 2];
            var color9 = TileColor.Blue;
            pattern9[0, 0] = color9; pattern9[1, 0] = color9;
            pattern9[2, 0] = color9; pattern9[0, 1] = color9;
            patterns.Add(new TilePattern(9, "L-Up", pattern9));

            // Pattern 10: Corner (top-right)
            // tileSet[x][y], tileSet[x][y+1], tileSet[x][y+2], tileSet[x+1][y+2]
            var pattern10 = new TileColor[2, 3];
            var color10 = GetRandomColor();
            pattern10[0, 0] = color10; pattern10[0, 1] = color10;
            pattern10[0, 2] = color10; pattern10[1, 2] = color10;
            patterns.Add(new TilePattern(10, "Corner-TR", pattern10));

            // Pattern 11: Corner (bottom-right)
            // tileSet[x][y+1], tileSet[x+1][y+1], tileSet[x+2][y+1], tileSet[x+2][y]
            var pattern11 = new TileColor[3, 2];
            var color11 = GetRandomColor();
            pattern11[0, 1] = color11; pattern11[1, 1] = color11;
            pattern11[2, 1] = color11; pattern11[2, 0] = color11;
            patterns.Add(new TilePattern(11, "Corner-BR", pattern11));

            // Pattern 12: T-shape (right)
            // tileSet[x][y], tileSet[x+1][y], tileSet[x+1][y+1], tileSet[x+1][y+2]
            var pattern12 = new TileColor[2, 3];
            var color12 = TileColor.Pink;
            pattern12[0, 0] = color12; pattern12[1, 0] = color12;
            pattern12[1, 1] = color12; pattern12[1, 2] = color12;
            patterns.Add(new TilePattern(12, "T-Right", pattern12));

            // Pattern 13: Z-shape (horizontal)
            // tileSet[x][y], tileSet[x+1][y], tileSet[x+1][y+1], tileSet[x+2][y+1]
            var pattern13 = new TileColor[3, 2];
            var color13 = TileColor.LimeGreen;
            pattern13[0, 0] = color13; pattern13[1, 0] = color13;
            pattern13[1, 1] = color13; pattern13[2, 1] = color13;
            patterns.Add(new TilePattern(13, "Z-Horizontal", pattern13));

            // Pattern 14: Cross
            // tileSet[x+1][y], tileSet[x][y+1], tileSet[x][y+2], tileSet[x+1][y+1]
            var pattern14 = new TileColor[2, 3];
            var color14 = GetRandomColor();
            pattern14[1, 0] = color14; pattern14[0, 1] = color14;
            pattern14[0, 2] = color14; pattern14[1, 1] = color14;
            patterns.Add(new TilePattern(14, "Cross", pattern14));

            // Pattern 15: S-shape (horizontal)
            // tileSet[x][y], tileSet[x][y+1], tileSet[x+1][y+1], tileSet[x+1][y+2]
            var pattern15 = new TileColor[2, 3];
            var color15 = TileColor.Cyan;
            pattern15[0, 0] = color15; pattern15[0, 1] = color15;
            pattern15[1, 1] = color15; pattern15[1, 2] = color15;
            patterns.Add(new TilePattern(15, "S-Horizontal", pattern15));

            // Pattern 16: Corner (bottom-left)
            // tileSet[x+1][y], tileSet[x+2][y], tileSet[x+1][y+1], tileSet[x][y+1]
            var pattern16 = new TileColor[3, 2];
            var color16 = GetRandomColor();
            pattern16[1, 0] = color16; pattern16[2, 0] = color16;
            pattern16[1, 1] = color16; pattern16[0, 1] = color16;
            patterns.Add(new TilePattern(16, "Corner-BL", pattern16));

            // Pattern 17: T-shape (up)
            // tileSet[x][y], tileSet[x+1][y], tileSet[x+2][y], tileSet[x+1][y+1]
            var pattern17 = new TileColor[3, 2];
            var color17 = TileColor.Pink;
            pattern17[0, 0] = color17; pattern17[1, 0] = color17;
            pattern17[2, 0] = color17; pattern17[1, 1] = color17;
            patterns.Add(new TilePattern(17, "T-Up", pattern17));

            // Pattern 18: Corner (top-left)
            // tileSet[x][y], tileSet[x][y+1], tileSet[x+1][y+1], tileSet[x][y+2]
            var pattern18 = new TileColor[2, 3];
            var color18 = GetRandomColor();
            pattern18[0, 0] = color18; pattern18[0, 1] = color18;
            pattern18[1, 1] = color18; pattern18[0, 2] = color18;
            patterns.Add(new TilePattern(18, "Corner-TL", pattern18));

            // Pattern 19: T-shape (left)
            // tileSet[x+1][y], tileSet[x][y+1], tileSet[x+1][y+1], tileSet[x+2][y+1]
            var pattern19 = new TileColor[3, 2];
            var color19 = TileColor.Pink;
            pattern19[1, 0] = color19; pattern19[0, 1] = color19;
            pattern19[1, 1] = color19; pattern19[2, 1] = color19;
            patterns.Add(new TilePattern(19, "T-Left", pattern19));

            // Pattern 20: Plus
            // tileSet[x+1][y], tileSet[x+1][y+1], tileSet[x][y+1], tileSet[x+1][y+2]
            var pattern20 = new TileColor[2, 3];
            var color20 = GetRandomColor();
            pattern20[1, 0] = color20; pattern20[1, 1] = color20;
            pattern20[0, 1] = color20; pattern20[1, 2] = color20;
            patterns.Add(new TilePattern(20, "Plus", pattern20));
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

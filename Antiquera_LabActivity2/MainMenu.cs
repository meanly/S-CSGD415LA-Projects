using Raylib_cs;
using System.Numerics;

public class MainMenu
{
    private windowSize gameSize;
    private int selectedOption;
    private readonly string[] menuOptions = { "Start Game", "About", "Quit" };
    private readonly int fontSize = 40;
    private readonly int titleFontSize = 60;

    public MainMenu(int width, int height)
    {
        gameSize = new windowSize { width = width, height = height };
        selectedOption = 0;
    }

    public void Update()
    {
        // Handle keyboard navigation
        if (Raylib.IsKeyPressed(KeyboardKey.Up) || Raylib.IsKeyPressed(KeyboardKey.W))
        {
            selectedOption = (selectedOption - 1 + menuOptions.Length) % menuOptions.Length;
        }

        if (Raylib.IsKeyPressed(KeyboardKey.Down) || Raylib.IsKeyPressed(KeyboardKey.S))
        {
            selectedOption = (selectedOption + 1) % menuOptions.Length;
        }

        // Handle selection
        if (Raylib.IsKeyPressed(KeyboardKey.Enter) || Raylib.IsKeyPressed(KeyboardKey.Space))
        {
            if (selectedOption == 0) // Start Game
            {
                GameState.SetState(GameState.State.Playing);
            }
            else if (selectedOption == 1) // About
            {
                GameState.SetState(GameState.State.About);
            }
            else if (selectedOption == 2) // Quit
            {
                Raylib.CloseWindow();
            }
        }
    }

    public void Draw()
    {
        // Draw background
        Raylib.ClearBackground(new Color(30, 30, 60, 255)); // Dark blue background

        // Draw title
        string title = "INSANIQUARIUM CLONE";
        int titleWidth = Raylib.MeasureText(title, titleFontSize);
        Raylib.DrawText(title,
            (gameSize.width - titleWidth) / 2,
            gameSize.height / 4,
            titleFontSize,
            Color.Yellow);

        // Draw menu options
        int startY = gameSize.height / 2;
        for (int i = 0; i < menuOptions.Length; i++)
        {
            Color textColor = (i == selectedOption) ? Color.White : Color.Gray;
            int textWidth = Raylib.MeasureText(menuOptions[i], fontSize);

            // Draw selection indicator
            if (i == selectedOption)
            {
                Raylib.DrawText(">",
                    (gameSize.width - textWidth) / 2 - 40,
                    startY + i * 60,
                    fontSize,
                    Color.White);
            }

            Raylib.DrawText(menuOptions[i],
                (gameSize.width - textWidth) / 2,
                startY + i * 60,
                fontSize,
                textColor);
        }

        // Draw instructions
        string instructions = "Use UP/DOWN or W/S to navigate, ENTER or SPACE to select";
        int instructionWidth = Raylib.MeasureText(instructions, 20);
        Raylib.DrawText(instructions,
            (gameSize.width - instructionWidth) / 2,
            gameSize.height - 50,
            20,
            Color.LightGray);
    }
}

// About screen class
public class AboutScreen
{
    private windowSize gameSize;

    public AboutScreen(int width, int height)
    {
        gameSize = new windowSize { width = width, height = height };
    }

    public void Update()
    {
        // Handle back to main menu
        if (Raylib.IsKeyPressed(KeyboardKey.Escape) || Raylib.IsKeyPressed(KeyboardKey.Backspace))
        {
            GameState.SetState(GameState.State.MainMenu);
        }
    }

    public void Draw()
    {
        // Draw background
        Raylib.ClearBackground(new Color(30, 30, 60, 255)); // Dark blue background

        // Draw title
        string title = "ABOUT";
        int titleWidth = Raylib.MeasureText(title, 60);
        Raylib.DrawText(title,
            (gameSize.width - titleWidth) / 2,
            80,
            60,
            Color.Yellow);

        // Draw game information
        string[] aboutText = {
            "INSANIQUARIUM CLONE",
            "",
            "A fish tank simulation game where you:",
            "• Feed fish with food pellets",
            "• Collect coins dropped by fish",
            "• Buy new fish to expand your tank",
            "• Keep your fish happy and healthy",
            "",
            "CONTROLS:",
            "• Left Click: Collect coins",
            "• Right Click: Drop food pellets",
            "• B Key: Buy new fish (costs 20 coins)",
            "• ESC: Return to main menu",
            "",
            "Keep your fish alive and earn coins!",
            "Don't let all your fish die or run out of money!"
        };

        int startY = 150;
        int lineHeight = 30;

        foreach (string line in aboutText)
        {
            if (line == "")
            {
                startY += lineHeight / 2; // Add smaller spacing for empty lines
                continue;
            }

            int textWidth = Raylib.MeasureText(line, 24);
            Color textColor = line.StartsWith("•") ? Color.SkyBlue :
                             line == "CONTROLS:" || line == "INSANIQUARIUM CLONE" ? Color.Yellow :
                             Color.White;

            Raylib.DrawText(line,
                (gameSize.width - textWidth) / 2,
                startY,
                24,
                textColor);

            startY += lineHeight;
        }

        // Draw back instruction
        string backText = "Press ESC or BACKSPACE to return to main menu";
        int backWidth = Raylib.MeasureText(backText, 20);
        Raylib.DrawText(backText,
            (gameSize.width - backWidth) / 2,
            gameSize.height - 50,
            20,
            Color.LightGray);
    }
}

// Game state management
public static class GameState
{
    public enum State
    {
        MainMenu,
        Playing,
        GameOver,
        About
    }

    private static State currentState = State.MainMenu;

    public static State GetState()
    {
        return currentState;
    }

    public static void SetState(State newState)
    {
        currentState = newState;
    }
}

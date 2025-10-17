# Memory Game - C# Raylib

A memory matching game built with C# and Raylib-cs. Match pairs of character tiles within the time limit!

## Features

- **4x7 Tile Board**: 28 tiles with 14 unique character pairs
- **Game States**: Ongoing, Paused, Game Over, and Victory
- **Health System**: 3 lives with visual health bar
- **Timer System**: 30-second timer per move with visual timer bar
- **Scoring System**: Earn points for successful matches
- **Sound Effects**: Audio feedback for all game actions
- **Background Music**: Atmospheric music during gameplay

## Controls

- **Left Click**: Flip a tile
- **P Key**: Pause/Resume game
- **R Key**: Restart game (on Game Over or Victory screen)

## Game Rules

1. Click on tiles to reveal character images
2. Match pairs of identical characters
3. Complete all matches before running out of time or health
4. Each wrong match costs you a life
5. Timer resets after each move (correct or incorrect)

## Installation

1. Install .NET 6.0 or higher
2. Install required dependencies:
   ```bash
   dotnet restore
   ```

## Running the Game

```bash
dotnet run
```

## Building the Game

```bash
dotnet build
```

## Project Structure

- `Game.cs` - Main entry point and game loop
- `GameManager.cs` - Game state management and logic
- `TileManager.cs` - Tile board management and matching logic
- `Tile.cs` - Individual tile data and behavior
- `TextureHandler.cs` - Rendering and graphics management
- `SoundManager.cs` - Audio management and sound effects
- `MemoryGame.csproj` - Project file with dependencies

## Assets

The game uses the following assets from the `img/` and `audio/` folders:

- Character images for tile matching
- UI elements (health bar, timer bar, buttons)
- Screen overlays for different game states
- Sound effects for game actions
- Background music

## Game States

- **Ongoing**: Active gameplay
- **Paused**: Game paused (press P to resume)
- **Game Over**: No lives remaining (press R to restart)
- **Win**: All tiles matched (press R to restart)

## Dependencies

- .NET 6.0 or higher
- Raylib-cs 4.5.0

Enjoy the game!
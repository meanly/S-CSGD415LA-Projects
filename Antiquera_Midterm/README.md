# Memory Game - Simple Color Version

A beginner-friendly memory/concentration game built with Raylib-cs, using colored tiles instead of images.

## Features

- 4x7 grid (28 tiles, 14 pairs)
- Color-coded tiles for each character
- Health system (8 hearts)
- Timer system (8 seconds per turn)
- Score tracking
- Preview mode at game start
- Pause functionality

## Controls

- **Mouse Click**: Flip tiles
- **P**: Pause/Resume game
- **R**: Restart after win/loss
- **X**: Start game from loading screen

## How to Play

1. The game starts with a 3-second preview showing all tiles
2. Click on tiles to reveal them
3. Match pairs of tiles with the same character name
4. Complete all pairs before running out of health or time
5. Each wrong match costs 1 health point
6. Timer resets when you make a move

## Character Colors

Each character has a unique color:

- Aren: Red
- Cisco: Blue
- ENGage: Green
- Euriepidies: Yellow
- Jiyo: Orange
- Kuzuri: Purple
- Marky: Pink
- Meanly: Brown
- Moon^2: Sky Blue
- N1by: Lime
- Nyte: Dark Blue
- Proksy: Magenta
- Sia: Cyan
- Zakkiyan: Gold

## Building and Running

```bash
dotnet build
dotnet run
```

## Code Structure

- **Program.cs**: Entry point
- **Game.cs**: Main game loop
- **GameManager.cs**: Game logic and state management
- **TileManager.cs**: Tile handling and matching logic
- **TextureHandler.cs**: Drawing tiles and overlays
- **Tile.cs**: Tile data structure
- **Palette.cs**: Color definitions

## Requirements

- .NET 9.0
- Raylib-cs 7.0.1

## Beginner-Friendly Features

- No audio dependencies
- Simple color-based graphics
- Clean, readable code structure
- Minimal external dependencies
- Easy to understand game logic

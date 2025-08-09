# Grid-Based Movement Assignment

This Unity project demonstrates grid-based movement for a player and an enemy, including obstacle avoidance and tile selection via mouse input.

## Features

- **Grid Movement:** Both player and enemy move on a 2D grid.
- **Obstacle Handling:** Obstacles are defined using a ScriptableObject (`ObstacleData`).
- **Pathfinding:** A* pathfinding is used to find valid paths around obstacles.
- **Player & Enemy Logic:**
  - The player moves to a clicked tile if it is walkable and not occupied by the enemy.
  - The enemy moves to the nearest walkable neighbor of the player's target tile.
  - Both avoid moving onto each other's positions.
- **UI Feedback:** Displays the grid coordinates of the tile under the mouse cursor.

## Scripts Overview

- **ObstacleData.cs:** Stores obstacle positions and provides walkability checks.
- **GridMover.cs:** Abstract base class for grid movement and pathfinding logic.
- **Player.cs:** Inherits from `GridMover`. Handles player-specific movement.
- **Enemy.cs:** Inherits from `GridMover`. Handles enemy-specific movement logic.
- **Hovering.cs:** Handles mouse input, tile selection, and UI updates.

## How to Use

1. **Setup:**
   - Attach the `Hovering` script to a GameObject in your scene.
   - Assign the required references in the Inspector:
     - `tileLayer`: LayerMask for grid tiles.
     - `positionText`: UI Text element for displaying coordinates.
     - `obstacleData`: Reference to your `ObstacleData` asset.
     - `player` and `enemy`: References to the respective GameObjects.
(if not already done)

2. **Running:**
   - Hover the mouse over grid tiles to see their coordinates.
   - Click a walkable tile to move the player; the enemy will respond accordingly.

## Customization

- **Obstacles:** Use the provided Unity tool/editor to add or remove obstacles directly on the grid. The `ObstacleData` asset will update automatically as you modify obstacles in the scene.
- **Grid Size:** Adjust `gridWidth` and `gridHeight` in the `GridMover`-derived scripts.
- **Movement Speed:** Change the `moveSpeed` property.

## Requirements

- Unity 2020.3 or newer (recommended)

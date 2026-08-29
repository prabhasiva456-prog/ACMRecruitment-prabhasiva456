# Neon Survival — Gameplay Mechanics

Neon Survival is a compact 2D arena game built in Unity. Move through the arena, collect energy crystals, avoid or attack chasing enemies, and survive for as long as possible.

## How to play

- **Move:** `A` / `D` or Left / Right arrows
- **Jump:** `Space` (press again in mid-air for a double jump)
- **Attack:** `F` or Left Mouse Button
- **Restart:** `R` or the on-screen Restart button after Game Over

## Gameplay mechanics implemented

- Player health with damage, brief invulnerability, knockback, and a visual health bar
- Patrolling/chasing enemy AI that damages the player on contact
- Solid platforms, arena walls, hazards, and physics-based collision detection
- Collectible energy crystals worth 10 points each
- Score and health UI, plus control hints
- Game Over when health reaches zero or the player falls out of the arena
- Restart button and keyboard restart
- Increasing challenge: enemies become faster as the score rises

## Bonus features

- **Enemy AI:** enemies patrol until the player approaches, then chase and jump over obstacles
- **Double jump:** the player can jump once more while airborne
- **Attack system:** a short-range pulse damages enemies; defeated enemies award 25 points

## Scripts

- `GameBootstrap.cs` — constructs the arena, camera, lighting-style background, UI, player, enemies, hazards, and collectibles at runtime
- `GameManager.cs` — owns score, difficulty scaling, game-over state, UI updates, and restarting
- `PlayerController.cs` — movement, grounded checks, double jump, attack input, and animation-like squash/tilt feedback
- `PlayerHealth.cs` — health, damage cooldown, knockback, death, and health-bar synchronization
- `EnemyAI.cs` — patrol/chase state, obstacle jumping, contact damage, enemy health, and defeat scoring
- `Collectible.cs` — animated collectible behavior and score awarding
- `Hazard.cs` — collision damage from spikes
- `CameraFollow.cs` — smooth camera tracking constrained to the arena

## Opening the project

1. Open the `UnityProject` folder in Unity Hub (Unity 2022.3 LTS or newer is recommended).
2. Open `Assets/Scenes/Main.unity`.
3. Press Play.

The level is intentionally generated in code, so no external art packages or manual inspector setup are required.

## Evidence

Add an exported gameplay screenshot or video to the `Media` folder after running the project in the Unity Editor. The project contains an Editor menu command at **Neon Survival → Capture Gameplay Screenshot** for convenient capture while in Play Mode.


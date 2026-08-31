# Unity Foundations

A basic playable 3D Unity scene with a controllable player and physics interactions.

## Features
- Capsule player with a Rigidbody and Capsule Collider.
- C# movement using WASD or arrow keys.
- Ground and a fixed obstacle with colliders.
- A pushable cube with a Rigidbody.

## Controls
- W / Up arrow: Forward
- S / Down arrow: Backward
- A / Left arrow: Left
- D / Right arrow: Right

## How to Run
1. Clone or download this repository.
2. In Unity Hub, add questline-1/unity-foundations/UnityProject.
3. Use Unity 6 Editor **6000.0.82f1**, as recorded in UnityProject/ProjectSettings/ProjectVersion.txt.
4. Open **Assets/Scenes/MainScene.unity**. This scene is also enabled in Build Profiles.
5. Press Play and click inside the Game view.
6. Move into the fixed obstacle to test collisions and into the pushable cube to push it.

## Project Structure
- UnityProject/Assets: Scenes, C# scripts, and assets.
- UnityProject/Packages: Package dependencies.
- UnityProject/ProjectSettings: Unity project configuration.
- Media: Gameplay and Player component screenshots.

## Unity components used
- **GameObject / Transform:** scene objects and their positions, rotations, and scales.
- **Rigidbody:** gravity and physics movement for the player and pushable box. Player rotation is frozen to keep the capsule upright.
- **Colliders:** CapsuleCollider on the player, a ground collider, and BoxColliders on the obstacle and pushable box prevent objects passing through each other.
- **MonoBehaviour:** PlayerMovement reads input in Update and applies horizontal velocity in FixedUpdate, preserving vertical velocity for gravity. Normalized input prevents faster diagonal movement.
- **Camera and light:** display and illuminate the playable scene.

## Screenshots
See [gameplay and component screenshots](Media/). These are the existing captures supplied with the project.

![Gameplay](Media/Screenshot%202026-08-30%20103105.png)

## Verification checklist
- Enter Play mode and use WASD or the arrow keys to move.
- Walk against the fixed obstacle: it should block the capsule.
- Walk into PushableBox: the box should move through Rigidbody physics.
- Check that the capsule stays upright and diagonal movement is no faster than straight movement.
- Active Input Handling must remain **Both** or **Input Manager (Old)** for the legacy Input API; the submitted setting is Both.

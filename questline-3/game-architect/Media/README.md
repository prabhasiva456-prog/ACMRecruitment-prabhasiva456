# Gameplay evidence

These files were captured from Lumen Run running in the Unity 6000.0.82f1 Game view:

- `start-menu.png` — mission menu and controls.
- `gameplay.png` — complete level, player, enemies, hazards, core counter, score, timer, health, and portal state.
- `win.png` — completed mission and final score.
- `game-over.png` — loss state and restart action.
- `gameplay-preview.gif` — browser-friendly animated preview of the Play-mode run.
- `gameplay-video.avi` — 10-second, 12 FPS MJPEG gameplay video.
- `runtime-validation.txt` — all assertions from the automated Play-mode smoke check.
- `capture-log.txt` — capture completion record and frame count.

The gameplay recording follows the production scene from the menu through six real collectible triggers, the win portal, scene restart, and the falling Game Over condition. It contains no mock or borrowed gameplay. The frame-based recording is silent; the Unity project generates and plays its own background melody and sound effects at runtime.

To regenerate the evidence, open the project and choose **Lumen Run > Capture submission media** from Edit mode. The separate **Capture screenshot** and **Start or stop recording frames** commands support manual capture.

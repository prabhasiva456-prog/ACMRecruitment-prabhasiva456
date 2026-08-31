# Gameplay evidence — pending

This folder must contain real captures from Lumen Run before the challenge is fully submitted. Unity's local license activation failed during preparation, so no screenshot or video has been fabricated or borrowed from another game.

## Screenshots

1. Open the project with Unity 6000.0.82f1 and enter Play mode.
2. Set the Game view to 1280 × 720.
3. Choose **Lumen Run > Capture screenshot** at the start menu, during gameplay, after winning, and after Game Over.
4. The tool saves each PNG here with its state and timestamp in the filename.

## Gameplay video

Use a desktop/game recorder to capture the actual Game view, ideally with audio. Show starting the mission, movement, a pickup, combat or hazard damage, the objective, and restart. Save the result as `gameplay.mp4` in this folder.

Alternatively, **Lumen Run > Start or stop recording frames** records real Game-view PNG frames at up to 20 FPS. Run it again to stop. The folder name starts with `Frames-`. With FFmpeg installed, encode the frames from that folder:

```powershell
ffmpeg -framerate 20 -i "frame-%06d.png" -c:v libx264 -pix_fmt yuv420p -movflags +faststart "../gameplay.mp4"
```

The frame recorder has a 2,400-frame cap, does not capture audio, and can slow the editor. A desktop recorder is preferable for smooth footage with game audio. Keep the final MP4 under GitHub's browser file-upload limit; do not commit the frame folders.

After capturing, update the main README with the screenshots and video link and remove its pending-media notice only once all required evidence exists.

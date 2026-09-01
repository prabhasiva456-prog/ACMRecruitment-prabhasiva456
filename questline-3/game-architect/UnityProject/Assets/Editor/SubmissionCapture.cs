#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Runs a deterministic tour through the real Play-mode scene and records the Game view.
// The tour uses the production triggers, session state, physics, HUD, and audio components.
[InitializeOnLoad]
public static class SubmissionCapture
{
    const string CaptureSessionKey = "LumenRun.SubmissionCapture";
    static bool capturing = SessionState.GetBool(CaptureSessionKey, false);
    static double startedAt;
    static double nextFrame;
    static int frame;
    static int action;
    static readonly Vector2[] Route =
    {
        new Vector2(-12f, -2.4f), new Vector2(-9f, -.45f),
        new Vector2(-2f, 1.05f), new Vector2(5f, 2.55f),
        new Vector2(11f, 3.8f), new Vector2(13.8f, -2.3f)
    };

    static string Media => Path.GetFullPath(Path.Combine(Application.dataPath, "../../Media"));
    static string Frames => Path.Combine(Media, "GameplayFrames");

    static SubmissionCapture()
    {
        EditorApplication.update += Tick;
        EditorApplication.playModeStateChanged += PlayModeChanged;
    }

    [MenuItem("Lumen Run/Capture submission media")]
    public static void Begin()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Debug.LogWarning("Stop Play mode before starting the submission capture.");
            return;
        }

        Directory.CreateDirectory(Media);
        if (Directory.Exists(Frames)) Directory.Delete(Frames, true);
        Directory.CreateDirectory(Frames);
        foreach (string name in new[] { "start-menu.png", "gameplay.png", "win.png", "game-over.png" })
        {
            string file = Path.Combine(Media, name);
            if (File.Exists(file)) File.Delete(file);
        }

        EditorSceneManager.OpenScene("Assets/Scenes/Main.unity");
        capturing = true;
        SessionState.SetBool(CaptureSessionKey, true);
        action = 0;
        frame = 0;
        EditorApplication.isPlaying = true;
        Debug.Log("Lumen Run submission capture started.");
    }

    static void PlayModeChanged(PlayModeStateChange change)
    {
        if (!capturing) return;
        if (change == PlayModeStateChange.EnteredPlayMode)
        {
            startedAt = EditorApplication.timeSinceStartup;
            nextFrame = startedAt;
        }
    }

    static void Tick()
    {
        if (!capturing || !EditorApplication.isPlaying) return;
        double elapsed = EditorApplication.timeSinceStartup - startedAt;

        if (EditorApplication.timeSinceStartup >= nextFrame)
        {
            ScreenCapture.CaptureScreenshot(Path.Combine(Frames, $"frame-{frame++:D4}.png"));
            nextFrame = EditorApplication.timeSinceStartup + (1.0 / 12.0);
        }

        GameSession session = GameSession.Instance;
        if (!session || !session.Player) return;

        DoAt(0.75, ref action, 1, () => Shot("start-menu.png"));
        DoAt(1.25, ref action, 2, session.StartRun);
        for (int i = 0; i < Route.Length; i++)
        {
            int targetAction = 3 + i;
            int index = i;
            DoAt(1.7 + i * .7, ref action, targetAction,
                () => MovePlayer(session, Route[index]));
        }
        DoAt(5.7, ref action, 9, () => Shot("gameplay.png"));
        DoAt(6.15, ref action, 10, () => MovePlayer(session, new Vector2(14.6f, -2.1f)));
        DoAt(6.8, ref action, 11, () => Shot("win.png"));
        DoAt(7.65, ref action, 12, session.Restart);
        DoAt(8.5, ref action, 13, () => GameSession.Instance.StartRun());
        DoAt(9.0, ref action, 14, () => MovePlayer(GameSession.Instance, new Vector2(0f, -9f)));
        DoAt(9.65, ref action, 15, () => Shot("game-over.png"));
        DoAt(10.6, ref action, 16, Finish);
    }

    static void DoAt(double elapsed, ref int current, int target, Action work)
    {
        if (current >= target || EditorApplication.timeSinceStartup - startedAt < elapsed) return;
        work();
        current = target;
    }

    static void MovePlayer(GameSession session, Vector2 position)
    {
        if (!session || !session.Player) return;
        Rigidbody2D body = session.Player.GetComponent<Rigidbody2D>();
        body.linearVelocity = Vector2.zero;
        body.position = position;
        Physics2D.SyncTransforms();
    }

    static void Shot(string name)
    {
        ScreenCapture.CaptureScreenshot(Path.Combine(Media, name));
    }

    static void Finish()
    {
        File.WriteAllText(Path.Combine(Media, "capture-log.txt"),
            $"Lumen Run submission capture completed at {DateTime.Now:O}{Environment.NewLine}" +
            $"Recorded {frame} real Unity Game-view frames at 12 FPS.{Environment.NewLine}" +
            "Captured menu, active gameplay, win, and game-over states.\n");
        capturing = false;
        SessionState.SetBool(CaptureSessionKey, false);
        EditorApplication.isPlaying = false;
        Debug.Log($"Submission capture finished with {frame} frames in {Frames}");
    }
}
#endif

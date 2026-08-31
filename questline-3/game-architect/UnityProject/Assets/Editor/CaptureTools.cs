#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

// Records the real Game view. It never generates a mock gameplay image.
[InitializeOnLoad]
public static class CaptureTools
{
    static bool recording;
    static int frame;
    static double nextFrame;
    static string frameDirectory;
    static CaptureTools() { EditorApplication.update += Tick; }
    static string Media => Path.GetFullPath(Path.Combine(Application.dataPath,"../../Media"));

    [MenuItem("Lumen Run/Capture screenshot")]
    public static void Screenshot()
    {
        if(!EditorApplication.isPlaying) { Debug.LogWarning("Enter Play mode first."); return; }
        Directory.CreateDirectory(Media);
        string state=GameSession.Instance.State.ToString().ToLowerInvariant();
        string path=Path.Combine(Media,state+"-"+DateTime.Now.ToString("yyyyMMdd-HHmmss")+".png");
        ScreenCapture.CaptureScreenshot(path);
        Debug.Log("Screenshot requested: "+path);
    }

    [MenuItem("Lumen Run/Start or stop recording frames")]
    public static void ToggleRecording()
    {
        if(recording) { recording=false; Debug.Log("Stopped. Recorded "+frame+" frames to "+frameDirectory); return; }
        if(!EditorApplication.isPlaying) { Debug.LogWarning("Enter Play mode first."); return; }
        frameDirectory=Path.Combine(Media,"Frames-"+DateTime.Now.ToString("yyyyMMdd-HHmmss"));
        Directory.CreateDirectory(frameDirectory); frame=0; nextFrame=EditorApplication.timeSinceStartup; recording=true;
        Debug.Log("Recording Game view at up to 20 FPS. Use the same menu command to stop.");
    }

    static void Tick()
    {
        if(!recording) return;
        if(!EditorApplication.isPlaying) { recording=false; return; }
        if(EditorApplication.timeSinceStartup<nextFrame) return;
        ScreenCapture.CaptureScreenshot(Path.Combine(frameDirectory,"frame-"+(frame++).ToString("D6")+".png"));
        nextFrame=EditorApplication.timeSinceStartup+.05;
        if(frame>=2400) { recording=false; Debug.Log("Recording stopped at 2400 frames."); }
    }
}
#endif

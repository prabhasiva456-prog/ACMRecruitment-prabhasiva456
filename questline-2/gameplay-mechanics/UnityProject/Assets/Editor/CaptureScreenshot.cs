#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
public static class CaptureScreenshot
{
    [MenuItem("Neon Survival/Capture Gameplay Screenshot")]
    public static void Capture(){string folder=Path.GetFullPath(Path.Combine(Application.dataPath,"../../Media"));Directory.CreateDirectory(folder);string path=Path.Combine(folder,"gameplay.png");ScreenCapture.CaptureScreenshot(path,2);Debug.Log("Screenshot saved to: "+path);}
}
#endif

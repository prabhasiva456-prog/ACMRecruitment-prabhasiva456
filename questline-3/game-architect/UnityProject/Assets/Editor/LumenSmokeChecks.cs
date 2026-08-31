#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class LumenSmokeChecks
{
    const string Pending="LumenRun.SmokePending";
    static bool running;
    static int stage;
    static double next;
    static float pausedTime;
    static string report;

    static LumenSmokeChecks()
    {
        EditorApplication.playModeStateChanged+=OnPlayMode;
        EditorApplication.update+=Tick;
    }

    [MenuItem("Lumen Run/Run smoke checks")]
    public static void Run()
    {
        if(EditorApplication.isPlaying) { Debug.LogWarning("Stop Play mode before running smoke checks."); return; }
        if(!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        EditorSceneManager.OpenScene("Assets/Scenes/Main.unity");
        SessionState.SetBool(Pending,true);
        EditorApplication.EnterPlaymode();
    }

    static void OnPlayMode(PlayModeStateChange change)
    {
        if(change==PlayModeStateChange.EnteredPlayMode && SessionState.GetBool(Pending,false))
        {
            SessionState.SetBool(Pending,false); running=true; stage=0;
            report="Lumen Run runtime smoke checks\n"; next=EditorApplication.timeSinceStartup+.5;
        }
        if(change==PlayModeStateChange.ExitingPlayMode) running=false;
    }

    static void Check(bool condition,string name)
    {
        if(!condition) throw new Exception(name);
        report+="PASS: "+name+"\n";
    }

    static void Tick()
    {
        if(!running || EditorApplication.timeSinceStartup<next) return;
        try
        {
            GameSession s=GameSession.Instance;
            switch(stage)
            {
                case 0:
                    Check(s && s.State==RunState.Menu,"Start menu state");
                    Check(s.Health==5 && s.Score==0 && s.Collected==0,"Initial health, score, and progression");
                    Check(!s.Player.GetComponent<Rigidbody2D>().simulated,"Player frozen at menu");
                    s.StartRun(); break;
                case 1:
                    Check(s.Remaining<120 && s.State==RunState.Playing,"Start button starts timer and gameplay");
                    s.Hurt(s.Player.transform.position+Vector3.left);
                    s.Hurt(s.Player.transform.position+Vector3.left);
                    Check(s.Health==4,"Damage plus invulnerability prevents double damage");
                    s.TogglePause(); pausedTime=s.Remaining; break;
                case 2:
                    Check(s.State==RunState.Paused && Mathf.Abs(s.Remaining-pausedTime)<.01f,"Pause freezes timer");
                    s.TogglePause();
                    s.Player.GetComponent<Rigidbody2D>().position=new Vector2(-12,-2.4f);
                    s.Player.GetComponent<Rigidbody2D>().linearVelocity=Vector2.zero; break;
                case 3:
                    Check(s.Collected==1 && s.Score==100,"Physics trigger collects first core and awards score");
                    s.EnterExit(); Check(s.State==RunState.Playing,"Exit stays locked before collecting all cores");
                    while(s.Collected<GameSession.CrystalGoal) s.Collect();
                    s.EnterExit(); Check(s.State==RunState.Won && s.Score>600,"All cores unlock win and bonuses");
                    s.Restart(); break;
                case 4:
                    Check(s.State==RunState.Menu && s.Health==5 && s.Score==0 && s.Collected==0,"Restart reconstructs a fresh run");
                    Check(UnityEngine.Object.FindObjectsByType<Sentinel>(FindObjectsSortMode.None).Length==3,"Restart restores enemies");
                    s.StartRun(); s.Player.GetComponent<Rigidbody2D>().position=new Vector2(-14,-9); break;
                case 5:
                    Check(s.State==RunState.Lost,"Falling triggers Game Over");
                    Finish(true); return;
            }
            stage++; next=EditorApplication.timeSinceStartup+.6;
        }
        catch(Exception error) { report+="FAIL: "+error+"\n"; Finish(false); }
    }

    static void Finish(bool passed)
    {
        running=false;
        string folder=Path.GetFullPath(Path.Combine(Application.dataPath,"../../Media"));
        Directory.CreateDirectory(folder);
        File.WriteAllText(Path.Combine(folder,"runtime-validation.txt"),report);
        if(passed) Debug.Log(report); else Debug.LogError(report);
        EditorApplication.ExitPlaymode();
    }
}
#endif

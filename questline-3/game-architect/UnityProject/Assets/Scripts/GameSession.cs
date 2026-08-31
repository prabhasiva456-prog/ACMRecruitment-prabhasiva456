using UnityEngine;
using UnityEngine.SceneManagement;

public enum RunState { Menu, Playing, Paused, Won, Lost }

public class GameSession : MonoBehaviour
{
    public static GameSession Instance { get; private set; }
    public RunState State { get; private set; } = RunState.Menu;
    public int Health { get; private set; } = 5;
    public int Score { get; private set; }
    public int Collected { get; private set; }
    public const int CrystalGoal = 6;
    public float Remaining { get; private set; } = 120f;
    public string ResultReason { get; private set; } = "";
    public string Hint { get; private set; } = "Collect all 6 light cores, then reach the exit.";
    public Runner Player { get; private set; }
    public SoundSynth Sound { get; private set; }
    float damageAllowedAt;
    float hintUntil;

    void Awake()
    {
        Instance = this;
        Sound = gameObject.AddComponent<SoundSynth>();
        Player = gameObject.AddComponent<LevelBuilder>().Build();
        gameObject.AddComponent<GameHud>();
        SetSimulation(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)) Sound.ToggleMute();
        if (State == RunState.Menu && Input.GetKeyDown(KeyCode.Return)) StartRun();
        else if ((State == RunState.Won || State == RunState.Lost) && Input.GetKeyDown(KeyCode.R)) Restart();
        else if (Input.GetKeyDown(KeyCode.Escape) && (State == RunState.Playing || State == RunState.Paused)) TogglePause();

        if (State != RunState.Playing) return;
        Remaining = Mathf.Max(0f, Remaining - Time.deltaTime);
        if (Remaining <= 0f) EndRun(false, "The light faded. Time ran out.");
        else if (Player.transform.position.y < -7f) EndRun(false, "You fell beyond the station.");
        if (Time.time > hintUntil) Hint = Collected == CrystalGoal ? "All cores recovered! Reach the portal on the right." : "Collect all 6 light cores, then reach the exit.";
    }

    public void StartRun()
    {
        State = RunState.Playing;
        SetSimulation(true);
        Sound.Play(Cue.Start);
    }

    public void TogglePause()
    {
        State = State == RunState.Playing ? RunState.Paused : RunState.Playing;
        SetSimulation(State == RunState.Playing);
    }

    public void Restart() { SceneManager.LoadScene("Main"); }

    void SetSimulation(bool enabled)
    {
        foreach (Rigidbody2D body in FindObjectsByType<Rigidbody2D>(FindObjectsSortMode.None)) body.simulated = enabled;
    }

    public void Collect()
    {
        if (State != RunState.Playing) return;
        Collected++;
        Score += 100;
        Sound.Play(Cue.Collect);
        ShowHint(Collected == CrystalGoal ? "PORTAL ONLINE — return to the right-hand exit." : "Light core recovered +100");
    }

    public void EnemyDefeated()
    {
        if (State != RunState.Playing) return;
        Score += 150;
        Sound.Play(Cue.Collect);
        ShowHint("Sentinel disabled +150");
    }

    public void Hurt(Vector2 source)
    {
        if (State != RunState.Playing || Time.time < damageAllowedAt) return;
        damageAllowedAt = Time.time + .9f;
        Health--;
        Sound.Play(Cue.Hurt);
        Player.Knockback(source);
        if (Health <= 0) EndRun(false, "Your suit ran out of energy.");
    }

    public void EnterExit()
    {
        if (State != RunState.Playing) return;
        if (Collected < CrystalGoal) { ShowHint("Portal locked: recover " + (CrystalGoal - Collected) + " more cores."); return; }
        Score += Mathf.CeilToInt(Remaining) * 5 + Health * 100;
        EndRun(true, "All six light cores are safe. Station restored!");
    }

    public void ShowHint(string text) { Hint = text; hintUntil = Time.time + 2.5f; }

    void EndRun(bool won, string reason)
    {
        State = won ? RunState.Won : RunState.Lost;
        ResultReason = reason;
        SetSimulation(false);
        Sound.Play(won ? Cue.Win : Cue.Lose);
    }
}

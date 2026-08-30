using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public int Score { get; private set; }
    public bool IsGameOver { get; private set; }
    public float DifficultyMultiplier => 1f + Mathf.Min(Score / 100f, 1.5f);

    Text scoreText, healthText, finalScoreText;
    GameObject gameOverPanel;
    Image healthFill;

    void Awake() { Instance = this; }

    public void BindUI(Text score, Text health, Image fill, GameObject panel, Text finalScore)
    {
        scoreText = score; healthText = health; healthFill = fill;
        gameOverPanel = panel; finalScoreText = finalScore;
        RefreshScore(); SetHealth(5, 5); gameOverPanel.SetActive(false);
    }

    public void AddScore(int amount)
    {
        if (IsGameOver) return;
        Score += amount;
        RefreshScore();
    }

    void RefreshScore() { if (scoreText) scoreText.text = $"SCORE  {Score:0000}"; }

    public void SetHealth(int current, int maximum)
    {
        if (healthText) healthText.text = $"HEALTH  {current} / {maximum}";
        if (healthFill) healthFill.fillAmount = (float)current / maximum;
    }

    public void GameOver()
    {
        if (IsGameOver) return;
        IsGameOver = true;
        if (finalScoreText) finalScoreText.text = $"FINAL SCORE  {Score:0000}";
        if (gameOverPanel) gameOverPanel.SetActive(true);
    }

    public void Restart() { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); }

    void Update()
    {
        if (IsGameOver && Input.GetKeyDown(KeyCode.R)) Restart();
    }
}

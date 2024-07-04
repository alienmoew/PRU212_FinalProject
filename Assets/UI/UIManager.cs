using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public TMP_Text scoreText;

    public Image progressBar;  // Reference to the UI Image component
    public Sprite[] progressSprites;  // Array of sprites representing different progress stages

    private int maxScore = 100;  // Maximum score for full progress (sprite 5)

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = "" + score;
        }

        UpdateProgressBar(score);
    }

    private void UpdateProgressBar(int score)
    {
        // Calculate which sprite to display based on the current score
        int spriteIndex = Mathf.FloorToInt((float)score / maxScore * (progressSprites.Length - 1));

        // Set the sprite of the progress bar
        if (spriteIndex >= 0 && spriteIndex < progressSprites.Length)
        {
            progressBar.sprite = progressSprites[spriteIndex];
        }
    }
}

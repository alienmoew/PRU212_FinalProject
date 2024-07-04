using UnityEngine;
using UnityEngine.UI;

public class ProgressBarController : MonoBehaviour
{
    public Image progressBar;  // Reference to the UI Image component
    public Sprite[] progressSprites;  // Array of sprites representing different progress stages

    private int maxScore = 100;  // Maximum score for full progress (sprite 5)
    private int currentScore = 50;  // Current score

    void Start()
    {
        UpdateProgress();
    }

    public void IncreaseScore(int amount)
    {
        currentScore += amount;
        currentScore = Mathf.Clamp(currentScore, 0, maxScore);  // Clamp score to be within 0 to maxScore
        UpdateProgress();
    }

    private void UpdateProgress()
    {
        // Calculate which sprite to display based on the current score
        int spriteIndex = Mathf.FloorToInt((float)currentScore / maxScore * (progressSprites.Length - 1));

        // Set the sprite of the progress bar
        if (spriteIndex >= 0 && spriteIndex < progressSprites.Length)
        {
            progressBar.sprite = progressSprites[spriteIndex];
        }
    }
}

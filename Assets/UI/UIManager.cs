using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public TMP_Text scoreText;
    public GameObject deathPanel; // Reference to the death panel GameObject
    public GameObject winPanel;   // Reference to the win panel GameObject

    public Image progressBar;  // Reference to the UI Image component
    public Sprite[] progressSprites;  // Array of sprites representing different progress stages

    private int[] scoreMilestones = { 0, 50, 100, 150, 300 };  // Score milestones

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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "ConnectToServer")
        {
            ResetGameData();
        }
    }

    private void ResetGameData()
    {
        // Reset UI elements
        if (scoreText != null)
        {
            scoreText.text = "0";
        }

        if (deathPanel != null)
        {
            deathPanel.SetActive(false);
        }

        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }

        // Reset progress bar if needed
        UpdateProgressBar(0);
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
        // Determine which milestone the current score is at or above
        int milestoneIndex = 0;
        for (int i = 0; i < scoreMilestones.Length; i++)
        {
            if (score >= scoreMilestones[i])
            {
                milestoneIndex = i;
            }
            else
            {
                break;
            }
        }

        // Set the sprite of the progress bar based on the milestone index
        if (milestoneIndex >= 0 && milestoneIndex < progressSprites.Length)
        {
            progressBar.sprite = progressSprites[milestoneIndex];
        }
    }

    // Method to show the death panel
    public void ShowDeathPanel()
    {
        PhotonNetwork.Disconnect();
        if (deathPanel != null)
        {
            deathPanel.SetActive(true);
        }
    }

    // Method to show the win panel
    public void ShowWinPanel()
    {
        PhotonNetwork.Disconnect();
        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }
    }

    // Method to handle the "Return to Lobby" button click
    public void ReturnToLobby()
    {
        SceneManager.LoadScene("ConnectToServer");
    }
}

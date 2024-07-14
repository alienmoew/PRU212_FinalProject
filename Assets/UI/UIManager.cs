using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public TMP_Text scoreText;
    public GameObject deathPanel; 
    public GameObject winPanel;   
    public GameObject menuPanel;

    public Image progressBar;  
    public Sprite[] progressSprites;  

    public TMP_Text redzoneTimerText; 

    private int[] scoreMilestones = { 0, 50, 100, 150, 300 };

    public TMP_Text remainingPlayersText;


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

        UpdateProgressBar(0);

        if (redzoneTimerText != null)
        {
            redzoneTimerText.text = "00:00";
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

        if (milestoneIndex >= 0 && milestoneIndex < progressSprites.Length)
        {
            progressBar.sprite = progressSprites[milestoneIndex];
        }
    }

    public void ShowDeathPanel()
    {
        PhotonNetwork.Disconnect();
        if (deathPanel != null)
        {
            deathPanel.SetActive(true);
        }
    }

    public void ShowWinPanel()
    {
        PhotonNetwork.Disconnect();
        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }
    }

    public void ShowMenuPanel()
    {
        if(menuPanel != null)
        {
            menuPanel.SetActive(true);
        }
    }

    public void ReturnToLobby()
    {
        SceneManager.LoadScene("ConnectToServer");
    }

    public void UpdateRedzoneTimer(float timeRemaining)
    {
        if (redzoneTimerText != null)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60F);
            int seconds = Mathf.FloorToInt(timeRemaining % 60F);
            redzoneTimerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    public void Resume()
    {
        if (menuPanel != null)
        {
            menuPanel.SetActive(false);
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void Home()
    {
        PhotonNetwork.Disconnect();
        ReturnToLobby();
    }

    public void UpdateRemainingPlayers(int remainingPlayers)
    {
        if (remainingPlayersText != null)
        {
            remainingPlayersText.text = "Alive: " + remainingPlayers;
        }
    }

}

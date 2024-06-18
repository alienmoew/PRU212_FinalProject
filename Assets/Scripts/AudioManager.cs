using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioSource audioSource;
    public AudioClip backgroundMusic1; // Music for ConnectToServer and Lobby
    public AudioClip backgroundMusic2; // Music for Game

    private void Awake()
    {
        // Singleton pattern to ensure only one instance of AudioManager exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Prevents this object from being destroyed when loading a new scene
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        PlayBackgroundMusic1();
    }

    void Update()
    {
        if (audioSource == null) // Check if audioSource is null
        {
            Debug.LogWarning("AudioSource is null. This might be caused by the object being destroyed.");
            return;
        }

        string currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName == "Game" && audioSource.clip != backgroundMusic2)
        {
            PlayBackgroundMusic2();
        }
        else if ((currentSceneName == "ConnectToServer" || currentSceneName == "Lobby") && audioSource.clip != backgroundMusic1)
        {
            PlayBackgroundMusic1();
        }
    }

    void PlayBackgroundMusic1()
    {
        if (audioSource != null && audioSource.clip != backgroundMusic1) // Check if audioSource is not null
        {
            audioSource.clip = backgroundMusic1;
            audioSource.Play();
        }
    }

    void PlayBackgroundMusic2()
    {
        if (audioSource != null && audioSource.clip != backgroundMusic2) // Check if audioSource is not null
        {
            audioSource.clip = backgroundMusic2;
            audioSource.Play();
        }
    }
}

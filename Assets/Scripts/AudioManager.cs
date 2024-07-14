using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioSource audioSource;
    public AudioClip backgroundMusic1;
    public AudioClip backgroundMusic2; 

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
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
        if (audioSource == null)
        {
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
        if (audioSource != null && audioSource.clip != backgroundMusic1)
        {
            audioSource.clip = backgroundMusic1;
            audioSource.Play();
        }
    }

    void PlayBackgroundMusic2()
    {
        if (audioSource != null && audioSource.clip != backgroundMusic2)
        {
            audioSource.clip = backgroundMusic2;
            audioSource.loop = true;
            audioSource.Play();
        }
    }
}

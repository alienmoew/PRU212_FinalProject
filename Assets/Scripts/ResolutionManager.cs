using UnityEngine;

public class ResolutionManager : MonoBehaviour
{
    private static ResolutionManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Don't destroy this object when loading a new scene
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }

    private void Start()
    {
        // Set initial resolution and mode
        SetWindowedMode();
    }

    public void SetFullScreenMode()
    {
        Screen.SetResolution(1920, 1080, FullScreenMode.ExclusiveFullScreen);
    }

    public void SetWindowedMode()
    {
        Screen.SetResolution(1920, 1080, FullScreenMode.Windowed);
    }

    private void Update()
    {
        // Check for user input to toggle modes
        if (Input.GetKeyDown(KeyCode.F11))
        {
            if (Screen.fullScreenMode == FullScreenMode.ExclusiveFullScreen)
            {
                SetWindowedMode();
            }
            else
            {
                SetFullScreenMode();
            }
        }
    }
}

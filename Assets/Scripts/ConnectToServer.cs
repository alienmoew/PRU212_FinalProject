using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using UnityEngine.UI;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    public InputField usernameInput;
    public Text buttonText;

    private bool isConnecting = false; // Track connection state

    private void Start()
    {
        // Ensure the UIManager is destroyed when transitioning to ConnectToServer scene
        if (UIManager.Instance != null)
        {
            Destroy(UIManager.Instance.gameObject);
        }
    }

    public void OnClickConnect()
    {
        if (!isConnecting && usernameInput.text.Length >= 1)
        {
            isConnecting = true;
            PhotonNetwork.NickName = usernameInput.text;
            buttonText.text = "Connecting...";
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        SceneManager.LoadScene("Lobby");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        isConnecting = false;
        buttonText.text = "Connect";
    }
}

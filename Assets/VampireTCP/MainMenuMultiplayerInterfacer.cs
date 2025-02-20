using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MainMenuMultiplayerInterfacer : MonoBehaviour
{
    private MultiplayerManager multiplayerManager;
    public TMP_InputField roomCodeInput;
    public Button joinButton;
    public bool publicRoom;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        multiplayerManager = Object.FindObjectsByType<MultiplayerManager>(FindObjectsSortMode.None)[0];
    }

    public void HostGame()
    {
        multiplayerManager.HostGame();
    }

    public void JoinGame()
    {
        multiplayerManager.JoinGame(roomCodeInput.text);
    }
    public void JoinRandomGame()
    {
        //multiplayerManager.JoinGame();
    }
    public void TogglePublic()
    {
        publicRoom = !publicRoom;
    }
    public void OnCodeInputChange()
    {
        if (roomCodeInput.text.Length == 6)
        {
            joinButton.interactable = true;
        }
        else
        {
            joinButton.interactable = false;
        }
    }

    public void ExitGame()
    {
        multiplayerManager.ExitGame();
    }
}

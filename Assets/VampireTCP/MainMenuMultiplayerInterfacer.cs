using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuMultiplayerInterfacer : MonoBehaviour
{
    private MultiplayerManager multiplayerManager;
    public TMP_InputField roomCodeInput;

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

    public void ExitGame()
    {
        multiplayerManager.ExitGame();
    }
}

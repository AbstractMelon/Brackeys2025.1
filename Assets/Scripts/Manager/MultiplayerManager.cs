using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MultiplayerManager : MonoBehaviour
{
    [SerializeField] private VampireTCP networkManager;
    private TMP_InputField roomCodeInput;

    private void Start()
    {
        roomCodeInput = Object.FindObjectsByType<TMP_InputField>(FindObjectsSortMode.None)[0];
        Object.FindObjectsByType<Button>(FindObjectsSortMode.None)[0].onClick.AddListener(HostGame);
        roomCodeInput.text = "Room Code";
    }

    public void HostGame()
    {
        networkManager.CreateRoom(true);
        SceneManager.LoadScene("Lobby");
    }

    public void JoinGame()
    {
        networkManager.JoinRoom(roomCodeInput.text);
        SceneManager.LoadScene("Lobby");
    }

    public void ExitGame()
    {
        UnityEngine.Application.Quit();
    }
}

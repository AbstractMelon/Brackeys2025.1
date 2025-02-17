using TMPro;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private VampireTCP networkManager;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private TMP_InputField roomCodeInput;

    private void Start()
    {
        roomCodeInput.text = "Room Code";
    }

    public void HostGame()
    {
        networkManager.CreateRoom(true);
        mainMenuPanel.SetActive(false);
    }

    public void JoinGame()
    {
        networkManager.JoinRoom(roomCodeInput.text);
        mainMenuPanel.SetActive(false);
    }

    public void ExitGame()
    {
        UnityEngine.Application.Quit();
    }
}

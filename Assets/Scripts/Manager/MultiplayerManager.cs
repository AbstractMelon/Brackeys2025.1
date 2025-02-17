using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MultiplayerManager : MonoBehaviour
{
    [SerializeField] private VampireTCP networkManager;

    public void HostGame()
    {
        networkManager.CreateRoom(true);
        SceneManager.LoadScene("Game");
    }

    public void JoinGame(string code)
    {
        networkManager.JoinRoom(code);
        SceneManager.LoadScene("Game");
    }

    public void ExitGame()
    {
        UnityEngine.Application.Quit();
    }
}

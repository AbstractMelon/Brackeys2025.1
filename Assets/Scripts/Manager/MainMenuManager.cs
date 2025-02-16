using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private NetworkManager networkManager;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private InputField ipAddressInput;

    private void Start()
    {
        // Auto-fill local IP
        ipAddressInput.text = "localhost";
    }

    public void HostGame()
    {
        networkManager.StartHost();
        mainMenuPanel.SetActive(false);
    }

    public void JoinGame()
    {
        networkManager.networkAddress = ipAddressInput.text;
        networkManager.StartClient();
        mainMenuPanel.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
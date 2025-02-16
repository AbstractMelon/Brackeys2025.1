using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System.Linq;

public class LobbyUI : MonoBehaviour
{
    public static LobbyUI Instance;
    
    [SerializeField] private Transform playerListContent;
    [SerializeField] private GameObject playerListItemPrefab;
    [SerializeField] private Button startGameButton;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdatePlayerList()
    {
        // Clear existing items
        foreach (Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }

        // Create new items
        foreach (LobbyPlayer player in FindObjectsOfType<LobbyPlayer>().OrderBy(p => p.index))
        {
            GameObject item = Instantiate(playerListItemPrefab, playerListContent);
            // item.GetComponent<LobbyPlayerListItem>().Setup(player);
        }

        // Show start button only for host
        startGameButton.gameObject.SetActive(NetworkServer.active && NetworkClient.active);
    }

    public void OnReadyClicked()
    {
        NetworkClient.localPlayer.GetComponent<LobbyPlayer>().CmdToggleReady();
    }

    public void OnStartGameClicked()
    {
        NetworkManager.singleton.ServerChangeScene("Game");
    }
}
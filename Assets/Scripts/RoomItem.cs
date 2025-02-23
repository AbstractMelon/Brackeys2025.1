using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomListItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI roomCodeText;
    [SerializeField] private Button joinButton;

    private string roomCode;
    private MultiplayerManager multiplayerManager;

    public void Initialize(string code, MultiplayerManager manager)
    {
        roomCode = code;
        multiplayerManager = manager;
        roomCodeText.text = $"Room: {code}";
        joinButton.onClick.AddListener(OnJoinClicked);

        Debug.Log("Initialized room list item with code: " + code);
    }

    private void OnJoinClicked()
    {
        multiplayerManager = MultiplayerManager.instance;
        Debug.Log("Join button clicked for room code: " + roomCode);
        multiplayerManager.JoinGame(roomCode);
    }
}


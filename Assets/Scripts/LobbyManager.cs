using UnityEngine;
using TMPro;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerCountText;
    [SerializeField] private TextMeshProUGUI roomCodeText;

    public static LobbyManager instance;
    public int alivePlayers { get; private set; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        UpdatePlayerCount(1);
        UpdateRoomCode(string.Empty);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Update the GUI with the current player count
    public void UpdatePlayerCount(int count)
    {
        playerCountText.text = $"Players: {count}";
    }

    // Update the GUI with the room code
    public void UpdateRoomCode(string roomCode)
    {
        roomCodeText.text = $"Room Code: {roomCode}";
    }
    public void PlayerDied()
    {
        alivePlayers--;
    }
}
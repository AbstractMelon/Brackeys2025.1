using UnityEngine;

public class MultiplayerUIManager : MonoBehaviour
{
    public GameObject GoTime;
    public LobbyManager lobbyManager;

    public void DisplayGoTime(bool v)
    {
        GoTime.SetActive(v);
    }

    public void SetPlayerCount(int c)
    {
        lobbyManager.UpdatePlayerCount(c);
    }
}

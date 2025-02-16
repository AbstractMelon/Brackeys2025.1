using UnityEngine;
using Mirror;

public class LobbyPlayer : NetworkRoomPlayer
{
    [SyncVar(hook = nameof(HandleReadyStateChanged))] 
    public bool IsReady;

    public override void OnStartClient()
    {
        base.OnStartClient();
        LobbyUI.Instance.UpdatePlayerList();
    }

    [Command]
    public void CmdToggleReady()
    {
        IsReady = !IsReady;
        CheckAllPlayersReady();
    }

    private void CheckAllPlayersReady()
    {
        if (isServer)
        {
            bool allReady = true;
            foreach (LobbyPlayer player in FindObjectsOfType<LobbyPlayer>())
            {
                if (!player.IsReady) allReady = false;
            }
            
            if (allReady) NetworkManager.singleton.ServerChangeScene("Game");
        }
    }

    private void HandleReadyStateChanged(bool oldValue, bool newValue)
    {
        LobbyUI.Instance.UpdatePlayerList();
    }
}
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using System.Collections.Generic;
using Newtonsoft.Json;

public class MainMenuMultiplayerInterfacer : MonoBehaviour
{
    private MultiplayerManager multiplayerManager;
    public TMP_InputField roomCodeInput;
    public Button joinButton;
    public bool publicRoom;
    public bool waitingForRooms;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        multiplayerManager = Object.FindObjectsByType<MultiplayerManager>(FindObjectsSortMode.None)[0];
    }

    public void HostGame()
    {
        multiplayerManager.HostGame(publicRoom);
    }

    public void JoinGame()
    {
        multiplayerManager.JoinGame(roomCodeInput.text.ToUpper());
    }
    public void JoinRandomGame()
    {
        waitingForRooms = true;
        FindObjectsByType<VampireTCP>(FindObjectsSortMode.None)[0].RequestRoomsList();
    }
    public void GetMessageForRooms(GenericMessageWrapper message)
    {
        if (waitingForRooms)
        {
            string[] rooms = message.msg.msg;
            if (rooms == null) 
            {
                waitingForRooms = false;
                return;
            }
            List<string> publicRooms = new List<string>();
            for (int i = 0; i < rooms.Length; i++)
            {
                if (rooms[i].EndsWith("PUBLIC"))
                {
                    publicRooms.Add(rooms[i]);
                }
            }
            if (publicRooms.Count != 0)
            {
                multiplayerManager.JoinGame(publicRooms[Random.Range(0, publicRooms.Count)]);
            }
            waitingForRooms = false;
        }
    }
    public void TogglePublic()
    {
        publicRoom = !publicRoom;
    }
    public void OnCodeInputChange()
    {
        if (roomCodeInput.text.Length == 6 || (roomCodeInput.text.Length == 12 && roomCodeInput.text.EndsWith("PUBLIC")))
        {
            joinButton.interactable = true;
        }
        else
        {
            joinButton.interactable = false;
        }
    }

    public void ExitGame()
    {
        multiplayerManager.ExitGame();
    }
}

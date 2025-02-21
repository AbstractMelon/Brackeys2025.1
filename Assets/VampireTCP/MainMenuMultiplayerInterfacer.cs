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
            if (rooms.Length != 0)
            {
                multiplayerManager.JoinGame(rooms[Random.Range(0, rooms.Length)]);
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
        if (roomCodeInput.text.Length == 6)
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


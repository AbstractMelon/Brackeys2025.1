using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RoomFinder : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform roomListContent;
    [SerializeField] private GameObject roomListItemPrefab;
    [SerializeField] private Button refreshButton;
    [SerializeField] private GameObject noRoomsMessage;

    private VampireTCP networkManager;
    private MultiplayerManager multiplayerManager;

    private List<GameObject> currentRoomItems = new List<GameObject>();

    private void Awake()
    {
        networkManager = FindObjectOfType<VampireTCP>();
        multiplayerManager = MultiplayerManager.instance;
        
        refreshButton.onClick.AddListener(RefreshRoomList);
    }

    private void OnEnable()
    {
        networkManager.onRecieveNewBaseMessage.AddListener(HandleRoomListUpdate);
        RefreshRoomList();
    }

    private void OnDisable()
    {
        networkManager.onRecieveNewBaseMessage.RemoveListener(HandleRoomListUpdate);
    }

    private void HandleRoomListUpdate(GenericMessageWrapper message)
    {
        if (message.msg != null && message.msg.msg != null)
        {
            UpdateRoomList(message.msg.msg);
        }
    }

    public void RefreshRoomList()
    {
        ClearRoomList();
        networkManager.RequestRoomsList();
    }

    private void UpdateRoomList(string[] roomCodes)
    {
        ClearRoomList();

        noRoomsMessage.SetActive(roomCodes.Length == 0);

        foreach (string roomCode in roomCodes)
        {
            GameObject roomItem = Instantiate(roomListItemPrefab, roomListContent);
            roomItem.GetComponent<RoomListItem>().Initialize(roomCode, multiplayerManager);
            currentRoomItems.Add(roomItem);
        }
    }

    private void ClearRoomList()
    {
        foreach (GameObject item in currentRoomItems)
        {
            Destroy(item);
        }
        currentRoomItems.Clear();
    }
}
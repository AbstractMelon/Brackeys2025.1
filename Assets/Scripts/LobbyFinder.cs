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
        Debug.Log("Awake called on RoomFinder.");
        multiplayerManager = MultiplayerManager.instance;
    }

    private void HandleRoomListUpdate(GenericMessageWrapper message)
    {
        if (message != null && message.msg != null && message.msg.msg != null)
        {
            Debug.Log("Received room list message.");
            UpdateRoomList(message.msg.msg);
        }
        else
        {
            Debug.LogError("HandleRoomListUpdate() received null or empty message.");
        }
    }

    public void RefreshRoomList()
    {
        Debug.Log("Refreshing room list.");
        ClearRoomList();
        if (networkManager != null)
        {
            Debug.Log("Requesting room list from network manager.");
            networkManager.RequestRoomsList();
        }
        else
        {
            Debug.LogError("No network manager found to request rooms list.");
        }
    }

    private void UpdateRoomList(string[] roomCodes)
    {
        Debug.Log($"Updating UI with {(roomCodes?.Length ?? 0)} rooms.");
        ClearRoomList();

        if (noRoomsMessage != null)
        {
            noRoomsMessage.SetActive(roomCodes == null || roomCodes.Length == 0);
            if (roomCodes == null)
            {
                noRoomsMessage.GetComponentInChildren<Text>().text = "No rooms found.";
            }
        }

        if (roomCodes == null) return;

        foreach (string roomCode in roomCodes)
        {
            if (roomListItemPrefab != null)
            {
                GameObject roomItem = Instantiate(roomListItemPrefab, roomListContent);
                RoomListItem itemComponent = roomItem.GetComponent<RoomListItem>();
                if (itemComponent != null)
                {
                    Debug.Log($"Initializing room list item for room code: {roomCode}");
                    itemComponent.Initialize(roomCode, multiplayerManager);
                    currentRoomItems.Add(roomItem);
                }
                else
                {
                    Debug.LogError("RoomListItem component missing on prefab.");
                    Destroy(roomItem);
                }
            }
            else
            {
                Debug.LogError("Room list item prefab is missing.");
            }
        }
    }

    private void ClearRoomList()
    {
        Debug.Log("Clearing room list.");
        foreach (GameObject item in currentRoomItems)
        {
            Destroy(item);
        }
        currentRoomItems.Clear();
    }

    public void SetNetworkManager()
    {
        Debug.Log("Setting network manager.");
        networkManager = FindFirstObjectByType<VampireTCP>();
        if (networkManager != null)
        {
            Debug.Log("Adding listener to network manager.");
        }
        else
        {
            Debug.LogError("VampireTCP network manager not found.");
        }
    }
}


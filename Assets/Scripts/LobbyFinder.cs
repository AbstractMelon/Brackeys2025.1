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
        networkManager = Object.FindFirstObjectByType<VampireTCP>();
        multiplayerManager = MultiplayerManager.instance;
        
        refreshButton.onClick.AddListener(RefreshRoomList);
    }

    private void OnEnable()
    {
        if (networkManager != null)
        {
            networkManager.onRecieveNewBaseMessage.AddListener(HandleRoomListUpdate);
        }
        RefreshRoomList();
    }

    private void OnDisable()
    {
        if (networkManager != null)
        {
            networkManager.onRecieveNewBaseMessage.RemoveListener(HandleRoomListUpdate);
        }
    }


    private void HandleRoomListUpdate(GenericMessageWrapper message)
    {
        if (message != null && message.msg != null && message.msg.msg != null)
        {
            UpdateRoomList(message.msg.msg);
        }
    }

    public void RefreshRoomList()
    {
        ClearRoomList();
        if (networkManager != null)
        {
            networkManager.RequestRoomsList();
        }
    }

    private void UpdateRoomList(string[] roomCodes)
    {
        Debug.Log($"Updating UI with {(roomCodes?.Length ?? 0)} rooms.");
        ClearRoomList();

        if (noRoomsMessage != null)
        {
            noRoomsMessage.SetActive(roomCodes == null || roomCodes.Length == 0);
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
        foreach (GameObject item in currentRoomItems)
        {
            Destroy(item);
        }
        currentRoomItems.Clear();
    }
}


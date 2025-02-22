using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private MultiplayerManager multiplayerManager;
    private VampireTCP networkManager;
    public GameObject newDemonInstance;
    public static GameManager instance;
    private static bool decideDemonOnLoad;
    private float timeSinceStart;
    private int demonId;
    private bool demonSet;

    void Awake()
    {
        instance = this;
        multiplayerManager = FindObjectsByType<MultiplayerManager>(FindObjectsSortMode.None)[0];
        networkManager = FindObjectsByType<VampireTCP>(FindObjectsSortMode.None)[0];

        networkManager.onRecieveNewMessage.AddListener(CheckUpdateDemon);
        if (decideDemonOnLoad) DecideDemon();
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceStart += Time.deltaTime;
        if (timeSinceStart >= 1 && !demonSet)
        {
            SetDemonToID(demonId);
        }
    }
    public static void DoDecideDemonOnLoad()
    {
        decideDemonOnLoad = true;
    }
    public void CheckUpdateDemon(MessageWrapper newMessage)
    {
        if (newMessage.msg.message == "demonID")
        {
            if (timeSinceStart >= 1)
                SetDemonToID(JsonConvert.DeserializeObject<DemonData>(newMessage.msg.value.ToString()).id);
            else
            {
                demonId = JsonConvert.DeserializeObject<DemonData>(newMessage.msg.value.ToString()).id;
            }
        }
    }
    public void DecideDemon()
    {
        OtherPlayerController[] players = FindObjectsByType<OtherPlayerController>(FindObjectsSortMode.None);
        int[] ids = new int[players.Length + 1];
        for (int i = 0; i < players.Length; i++)
        {
            ids[i] = int.Parse(players[i].gameObject.name.Substring(6));
            Debug.Log("Possible demon: " + ids[i]);
        }
        ids[ids.Length - 1] = networkManager.clientId;
        // Set current ID to last index
        int demonId = ids[UnityEngine.Random.Range(0, ids.Length)];
        networkManager.BroadcastNewMessage("demonID", new { 
            id = demonId
        });
        Debug.Log("The demon is: " + demonId);
        this.demonId = demonId;
        demonSet = false;
    }
    public void SetDemonToID(int id)
    {
        demonSet = true;
        Debug.Log("Setting demon to: " + id);
        if (id == networkManager.clientId)
        {
            FindFirstObjectByType<PlayerController>().gameObject.SetActive(false);
            FindFirstObjectByType<DemonController>(FindObjectsInactive.Include).gameObject.SetActive(true);
        }
        else
        {
            Destroy(GameObject.Find("Player" + id));
            GameObject newDemon = Instantiate(newDemonInstance);
            newDemon.name = "Player" + id;
        }
    }

    // Check if the game is over
    public void CheckGameOver()
    {
        Debug.Log(multiplayerManager.numPlayers);
        if (multiplayerManager.numPlayers <= 0)
        {
            EndGame();
        }
    }

    // Ends the game and declares the winner
    public void EndGame()
    {
        string winner = (multiplayerManager.numPlayers >= 1) ? "Survivors" : "Demon";
        Debug.Log($"{winner} wins!");
    }
}


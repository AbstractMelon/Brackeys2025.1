using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public class DemonData
    {
        public int id { get; set; }
    }
    public class HurtData
    {
        public int id { get; set; }
        public int d { get; set; }
    }
    private MultiplayerManager multiplayerManager;
    private VampireTCP networkManager;
    public GameObject newDemonInstance;
    public static GameManager instance;
    private float timeSinceStart;
    private int demonId;
    private bool demonSet;
    private bool gameOver;

    public EndScreen endScreenManager;

    void Awake()
    {
        instance = this;
        multiplayerManager = FindObjectsByType<MultiplayerManager>(FindObjectsSortMode.None)[0];
        networkManager = FindObjectsByType<VampireTCP>(FindObjectsSortMode.None)[0];

        networkManager.onRecieveNewMessage.AddListener(GetBroadcast);
    }

    // Update is called once per frame
    void Update()
    {
        CheckGameOver();
        timeSinceStart += Time.deltaTime;
        if (timeSinceStart >= 2 && !demonSet)
        {
            SetDemonToID(demonId);
        }
    }
    public void GetBroadcast(MessageWrapper newMessage)
    {
        if (newMessage.msg.message == "demonId")
        {
            // Not going through?
            Debug.Log("Recieved demon as: " + JsonConvert.DeserializeObject<DemonData>(newMessage.msg.value.ToString()).id);
            if (timeSinceStart >= 1)
                SetDemonToID(JsonConvert.DeserializeObject<DemonData>(newMessage.msg.value.ToString()).id);
            else
            {
                demonId = JsonConvert.DeserializeObject<DemonData>(newMessage.msg.value.ToString()).id;
            }
        }
        else if (newMessage.msg.message == "hurtPlayer")
        {
            HurtData data = JsonConvert.DeserializeObject<HurtData>(newMessage.msg.value.ToString());
            if (data.id == networkManager.clientId)
            {
                GameObject.Find("Player").GetComponent<HealthSystem>().TakeDamage(data.d);
            }
            else
            {
                GameObject.Find("Player" + data.id).GetComponent<HealthSystem>().TakeDamage(data.d);
            }
        }
        else if (newMessage.msg.message == "demonDied")
        {
            EndGame(false);
        }
    }
    public IEnumerator DecideDemon()
    {
        yield return new WaitForSeconds(1f);
        VampireTCP networkManager = FindObjectsByType<VampireTCP>(FindObjectsSortMode.None)[0];
        OtherPlayerController[] players = FindObjectsByType<OtherPlayerController>(FindObjectsSortMode.None);
        int[] ids = new int[players.Length + 1];
        for (int i = 0; i < players.Length; i++)
        {
            ids[i] = int.Parse(players[i].gameObject.name.Substring(6));
            Debug.Log("Possible demon: " + ids[i]);
        }
        ids[ids.Length - 1] = networkManager.clientId;
        // Set current ID to last index
        int demonId2 = ids[Random.Range(0, ids.Length)];
        // Not going through?
        networkManager.BroadcastNewMessage("demonId", new { 
            id = demonId2
        });
        Debug.Log("The demon is: " + demonId2);
        demonId = demonId2;
        demonSet = false;
    }
    public void SetDemonToID(int id)
    {
        demonSet = true;
        demonId = id;
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
        if (gameOver) return;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length == 0) EndGame(true);
    }

    // Ends the game and declares the winner
    public void EndGame(bool demonWins)
    {
        if (gameOver) return;
        gameOver = true;
        endScreenManager.ShowEndScreen(demonWins);
        string winner = !demonWins ? "Survivors" : "Demon";
        Debug.Log($"{winner} wins!");
    }
}


using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System;
using Mono.Cecil.Cil;

public class PositionData
{
    public string t { get; set; }
    public string r { get; set; }
    public string s { get; set; }
}
public class DemonData
{
    public int id { get; set; }
}

public class MultiplayerManager : MonoBehaviour
{
    [SerializeField] private VampireTCP networkManager;

    public GameObject newPlayerInstance;

    public int numPlayers;

    public MultiplayerUIManager multiplayerUIManager;
    private bool startable = false;

    public static MultiplayerManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void StartGame()
    {
        if (numPlayers >= 2)
        {
            networkManager.BroadcastNewMessage("startGame", new { });
            startable = true;
            OtherPlayerController[] players = FindObjectsByType<OtherPlayerController>(FindObjectsSortMode.None);
            int[] ids = new int[players.Length + 1];
            for (int i = 0; i < players.Length; i++)
            {
                ids[i] = int.Parse(players[i].gameObject.name.Substring(6));
            }
            ids[ids.Length - 1] = networkManager.localID;
            // Set current ID to last index
            int demonID = ids[UnityEngine.Random.Range(0, ids.Length)];
            networkManager.BroadcastNewMessage("demonID", new { 
                id = demonID
            });
            Debug.Log("The demon is: " + demonID);
            SceneManager.LoadScene("Game");
            return;
        }
        Debug.Log("Unable to start game, not enough players");
    }
    private void Update()
    {
        numPlayers = GameObject.FindGameObjectsWithTag("Player").Length;
        if (multiplayerUIManager)
        {
            if(Input.GetKeyDown(KeyCode.B))
            {
                StartGame();
            }
            multiplayerUIManager.SetPlayerCount(numPlayers);
        }
    }
    public void GetMessage(GenericMessageWrapper message)
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            FindObjectsByType<MainMenuMultiplayerInterfacer>(FindObjectsSortMode.None)[0].GetMessageForRooms(message);
        }
    }

    public void HostGame(bool isPublic)
    {
        networkManager.CreateRoom(isPublic);
        SceneManager.LoadScene("Lobby");
    }

    public void JoinGame(string code)
    {
        networkManager.JoinRoom(code);
        SceneManager.LoadScene("Lobby");
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Lobby")
        {
            multiplayerUIManager = FindObjectsByType<MultiplayerUIManager>(FindObjectsSortMode.None)[0];
            startable = false;
        } else if (scene.name == "MainMenu")
        {
            networkManager.Reconnect();
        } else if (scene.name.Contains("Game"))
        {

        }
    }

    public void ExitGame()
    {
        UnityEngine.Application.Quit();
    }

    Vector3 StringToVector3(string input)
    {
        input = input.Trim('(', ')'); // Remove parentheses
        string[] values = input.Split(',');

        if (values.Length == 3 &&
            float.TryParse(values[0], out float x) &&
            float.TryParse(values[1], out float y) &&
            float.TryParse(values[2], out float z))
        {
            return new Vector3(x, y, z);
        }

        Debug.LogError("Invalid format: " + input);
        return Vector3.zero;
    }

    public void OnRecieveNewMessage(MessageWrapper newMessage)
    {
        if(newMessage.err.message != null)
        {
            Debug.LogError(newMessage.err.message);
        } else if(GameObject.Find("Player" + newMessage.msg.from))
        {
            if (newMessage.msg.message == "updatePlayerPosition")
            {
                GameObject otherPlayer = GameObject.Find("Player" + newMessage.msg.from);
                PositionData posData = JsonConvert.DeserializeObject<PositionData>(newMessage.msg.value.ToString());
                Vector3 targetPosition = StringToVector3(posData.t);
                Vector3 targetRotation = StringToVector3(posData.r);
                Vector3 targetScale = StringToVector3(posData.s);
                StartCoroutine(LerpPosition(otherPlayer.transform, targetPosition, targetRotation, targetScale, 0.1f));
            } else if (newMessage.msg.message == "startGame" && !startable)
            {
                startable = true;
                SceneManager.LoadScene("Game");
            } else if (newMessage.msg.message == "demonID")
            {
                SetDemonToID(JsonConvert.DeserializeObject<DemonData>(newMessage.msg.value.ToString()).id);
            }
        } else
        {
            GameObject newPlayer = Instantiate(newPlayerInstance);
            newPlayer.name = "Player" + newMessage.msg.from;
        }
    }
    public void SetDemonToID(int id)
    {
        Debug.Log("Recieved demon as: " + id);
    }

    public void OnRecieveNewAudioMessage(AudioMessageWrapper newAudioMessage)
    {
        if (GameObject.Find("Player" + newAudioMessage.msg.from))
        {
            OtherPlayerController otherPlayerScript = GameObject.Find("Player" + newAudioMessage.msg.from).GetComponent<OtherPlayerController>();
            otherPlayerScript.PlayAudio(newAudioMessage.msg.audio);
        }
    }

    IEnumerator LerpPosition(Transform playerTransform, Vector3 targetPosition, Vector3 targetRotation, Vector3 targetScale, float duration)
    {
        float elapsedTime = 0f;
        Vector3 startPosition = playerTransform.position;
        Vector3 startRotation = playerTransform.rotation.eulerAngles;
        Vector3 startScale = playerTransform.localScale;

        while (elapsedTime < duration)
        {
            if (playerTransform == null) yield break;
            playerTransform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            playerTransform.eulerAngles = Vector3.Lerp(startRotation, targetRotation, elapsedTime / duration);
            playerTransform.localScale = Vector3.Lerp(startScale, targetScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        playerTransform.position = targetPosition;
        playerTransform.eulerAngles = targetRotation;
    }

}


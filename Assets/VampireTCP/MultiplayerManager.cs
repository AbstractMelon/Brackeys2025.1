using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System;

public class PositionData
{
    public string t { get; set; }
}

public class MultiplayerManager : MonoBehaviour
{
    [SerializeField] private VampireTCP networkManager;

    public GameObject newPlayerInstance;

    public int numPlayers;

    public MultiplayerUIManager multiplayerUIManager;
    private bool startable = false;

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Update()
    {
        numPlayers = GameObject.FindGameObjectsWithTag("Player").Length;
        if (multiplayerUIManager)
        {
            if(numPlayers >= 2 && Input.GetKeyDown(KeyCode.B))
            {
                networkManager.BroadcastNewMessage("startGame", new { });
                startable = true;
                SceneManager.LoadScene("Map1");
            }
            multiplayerUIManager.DisplayGoTime(numPlayers >= 2);
        }
    }

    public void HostGame()
    {
        networkManager.CreateRoom(true);
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
                StartCoroutine(LerpPosition(otherPlayer.transform, targetPosition, 0.1f));
            } else if (newMessage.msg.message == "startGame" && !startable)
            {
                startable = true;
                SceneManager.LoadScene("Map1");
            }
        } else
        {
            GameObject newPlayer = Instantiate(newPlayerInstance);
            newPlayer.name = "Player" + newMessage.msg.from;
        }
    }

    IEnumerator LerpPosition(Transform playerTransform, Vector3 targetPosition, float duration)
    {
        float elapsedTime = 0f;
        Vector3 startPosition = playerTransform.position;

        while (elapsedTime < duration)
        {
            playerTransform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        playerTransform.position = targetPosition;
    }

}


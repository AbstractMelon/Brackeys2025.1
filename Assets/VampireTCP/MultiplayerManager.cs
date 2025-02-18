using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class PositionData
{
    public string t { get; set; }
}

public class MultiplayerManager : MonoBehaviour
{
    [SerializeField] private VampireTCP networkManager;

    public GameObject newPlayerInstance;
    private GameObject startText;

    public int numPlayers = 1;

    private bool beginningGame = false;

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

    public void ExitGame()
    {
        UnityEngine.Application.Quit();
    }

    public void StartGame()
    {
        networkManager.BroadcastNewMessage("beginGame", 0);
        beginningGame = true;
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

    public void OnRecieveNewMessage(MessageWrapper wrapper)
    {
        if(GameObject.Find("Player" +wrapper.msg.from))
        {
            if (wrapper.msg.message == "updatePlayerPosition")
            {
                GameObject otherPlayer = GameObject.Find("Player" + wrapper.msg.from);
                PositionData posData = JsonConvert.DeserializeObject<PositionData>(wrapper.msg.value.ToString());
                Vector3 targetPosition = StringToVector3(posData.t);
                StartCoroutine(LerpPosition(otherPlayer.transform, targetPosition, 0.1f));
            } else if (wrapper.msg.message == "beginGame" && !beginningGame)
            {
                beginningGame = true;
                SceneManager.LoadScene("Lobby");
            }
        } else
        {
            GameObject newPlayer = Instantiate(newPlayerInstance);
            newPlayer.name = "Player" + wrapper.msg.from;
            numPlayers++;
            LobbyManager.instance.UpdatePlayerCount(numPlayers);
            if(numPlayers >= 2)
            {
                GameObject.Find("GoTime Text").SetActive(true);
            }
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


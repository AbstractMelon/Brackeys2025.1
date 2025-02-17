using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PositionData
{
    public string t { get; set; }
}

public class MultiplayerManager : MonoBehaviour
{
    [SerializeField] private VampireTCP networkManager;

    public GameObject newPlayerInstance;

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
                PositionData posData = JsonConvert.DeserializeObject<PositionData>(wrapper.msg.value.ToString());

                GameObject.Find("Player" + wrapper.msg.from).transform.position = StringToVector3(posData.t);
            }
        } else
        {
            GameObject newPlayer = Instantiate(newPlayerInstance);
            newPlayer.name = "Player" + wrapper.msg.from;
        }
    }
}

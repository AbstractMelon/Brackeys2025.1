using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using Newtonsoft.Json;
using UnityEngine.Windows;

[System.Serializable]
public class MessageEvent : UnityEvent<MessageWrapper> { }

[System.Serializable]
public class AudioMessageEvent : UnityEvent<AudioMessageWrapper> { }

[System.Serializable]
public class GenericMessageEvent : UnityEvent<GenericMessageWrapper> { }

[System.Serializable]
public class MessageWrapper
{
    public BroadcastMessage msg;
    public ErrorMessage err;

    public MessageWrapper(BroadcastMessage msg, ErrorMessage err)
    {
        this.msg = msg;
        this.err = err;
    }
}

[System.Serializable]
public class AudioMessageWrapper
{
    public AudioBroadcastMessage msg;
    public ErrorMessage err;

    public AudioMessageWrapper(AudioBroadcastMessage msg, ErrorMessage err)
    {
        this.msg = msg;
        this.err = err;
    }
}

[System.Serializable]
public class GenericMessageWrapper
{
    public GenericMessage msg;
    public ErrorMessage err;

    public GenericMessageWrapper(GenericMessage msg, ErrorMessage err)
    {
        this.msg = msg;
        this.err = err;
    }
}


public class GenericMessage
{
    public string[] msg { get; set; }

    public GenericMessage(string[] msg)
    {
        this.msg = msg;
    }
}

public class BaseMessage
{
    public string action { get; set; }
}

public class RoomCreatedMessage : BaseMessage
{
    public string room_code { get; set; }
}

public class JoinedRoomMessage : BaseMessage
{
    public string room_code { get; set; }
}

public class RoomsListMessage : BaseMessage
{
    public string[] value { get; set; }
}

public class ClientIDMessage : BaseMessage
{
    public int value { get; set; }
}

public class BroadcastMessage : BaseMessage
{
    public int from { get; set; }
    public string message { get; set; }
    public object value { get; set; }
}

public class AudioBroadcastMessage : BaseMessage
{
    public int from { get; set; }
    public byte[] audio { get; set; }
}

public class ErrorMessage : BaseMessage
{
    public string message { get; set; }
}

public class VampireTCP : MonoBehaviour
{
    public string serverAddress = "127.0.0.1";
    public int serverPort = 8888;
    private TcpClient client;
    private NetworkStream stream;

    public int clientId;

    public MessageEvent onRecieveNewMessage;

    public AudioMessageEvent onRecieveNewAudioMessage;

    public GenericMessageEvent onRecieveNewBaseMessage;
    public int localID;

    async void Start()
    {
        await ConnectToServer();
    }

    async Task ConnectToServer()
    {
        try
        {
            client = new TcpClient();
            await client.ConnectAsync(serverAddress, serverPort);
            stream = client.GetStream();
            Debug.Log("Connected to server");
            StartReceiving();
        }
        catch (Exception ex)
        {
            Debug.LogError("Connection error: " + ex.Message); // TODO: Indicate Failure To Connect To Server
        }
    }

    public async void Reconnect()
    {
        Debug.Log("Reconnecting to server...");

        if (client != null)
        {
            try
            {
                if (client.Connected)
                {
                    client.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Error while disconnecting: " + ex.Message);
            }
            finally
            {
                client = null;
                stream = null;
            }
        }

        await ConnectToServer();
    }

    async void StartReceiving()
    {
        byte[] buffer = new byte[4096];
        while (client.Connected)
        {
            try
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                {
                    Debug.Log("Disconnected from server.");
                    break;
                }
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                string[] messages = message.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string msg in messages)
                {
                    if (msg.StartsWith("{") && msg.EndsWith("}"))
                    {
                        ProcessMessage(msg);
                    }
                }
            }
            catch (Exception ex)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                {
                    Debug.Log("Disconnected from server.");
                    break;
                }
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Debug.LogError("Receive error: " + ex.Message + " | Recieved: "+ message);
            }
        }
    }

    void ProcessMessage(string jsonMessage)
    {
        BaseMessage baseMsg = JsonConvert.DeserializeObject<BaseMessage>(jsonMessage);

        switch (baseMsg.action)
        {
            case "clientId":
                ClientIDMessage NewClientID = JsonConvert.DeserializeObject<ClientIDMessage>(jsonMessage);
                clientId = NewClientID.value;
                break;
            case "roomCreated":
                RoomCreatedMessage roomCreated = JsonConvert.DeserializeObject<RoomCreatedMessage>(jsonMessage);
                Debug.Log("Room created with code: " + roomCreated.room_code);
                LobbyManager.instance.UpdateRoomCode(roomCreated.room_code);
                break;
            case "joinedRoom":
                JoinedRoomMessage joinedRoom = JsonConvert.DeserializeObject<JoinedRoomMessage>(jsonMessage);
                Debug.Log("Joined room: " + joinedRoom.room_code);
                LobbyManager.instance.UpdateRoomCode(joinedRoom.room_code);
                break;
            case "roomsList":
                RoomsListMessage roomsList = JsonConvert.DeserializeObject<RoomsListMessage>(jsonMessage);
                onRecieveNewBaseMessage?.Invoke(new GenericMessageWrapper(new GenericMessage(roomsList.value), new ErrorMessage()));
                Debug.Log("Available rooms: " + string.Join(", ", roomsList.value));
                break;
            case "broadcast":
                BroadcastMessage broadcast = JsonConvert.DeserializeObject<BroadcastMessage>(jsonMessage);
                OnRecieveNewMessage(broadcast, new ErrorMessage());
                //Debug.Log("Broadcast from client " + broadcast.from + ": " + broadcast.message + " - Value: " + broadcast.value);
                break;
            case "voice":
                AudioBroadcastMessage voiceMsg = JsonConvert.DeserializeObject<AudioBroadcastMessage>(jsonMessage);
                OnRecieveNewAudioMessage(voiceMsg, new ErrorMessage());
                //Debug.Log("Broadcast from client " + broadcast.from + ": " + broadcast.message + " - Value: " + broadcast.value);
                break;
            case "error":
                ErrorMessage errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(jsonMessage);
                OnRecieveNewMessage(new BroadcastMessage(), errorMsg);
                Debug.LogError("Error: " + errorMsg.message);
                break;
            default:
                Debug.Log("Unknown action: " + baseMsg.action);
                break;
        }
    }

    public void OnRecieveNewMessage(BroadcastMessage msg, ErrorMessage err)
    {
        onRecieveNewMessage?.Invoke(new MessageWrapper(msg, err));
    }

    public void OnRecieveNewAudioMessage(AudioBroadcastMessage msg, ErrorMessage err)
    {
        onRecieveNewAudioMessage?.Invoke(new AudioMessageWrapper(msg, err));
    }

    public void CreateRoom(bool isPublic)
    {
        var msg = new
        {
            action = "createRoom",
            publicRoom = isPublic
        };
        SendNewMessage(JsonConvert.SerializeObject(msg));
    }

    public void SendVoiceMessage(byte[] audioData)
    {
        var msg = new
        {
            action = "voice",
            audio = Convert.ToBase64String(audioData)
        };
        SendNewMessage(JsonConvert.SerializeObject(msg));
    }

    public void refreshToken(string roomCode)
    {
        var msg = new
        {
            action = "createRoom",
            room_code = roomCode
        };
        SendNewMessage(JsonConvert.SerializeObject(msg));
    }

    public void JoinRoom(string roomCode)
    {
        var msg = new
        {
            action = "joinRoom",
            room_code = roomCode
        };
        SendNewMessage(JsonConvert.SerializeObject(msg));
    }

    public void RequestRoomsList()
    {
        var msg = new
        {
            action = "listRooms"
        };
        SendNewMessage(JsonConvert.SerializeObject(msg));
    }

    public void BroadcastNewMessage(string messageText, object value)
    {
        var msg = new
        {
            action = "broadcast",
            message = messageText,
            value = value
        };
        SendNewMessage(JsonConvert.SerializeObject(msg));
    }

    async void SendNewMessage(string json)
    {
        if (stream != null && stream.CanWrite)
        {
            byte[] data = Encoding.UTF8.GetBytes(json + "\n");
            try
            {
                await stream.WriteAsync(data, 0, data.Length);
                await stream.FlushAsync();
            }
            catch (Exception ex)
            {
                Debug.LogError("Send error: " + ex.Message);
            }
        }
    }

    private void OnApplicationQuit()
    {
        if (client != null)
        {
            client.Close();
        }
    }
}


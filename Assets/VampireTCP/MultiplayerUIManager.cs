using UnityEngine;

public class MultiplayerUIManager : MonoBehaviour
{
    public GameObject GoTime;

    public void DisplayGoTime(bool v)
    {
        GoTime.SetActive(v);
    }
}

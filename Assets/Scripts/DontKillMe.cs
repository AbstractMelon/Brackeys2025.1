using UnityEngine;
using UnityEngine.SceneManagement;

public class DontKillMe : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}

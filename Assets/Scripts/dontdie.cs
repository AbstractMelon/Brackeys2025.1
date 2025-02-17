using UnityEngine;
using UnityEngine.SceneManagement;

public class dontdie : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}

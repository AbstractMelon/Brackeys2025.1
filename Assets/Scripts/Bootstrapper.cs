using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrapper : MonoBehaviour
{
    private void Awake()
    {
        // Load the Bootstrap scene
        SceneManager.LoadScene("Bootstrap", LoadSceneMode.Additive);
    }
}


using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button mainMenuButton;

    [Header("Settings")]
    [SerializeField] private KeyCode pauseKey = KeyCode.Escape;
    [SerializeField] private string mainMenuScene = "MainMenu";

    private bool isPaused = false;

    private void Awake()
    {
        // Set up button callbacks
        resumeButton.onClick.AddListener(ResumeGame);
        mainMenuButton.onClick.AddListener(QuitToMainMenu);
        
        // Initialize state
        menuPanel.SetActive(false);
        UpdateCursor();
    }

    private void Update()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        menuPanel.SetActive(isPaused);
        UpdateCursor();
    }

    private void UpdateCursor()
    {
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public void ResumeGame()
    {
        isPaused = false;
        menuPanel.SetActive(false);
        UpdateCursor();
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuScene);
    }

}
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public MainMenuPanel[] panels;

    private MainMenuPanel currentPanel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (var i in panels)
        {
            i.panel.SetActive(false);
        }

        currentPanel = panels[0];
        currentPanel.panel.SetActive(true);
    }

    public void ShowPanel(int index)
    {
        foreach (var i in panels)
        {
            i.panel.SetActive(false);
        }

        currentPanel = panels[index];
        currentPanel.panel.SetActive(true);
    }

    public void ShowPanel(MainMenuPanel panel)
    {
        foreach (var i in panels)
        {
            i.panel.SetActive(false);
        }

        currentPanel = panel;
        currentPanel.panel.SetActive(true);
    }

    [ContextMenu("Exit")]
    public void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}


[System.Serializable]
public class MainMenuPanel
{
    public string name;
    public GameObject panel;
}


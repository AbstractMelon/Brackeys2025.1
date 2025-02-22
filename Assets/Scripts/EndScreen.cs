using System.Collections;
using UnityEditor;
using UnityEngine;
using TMPro;

public class EndScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Animator animator;

    private bool demonWon = true;

    // Parameter names for the Animator
    private const string DemonWonParam = "DemonWon";
    private const string ShowTrigger = "Show";

    private void Start()
    {
        ShowEndScreen(demonWon);
    }

    public void ShowEndScreen(bool demonWon)
    {
        // Update the message text
        messageText.text = demonWon ? "Demon Wins! - Killed all survivors!" : "Players Win! - Survived for " + (int)Time.time + " minutes";
        
        // Update the Animator parameters
        if (animator != null)
        {
            animator.SetBool(DemonWonParam, demonWon);
            animator.SetTrigger(ShowTrigger);
        }
        else
        {
            Debug.LogWarning("Animator reference not set in EndScreen.", this);
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(EndScreen))]
    public class EndScreenEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EndScreen endScreen = (EndScreen)target;
            if (GUILayout.Button("Toggle End Screen"))
            {
                // Toggle the demonWon state and update the screen
                endScreen.demonWon = !endScreen.demonWon;
                endScreen.ShowEndScreen(endScreen.demonWon);
                EditorUtility.SetDirty(endScreen); // Save the state change
            }
        }
    }
#endif
}
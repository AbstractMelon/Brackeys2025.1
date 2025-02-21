using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private MultiplayerManager multiplayerManager;
    public static GameManager instance;

    // Start is called before the first frame update
    void Start()
    {
        multiplayerManager = FindObjectsByType<MultiplayerManager>(FindObjectsSortMode.None)[0];
    }

    // Update is called once per frame
    void Update()
    {
        //CheckGameOver();
    }

    // Check if the game is over
    public void CheckGameOver()
    {
        Debug.Log(multiplayerManager.numPlayers);
        if (multiplayerManager.numPlayers <= 0)
        {
            EndGame();
        }
    }

    // Ends the game and declares the winner
    public void EndGame()
    {
        string winner = (multiplayerManager.numPlayers >= 1) ? "Survivors" : "Demon";
        Debug.Log($"{winner} wins!");
    }
}


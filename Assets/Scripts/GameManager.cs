using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<PlayerController> players; // List of players in the game
    private PlayerController demon; // The player who is the demon
    private int alivePlayers; // Count of players still alive

    // Start is called before the first frame update
    void Start()
    {
        AssignRandomDemon();
        alivePlayers = players.Count;
    }

    // Update is called once per frame
    void Update()
    {
        CheckGameOver();
    }

    // Assign a random player as the demon
    void AssignRandomDemon()
    {
        int randomIndex = Random.Range(0, players.Count);
        demon = players[randomIndex];
    }

    // Check if the game is over
    void CheckGameOver()
    {
        alivePlayers = 0;
        foreach (var player in players)
        {
            if (!player.IsDead())
            {
                alivePlayers++;
            }
        }

        if (alivePlayers <= 1)
        {
            EndGame();
        }
    }

    // Ends the game and declares the winner
    void EndGame()
    {
        string winner = (alivePlayers == 1) ? "Survivors" : "Demon";
        Debug.Log($"{winner} wins!");
    }
}


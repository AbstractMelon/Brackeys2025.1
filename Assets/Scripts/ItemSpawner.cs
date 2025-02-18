using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [Tooltip("List of potential spawn points")]
    public List<Transform> spawnPoints = new List<Transform>();

    [Tooltip("Prefabs to spawn randomly")]
    public List<GameObject> spawnablePrefabs = new List<GameObject>();

    [Header("Initial Spawn")]
    [Range(0, 1), Tooltip("Percentage of spawn points to use initially")]
    public float initialSpawnChance = 0.4f;

    [Header("Periodic Spawning")]
    public bool enablePeriodicSpawning = true;
    [Tooltip("Minimum time between spawns in seconds")]
    public float minSpawnInterval = 5f;
    [Tooltip("Maximum time between spawns in seconds")]
    public float maxSpawnInterval = 10f;

    private void Start()
    {
        ValidateLists();
        SpawnInitialItems();
        
        if (enablePeriodicSpawning)
            StartCoroutine(PeriodicSpawningRoutine());
    }

    private void ValidateLists()
    {
        if (spawnPoints.Count == 0)
            Debug.LogError("No spawn points assigned!", this);
        
        if (spawnablePrefabs.Count == 0)
            Debug.LogError("No spawnable prefabs assigned!", this);
    }

    private void SpawnInitialItems()
    {
        foreach (Transform spawnPoint in spawnPoints)
        {
            if (Random.value < initialSpawnChance)
            {
                SpawnRandomPrefabAtPoint(spawnPoint);
            }
        }
    }

    private IEnumerator PeriodicSpawningRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(waitTime);

            if (spawnPoints.Count > 0 && spawnablePrefabs.Count > 0)
            {
                Transform randomPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
                SpawnRandomPrefabAtPoint(randomPoint);
            }
        }
    }

    private void SpawnRandomPrefabAtPoint(Transform spawnPoint)
    {
        GameObject selectedPrefab = spawnablePrefabs[Random.Range(0, spawnablePrefabs.Count)];
        Instantiate(selectedPrefab, spawnPoint.position, spawnPoint.rotation);
    }
}
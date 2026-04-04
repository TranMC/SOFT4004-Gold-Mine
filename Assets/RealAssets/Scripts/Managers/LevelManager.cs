using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Quan ly noi dung level: spawn vat the, target score, reset level.
/// </summary>
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Level Config")]
    [SerializeField] private int targetScore = 500;
    [SerializeField] private List<GameObject> spawnPrefabs = new List<GameObject>();
    [SerializeField] private Transform[] spawnPoints;

    private readonly List<GameObject> spawnedObjects = new List<GameObject>();

    public int TargetScore => targetScore;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void InitializeLevel(int levelIndex)
    {
        // TODO: map theo levelIndex neu co nhieu bo du lieu level.
        ResetLevel();
        SpawnLevelObjects();
     UIManager.Instance?.UpdateTargetUI(targetScore);
    UIManager.Instance?.UpdateCapUI(levelIndex);
    }

    public void StartLevel()
    {
        GameManager.Instance?.SetState(GameState.Playing);
        TimerManager.Instance?.StartTimer();
    }

    public void ResetLevel()
    {
        for (int i = 0; i < spawnedObjects.Count; i++)
        {
            if (spawnedObjects[i] != null)
            {
                Destroy(spawnedObjects[i]);
            }
        }

        spawnedObjects.Clear();
    }

    private void SpawnLevelObjects()
    {
        if (spawnPrefabs.Count == 0 || spawnPoints == null || spawnPoints.Length == 0)
        {
            return;
        }

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            int randomIndex = Random.Range(0, spawnPrefabs.Count);
            GameObject spawned = Instantiate(spawnPrefabs[randomIndex], spawnPoints[i].position, Quaternion.identity);
            spawnedObjects.Add(spawned);
        }
    }

    public void SetTargetScore(int amount)
    {
        targetScore = Mathf.Max(0, amount);
    }
}

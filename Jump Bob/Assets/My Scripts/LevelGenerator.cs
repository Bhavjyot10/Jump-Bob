using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private List<Transform> LevelParts;
    [SerializeField] private Transform Player;
    [SerializeField] private Transform initialEndPos;
    [SerializeField] private Transform spawnParent;
    [SerializeField] private float SpawnLevelDistance = 200f;
    private Transform EndPos;
    public bool infiniteLevel = false;
    public static bool isInfiniteLevel = false;
    [SerializeField] int levelPartsToSpawn = 10;
    int levelPartsLeft;
    int totalPlatformsPresent = 0;
    public Transform grid;

    void Start()
    {
        isInfiniteLevel = infiniteLevel;
        levelPartsLeft = levelPartsToSpawn;
        EndPos = initialEndPos;
        for (int i = 0; i < 1; i++)
        {
            SpawnLevel(i);
        }
    }


    Transform LevelSpawner(Transform SpawnPos, int levelPartNumber)
    {
        SpawnPos.position = new Vector3(SpawnPos.position.x, Mathf.Clamp(SpawnPos.position.y, -25.5f, 9.5f), SpawnPos.position.z);
        Transform LastSpawned = Instantiate(LevelParts[levelPartNumber], SpawnPos.position, Quaternion.identity, spawnParent);
        if (!isInfiniteLevel)
            levelPartsLeft--;
        return LastSpawned;
    }

    // Update is called once per frame
    void Update()
    {
        if (levelPartsLeft > 1)
        {
            if (Vector3.Distance(EndPos.position, Player.position) < SpawnLevelDistance)
            {
                SpawnLevel();
            }
        }
    }

    void SpawnLevel(int levelPartNumber)
    {
        Transform pos = LevelSpawner(EndPos, levelPartNumber);
        EndPos = pos.Find("EndPos");

        if (levelPartsLeft == 1)
            SpawnLevel(LevelParts.Count - 1);
    }

    void SpawnLevel()
    {
        int levelToSpawn;
        levelToSpawn = Random.Range(0, (LevelParts.Count - 1));

        Transform pos = LevelSpawner(EndPos, levelToSpawn);
        EndPos = pos.Find("EndPos");

        if (levelPartsLeft == 1)
            SpawnLevel(LevelParts.Count - 1);
    }

    public int TotalPlatforms()
    {
        var platforms = FindObjectsOfType<GroundReach>();
        totalPlatformsPresent = platforms.Length;
        return (totalPlatformsPresent - 1);
    }

    public int PlatformsReached()
    {
        return levelPartsToSpawn;
    }

    public bool IsLevelInfinite()
    {
        return infiniteLevel;
    }
}

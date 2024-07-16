using System.Collections.Generic;
using System.Linq;
using CombatScene;
using CombatScene.Enemy;
using UnityEngine;
using UnityEngine.Events;

public class EnemySpawnSystem : MonoBehaviour
{
    [SerializeField] private List<GameObject> enemyPrefabs;
    [SerializeField] private List<float> enemySpawnWeights;
    [SerializeField] private GameObject player;
    [SerializeField] private MapHandler mapHandler;
    [SerializeField] private CombatManager combatManager;
    [SerializeField] private int maxActiveEnemies = 10;
    [SerializeField] private int poolSizePerType = 20;
    [SerializeField] private float initialSpawnTime = 5f;
    [SerializeField] private int spawnRange = 3;

    private Dictionary<string, Queue<GameObject>> enemyPools;
    private Dictionary<GameObject, string> enemyTypeMap;
    private List<string> enemyTypes;
    private float currentSpawnTime;
    private float spawnTimer = 0f;
    private int spawnCount = 0;
    private int powerUp = 0;

    void Start()
    {
        InitializeEnemyPools();
        currentSpawnTime = initialSpawnTime;
        enemyTypes = new List<string>(enemyPools.Keys);
        SubscribeToEnemyEvents();
    }

    void Update()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= currentSpawnTime)
        {
            spawnTimer = 0f;
            SpawnEnemy();
            UpdateSpawnTime();
        }
    }

    void InitializeEnemyPools()
    {
        enemyPools = new Dictionary<string, Queue<GameObject>>();
        enemyTypeMap = new Dictionary<GameObject, string>();

        foreach (GameObject enemyPrefab in enemyPrefabs)
        {
            Queue<GameObject> pool = new Queue<GameObject>();
            for (int i = 0; i < poolSizePerType; i++)
            {
                GameObject enemy = Instantiate(enemyPrefab, transform);
                enemy.SetActive(false);
                pool.Enqueue(enemy);
                enemyTypeMap[enemy] = enemyPrefab.name;
            }
            enemyPools[enemyPrefab.name] = pool;
        }
    }

    void SubscribeToEnemyEvents()
    {
        foreach (var pool in enemyPools.Values)
        {
            foreach (var enemy in pool)
            {
                EnemyController controller = enemy.GetComponent<EnemyController>();
                if (controller != null)
                {
                    controller.OnEnemyDead.AddListener(OnEnemyDefeated);
                }
            }
        }
    }

    void SpawnEnemy()
    {
        if (GetActiveEnemyCount() >= maxActiveEnemies)
        {
            Debug.Log("최대 적군 수에 도달했습니다. 생성을 중단합니다.");
            return;
        }

        Vector2 spawnPosition = FindValidSpawnPosition();
        if (spawnPosition != Vector2.negativeInfinity)
        {
            string enemyType = GetRandomEnemyType();
            GameObject enemy = GetEnemyFromPool(enemyType);
            if (enemy != null)
            {
                enemy.transform.position = spawnPosition;
                enemy.SetActive(true);
                InitializeEnemy(enemy);
                spawnCount++;
                UpdatePowerUp();
            }
            else
            {
                Debug.LogWarning($"풀에 {enemyType} 타입의 적이 없습니다.");
            }
        }
        else
        {
            Debug.Log("유효한 스폰 위치를 찾지 못했습니다.");
        }
    }

    Vector2 FindValidSpawnPosition()
    {
        Vector2 playerPosition = combatManager.playerPosition;
        List<Vector2> validPositions = new List<Vector2>();

        for (int x = -spawnRange; x <= spawnRange; x++)
        {
            for (int y = -spawnRange; y <= spawnRange; y++)
            {
                if (x == 0 && y == 0) continue;
                
                Vector2 pos = new Vector2(playerPosition.x + x, playerPosition.y + y);
                if (IsPositionWithinMapBounds(pos) && CheckSpawn(pos))
                {
                    validPositions.Add(pos);
                }
            }
        }

        if (validPositions.Count > 0)
        {
            return validPositions[Random.Range(0, validPositions.Count)];
        }

        return Vector2.negativeInfinity;
    }

    string GetRandomEnemyType()
    {
        if (enemySpawnWeights.Count == 0 || enemySpawnWeights.Count != enemyTypes.Count)
        {
            return enemyTypes[Random.Range(0, enemyTypes.Count)];
        }

        float totalWeight = enemySpawnWeights.Sum();
        float randomValue = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        for (int i = 0; i < enemyTypes.Count; i++)
        {
            cumulative += enemySpawnWeights[i];
            if (randomValue <= cumulative)
            {
                return enemyTypes[i];
            }
        }

        return enemyTypes[enemyTypes.Count - 1];
    }

    GameObject GetEnemyFromPool(string enemyType)
    {
        if (enemyPools.TryGetValue(enemyType, out Queue<GameObject> pool))
        {
            if (pool.Count > 0)
            {
                return pool.Dequeue();
            }
        }
        return null;
    }

    void InitializeEnemy(GameObject enemy)
    {
        EnemyController enemyManager = enemy.GetComponent<EnemyController>();
        enemyManager.SetCombatManager(combatManager);
        enemyManager.AddEnemyToSpawn();
        enemyManager.SetVariabePowerup(powerUp);
    }

    bool IsPositionWithinMapBounds(Vector2 position)
    {
        Vector2 relativePoint = position - mapHandler.startPosition;
        int idxX = Mathf.FloorToInt((relativePoint.x + ConstVariables.tileSizeX / 2) / ConstVariables.tileSizeX);
        int idxY = Mathf.FloorToInt((relativePoint.y + ConstVariables.tileSizeY / 2) / ConstVariables.tileSizeY);

        return idxX >= 0 && idxX < ConstVariables.mapWidth && idxY >= 0 && idxY < ConstVariables.mapHeight;
    }

    bool CheckSpawn(Vector2 position)
    {
        return mapHandler.GetPoint(position) == ObjectType.Load;
    }

    int GetActiveEnemyCount()
    {
        int activeCount = 0;
        foreach (var pool in enemyPools.Values)
        {
            activeCount += poolSizePerType - pool.Count;
        }
        return activeCount;
    }

    void UpdateSpawnTime()
    {
        if (spawnCount % 3 == 0 && currentSpawnTime > 2f)
        {
            currentSpawnTime -= 0.2f;
        }
    }

    void UpdatePowerUp()
    {
        if (spawnCount % 3 == 0 && powerUp < 20)
        {
            powerUp++;
        }
    }

    public void ReturnEnemyToPool(GameObject enemy)
    {
        if (enemyTypeMap.TryGetValue(enemy, out string enemyType))
        {
            enemy.SetActive(false);
            if (enemyPools.TryGetValue(enemyType, out Queue<GameObject> pool))
            {
                pool.Enqueue(enemy);
            }
            else
            {
                Debug.LogError($"Enemy type {enemyType} not found in pools.");
            }
        }
        else
        {
            Debug.LogError($"Enemy {enemy.name} not found in type mapping.");
        }
    }

    void OnEnemyDefeated(Vector2 position)
    {
        GameObject enemy = FindEnemyAtPosition(position);
        if (enemy != null)
        {
            ReturnEnemyToPool(enemy);
        }
        else
        {
            Debug.LogWarning($"Enemy at position {position} not found.");
        }
    }

    GameObject FindEnemyAtPosition(Vector2 position)
    {
        foreach (var enemy in enemyTypeMap.Keys)
        {
            if (enemy.activeSelf && Vector2.Distance(enemy.transform.position, position) < 0.1f)
            {
                return enemy;
            }
        }
        return null;
    }

    void OnDestroy()
    {
        foreach (var pool in enemyPools.Values)
        {
            foreach (var enemy in pool)
            {
                EnemyController controller = enemy.GetComponent<EnemyController>();
                if (controller != null)
                {
                    controller.OnEnemyDead.RemoveListener(OnEnemyDefeated);
                }
            }
        }
    }
}
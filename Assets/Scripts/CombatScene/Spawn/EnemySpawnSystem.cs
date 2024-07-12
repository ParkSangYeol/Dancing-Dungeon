using System.Collections.Generic;
using CombatScene;
using CombatScene.Enemy;
using UnityEngine;

public class EnemySpawnSystem : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> enemyPrefabs;

    private List<GameObject> enemyPool;
    [SerializeField]
    private GameObject player;
    public MapHandler mapHandler;
    [SerializeField]
    private CombatManager combatManager;
    public int range;
    public int poolSize;
    private int currentPoolIndex = 0;
    public float spawnTime = 5f;
    private float currentTime = 0f;
    public int powerUp = 0;
    private int spawnNum = 0;

    [SerializeField]
    private int maxActiveEnemies = 10;

    void Start()
    {
        enemyPool = new List<GameObject>();
        InstantiateEnemy();
    }

    void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime > spawnTime)
        {
            currentTime = 0f;
            SpawnEnemy();
            if (spawnNum % 3 == 0 && spawnTime > 2f)
            {
                spawnTime -= 0.2f;
            }
        }
    }

    void InstantiateEnemy()
    {
        for (int i = 0; i < poolSize; i++)
        {
            foreach (GameObject enemyPrefab in enemyPrefabs)
            {
                GameObject enemy = Instantiate(enemyPrefab);
                enemy.SetActive(false);
                enemy.transform.SetParent(transform, true);
                enemyPool.Add(enemy);
            }
        }
    }

    void SpawnEnemy()
    {
        if (GetActiveEnemyCount() >= maxActiveEnemies)
        {
            Debug.Log("최대 적군 수임. 생성하지마라");
            return;
        }

        Vector2 playerPosition = combatManager.playerPosition;
        int maxAttempts = 10;
        int attempts = 0;

        while (attempts++ < maxAttempts)
        {
            int spawnX = (int)playerPosition.x + GetRandomValue(range);
            int spawnY = (int)playerPosition.y + GetRandomValue(range);
            Vector2 spawnPosition = new Vector2(spawnX, spawnY);

            if (IsPositionWithinMapBounds(spawnPosition) && CheckSpawn(spawnPosition))
            {
                for (int i = 0; i < enemyPool.Count; i++)
                {
                    int index = (currentPoolIndex + i) % enemyPool.Count;
                    if (!enemyPool[index].activeInHierarchy)
                    {
                        GameObject enemy = enemyPool[index];
                        enemy.transform.position = spawnPosition;
                        enemy.SetActive(true);
                        InitializeEnemy(enemy);
                        currentPoolIndex = (index + 1) % enemyPool.Count;
                        spawnNum++;
                        if (spawnNum % 3 == 0 && powerUp < 20)
                        {
                            powerUp++;
                        }
                        return;
                    }
                }
            }
        }

        Debug.Log("유효한 스폰 위치를 찾지 못했습니다. " + maxAttempts + "번 시도 후 실패했습니다.");
    }

    void InitializeEnemy(GameObject enemy)
    {
        EnemyController enemyManager = enemy.GetComponent<EnemyController>();
        enemyManager.SetCombatManager(combatManager);
        enemyManager.AddEnemyToSpawn();
        enemyManager.SetVariabePowerup(powerUp);
    }

    bool IsPositionWithinMapBounds(Vector2 spawnPosition)
    {
        Vector2 relativePoint = spawnPosition - mapHandler.startPosition;
        int idxX = (int)((relativePoint.x + ConstVariables.tileSizeX / 2) / ConstVariables.tileSizeX);
        int idxY = (int)((relativePoint.y + ConstVariables.tileSizeY / 2) / ConstVariables.tileSizeY);

        return idxX >= 0 && idxX < ConstVariables.mapWidth && idxY >= 0 && idxY < ConstVariables.mapHeight;
    }

    bool CheckSpawn(Vector2 spawnPosition)
    {
        return mapHandler.GetPoint(spawnPosition) == ObjectType.Load;
    }

    int GetRandomValue(int range)
    {
        int randomValue = Random.Range(0, 2);
        return (randomValue == 0) ? -range : range;
    }

    int GetActiveEnemyCount()
    {
        int activeCount = 0;
        foreach (GameObject enemy in enemyPool)
        {
            if (enemy.activeInHierarchy)
            {
                activeCount++;
            }
        }
        return activeCount;
    }
}

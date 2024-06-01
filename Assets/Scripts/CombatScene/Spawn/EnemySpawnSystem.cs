using System.Collections.Generic;
using CombatScene;
using CombatScene.Enemy;
using UnityEngine;
using UnityEngine.Analytics;

public class EnemySpawnSystem : MonoBehaviour
{
    // 몬스터 생성 프리팹 목록
    [SerializeField]
    private List<GameObject> enemysPrefabs;

    // 몬스터 풀
    private List<GameObject> enemyPool;
    [SerializeField]
    private GameObject player;
    public MapHandler mapHandler;
    [SerializeField]
    private CombatManager combatManager;
    public int range;
    public int poolSize;
    private int currentPoolIndex = 0;
    private Vector2 randomSpawn; // 스폰 금지구역일 경우 랜덤을 계속 돌려서 어떻게든 찾아낸다.
    public float spawnTime = 5f;
    private float currentTime = 0f;

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
        }
    }

    void InstantiateEnemy()
    {
        // 풀에 프리팹 등록
        for (int i = 0; i < poolSize; i++)
        {
            foreach (GameObject enemyPrefab in enemysPrefabs)
            {
                GameObject enemy = Instantiate(enemyPrefab);
                enemy.SetActive(false);
                enemyPool.Add(enemy);
            }
        }
    }

    void SpawnEnemy()
    {
        Vector2 playerPosition = combatManager.playerPosition;
        Vector2 spawnPosition = Vector2.zero;

        // 스폰 위치 결정
        bool validSpawn = false;
        while (!validSpawn)
        {
            spawnPosition = playerPosition + new Vector2(GetRandomValue(range), GetRandomValue(range));
            if (CheckSpawn(spawnPosition))
            {
                
                validSpawn = true;
                break;
            }
        }

        // 적 활성화 및 위치 설정
        if (currentPoolIndex < enemyPool.Count)
        {
            GameObject enemy = enemyPool[currentPoolIndex];
            enemy.SetActive(true);
            enemy.GetComponent<RectTransform>().anchoredPosition = spawnPosition;
            currentPoolIndex = (currentPoolIndex + 1) % enemyPool.Count;
        }
    }

    bool CheckSpawn(Vector2 spawnPosition)
    {
        // 스폰 지점이 플레이어이거나 장애물인 경우
        return mapHandler.GetPoint(spawnPosition) == ObjectType.Load;
    }

    int GetRandomValue(int range)
    {
        int randomValue = Random.Range(0, 2); // 0 또는 1을 반환
        return (randomValue == 0) ? -range : range; // 0이면 -range, 1이면 range 반환
    }
}

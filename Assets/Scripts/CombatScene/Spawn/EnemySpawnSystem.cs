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
        if (combatManager == null)
        {
            combatManager = GameObject.Find("GameManager").GetComponent<CombatManager>();
        }
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
        int maxAttempts = 10; // 최대 시도 횟수 설정
        int attempts = 0; // 현재 시도 횟수

        
        

        while (attempts < maxAttempts)
        {

            int spawnX = (int)playerPosition.x + GetRandomValue(range);
            int spawnY = (int)playerPosition.y+GetRandomValue(range);
            
            
            Vector2 spawnPosition = new Vector2(spawnX, spawnY);

            Debug.Log(spawnPosition);
            if (CheckSpawn(spawnPosition))
            {
                if (currentPoolIndex < enemyPool.Count)
                {
                    GameObject enemy = enemyPool[currentPoolIndex];
                    enemy.SetActive(true);
                    enemy.GetComponent<RectTransform>().anchoredPosition = spawnPosition;
                    EnemyController enemyManager = enemy.GetComponent<EnemyController>();
                    enemyManager.SetCombatManager(combatManager);
                    enemyManager .AddEnemyToSpawn();
                    enemy.GetComponent<EnemyController>().SetVariables();
                    currentPoolIndex = (currentPoolIndex + 1) % enemyPool.Count;
                    return; 
                }
            }

            
        }

       
        Debug.Log("Failed to find a valid spawn position after " + maxAttempts + " attempts.");
    }

    bool CheckSpawn(Vector2 spawnPosition)
    {
        return mapHandler.GetPoint(spawnPosition) == ObjectType.Load;
    }

    int GetRandomValue(int range)
    {
        int randomValue = Random.Range(0, 2); // 0 또는 1을 반환
        return (randomValue == 0) ? -range : range; // 0이면 -range, 1이면 range 반환
    }
}
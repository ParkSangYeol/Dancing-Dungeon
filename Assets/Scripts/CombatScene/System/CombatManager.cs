//#define TEST_MOVE_WITHOUT_NOTE
using System;
using System.Collections.Generic;
using CombatScene.Enemy;
using CombatScene.Player;
using CombatScene.System;
using CombatScene.System.Particle;
using DG.Tweening;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;
using Sequence = DG.Tweening.Sequence;

namespace CombatScene
{
    public class CombatManager : MonoBehaviour
    {
        #region About Test
        #if TEST_MOVE_WITHOUT_NOTE
        
        [Title("Test Variables")]
        [SerializeField]
        private PlayerInput playerInput;
        private InputAction moveAction;
       
        
        private void PlayerBehavior(InputAction.CallbackContext context)
        {
            Vector2 inputVec = context.ReadValue<Vector2>();
            if (!(inputVec.x == 0 && Mathf.Abs(inputVec.y) == 1) && !(inputVec.y == 0 && Mathf.Abs(inputVec.x) == 1))
            {  
                // 잘못된 입력값
                Debug.LogError("입력된 값이 잘못되었습니다. 입력된 값: " + inputVec);
                return;
            }
            player.LookAt(inputVec);
            
            if (!TryPlayerAttackAble(inputVec, true))
            {
                // 공격 대상 없음
                player.MovePlayer(inputVec);
            }
            else
            {
                // 공격 성공
                SearchTiles();
            }
        }
        
        #endif
        #endregion

        [Title("Components")] 
        [SerializeField] 
        private AttackFocusPool _attackFocusPool;
        public AttackFocusPool attackFocusPool => _attackFocusPool;

        [SerializeField]
        private MapHandler _mapHandler;
        public MapHandler mapHandler => _mapHandler;
        [SerializeField]
        private ParticleManager particleManager;
        [SerializeField]
        private ItemManager itemManager;
        [InfoBox("CombatSceneUIManager 넣어주세요!", InfoMessageType.Error, "CombatSceneUIManagerNotSetup")]
        [SerializeField] 
        private CombatSceneUIManager combatSceneUIManager;
        [InfoBox("플레이어를 넣어주세요!", InfoMessageType.Error, "IsPlayerNotSetup")]
        [SerializeField]
        private PlayerController player;
        
        [Title("Variables")]
        public Vector2 playerPosition;
        private Dictionary<Vector2, EnemyController> enemies = new Dictionary<Vector2, EnemyController>();
        private HitScanByRay hitScanByRay;

        [InfoBox("이 값은 퍼펙트 시 크리티컬 보정값입니다.")]
        [SerializeField] private float perfectCrit;
        private Camera mainCamera;
        private Sequence cameraTween;

        
        private void Awake()
        {
            if (mapHandler == null)
            {
                _mapHandler = GetComponent<MapHandler>();
            }
            
            if (particleManager == null)
            {
                particleManager = GetComponent<ParticleManager>();
            }

            if (itemManager == null)
            {
                itemManager = GetComponent<ItemManager>();
            }
            if (player == null)
            {
                Debug.LogError("플레이어 오브젝트가 추가되지 않았습니다!");
                return;
            }
            
#if TEST_MOVE_WITHOUT_NOTE
            if (playerInput == null)
            {
                playerInput = player.GetComponent<PlayerInput>();
            }
            moveAction = playerInput.actions["Move"];
            moveAction.started += PlayerBehavior;
#endif

            playerPosition = player.transform.position;
        }

        private void Start()
        {
            DOTween.Init();
            mainCamera = Camera.main;
            float cameraSize = Camera.main.orthographicSize;
            
            cameraTween = DOTween.Sequence();
            cameraTween.Append(mainCamera.DOOrthoSize( cameraSize - 0.2f, 0.1f));
            cameraTween.Join(mainCamera.DOShakePosition(0.1f, 10f));
            cameraTween.Append(mainCamera.DOOrthoSize(cameraSize, 0.1f));
            cameraTween.SetAutoKill(false);
            cameraTween.Pause();
        }

        public void MovePlayer(Vector2 targetPosition)
        {
            // 아이템 획득 확인
            ItemInstance dropItem;
            if (itemManager.GetDroppedItem(targetPosition, out dropItem))
            {
                switch (dropItem.itemScriptableObject.itemType)
                {
                    case ItemType.HEAL:
                        player.hp += dropItem.itemScriptableObject.value;
                        
                        break;
                    case ItemType.POWER_UP:
                        player.power += dropItem.itemScriptableObject.value;
                        combatSceneUIManager.SetWeapon(null);
                        
                        break;
                    case ItemType.SHIELD:
                        player.shield += dropItem.itemScriptableObject.value;
                        
                        break;
                    case ItemType.WEAPON:
                    {
                        WeaponScriptableObject equippedWeapon = player.GetEquippedWeapon();
                        if (equippedWeapon != player.defaultWeapon)
                        {
                            // 기존에 무기를 장착 중
                            // 장착중인 무기를 바닥에 드랍.
                            itemManager.PlaceItem(playerPosition, itemManager.GetWeaponItem(equippedWeapon));
                        }
                        // 새로운 무기를 획득
                        player.EquipWeapon(dropItem.itemScriptableObject.weaponScriptableObject);
                        break;
                    }
                    case ItemType.SPECIAL_WEAPON:
                        break;
                }
                
                Destroy(dropItem.gameObject);
            }
            
            if (mapHandler.SetMapObject(targetPosition, ObjectType.Player))
            {
                mapHandler.SetMapObject(playerPosition, ObjectType.Load);
                playerPosition = targetPosition;
            }
            SearchTiles();
        }
        
        public bool MoveEnemy(Vector2 targetPosition, Vector2 enemyPosition)
        {
            ObjectType tileObject = mapHandler.GetPoint(targetPosition);
            
            if (tileObject.Equals(ObjectType.Player) || tileObject.Equals(ObjectType.Enemy) || tileObject.Equals(ObjectType.Boss))
            {
                // 플레이어나 적 위치로는 이동 불가
                return false;
            }
            
            EnemyController enemy = enemies[enemyPosition];
            if (mapHandler.SetMapObject(targetPosition, enemy.tileObjectType))
            {
                mapHandler.SetMapObject(enemyPosition, ObjectType.Load);
                enemies.Remove(enemyPosition);
                enemies.Add(targetPosition, enemy);
                enemy.MoveCharacter(targetPosition);
                
                return true;
            }
            
            return false;
        }

        public void SetEnemyEvent(EnemyController enemy)
        {
            if (enemy is BossController)
            {
                enemy.OnEnemyDead.AddListener((enemyPosition) =>
                {
                    // 랜덤한 아이템을 드랍하도록 설정.
                    float random = Random.Range(0, 1f);
                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            itemManager.SpawnRandomItem(enemyPosition + new Vector2(i, j));
                        }
                    }    
                });
                
                enemy.OnEnemyDead.AddListener((enemyPosition) =>
                {
                    Debug.Log("Enemy Dead. Position is " + enemyPosition);
                    mapHandler.SetMapObject(enemyPosition, ObjectType.Load);
                    EnemyController enemy = null;
                    bool isGetEnemy = enemies.TryGetValue(enemyPosition, out enemy);
                    Debug.Log("Enemy Dead. Get Enemy: " + isGetEnemy + " enemy: " + enemy);
                    Debug.Log("Enemy Dead. Removing Enemy: " + enemies.Remove(enemyPosition));
                });
            }
            else
            {
                enemy.OnEnemyDead.AddListener((enemyPosition) =>
                {
                    // 랜덤한 아이템을 드랍하도록 설정.
                    float random = Random.Range(0, 1f);
                    if (random < 0.1f)
                    {
                        itemManager.SpawnRandomItem(enemyPosition);
                    }
                });
                
                enemy.OnEnemyDead.AddListener((enemyPosition) =>
                {
                    Debug.Log("Enemy Dead. Position is " + enemyPosition);
                    mapHandler.SetMapObject(enemyPosition, ObjectType.Load);
                    EnemyController enemy = null;
                    bool isGetEnemy = enemies.TryGetValue(enemyPosition, out enemy);
                    Debug.Log("Enemy Dead. Get Enemy: " + isGetEnemy + " enemy: " + enemy);
                    Debug.Log("Enemy Dead. Removing Enemy: " + enemies.Remove(enemyPosition));
                });
            }
        }
        
        public void AddEnemy(Vector2 position, EnemyController enemy)
        {
            Debug.Log("Enemy Add. Position is " + position);
            if(!enemies.TryAdd(position, enemy))
            {
                return;
            }
            mapHandler.SetMapObject(position, ObjectType.Enemy);
        }
        public void AddBoss(Vector2 position, EnemyController enemy)
        {
            Debug.Log("Boss Add. Position is " + position);
            if(!enemies.TryAdd(position, enemy))
            {
                return;
            }
            mapHandler.SetMapObject(position, ObjectType.Boss);
        }
        
        public void PlayerBehavior(Vector2 inputVec, string hitResult = "normal")
        {
            if (!(inputVec.x == 0 && Mathf.Abs(inputVec.y) == 1) && !(inputVec.y == 0 && Mathf.Abs(inputVec.x) == 1))
            {  
                // 잘못된 입력값
                Debug.LogError("입력된 값이 잘못되었습니다. 입력된 값: " + inputVec);
                return;
            }
            
            player.LookAt(inputVec);
           // bool isCrit = false; //
            bool isCrit = hitResult.Equals("Perfect");
                
            if (!TryPlayerAttackAble(inputVec, isCrit))
            {
                // 공격 대상 없음
                player.MovePlayer(inputVec);
            }
            else
            {
                // 공격 성공
                SearchTiles();
            }
        }
        
        private bool EnemyBehavior(Vector2 targetPosition, Vector2 enemyPosition, out bool isAttack)
        {
            EnemyController enemy;
            if (enemies.TryGetValue(enemyPosition, out enemy))
            {
                List<Vector2> enemyDelayedAttackPositions;
                if (enemy.DelayedAttack(out enemyDelayedAttackPositions))
                {
                    bool tempCheckAttack = false;
                    foreach (var enemyDelayedAttackPosition in enemyDelayedAttackPositions)
                    {
                        WeaponScriptableObject enemyEquipWeapon = enemy.GetEquippedWeapon();
                        particleManager.PlayParticle(enemyEquipWeapon.name, enemyDelayedAttackPosition + new Vector2(ConstVariables.tileSizeX / 2,ConstVariables.CharacterHeight), false);
                        attackFocusPool.ReturnAttackFocus(enemyDelayedAttackPosition);
                        
                        if (enemyDelayedAttackPosition.Equals(playerPosition))
                        {
                            player.Attacked(enemy.GetPower());
                            if (!cameraTween.IsPlaying())
                            {
                                cameraTween.Restart();
                            }

                            tempCheckAttack = true;
                        }
                    }
                    enemyDelayedAttackPositions.Clear();
                    
                    isAttack = tempCheckAttack;
                    return !isAttack;
                }
                
                bool attackDelaying, delayAttackDelaying;
                if (enemy.CanAttack(playerPosition, out attackDelaying, out delayAttackDelaying))
                {
                    // 플레이어 공격
                    enemy.Attack();
                    WeaponScriptableObject enemyEquipWeapon = enemy.GetEquippedWeapon();
                    Vector2 enemyLook = targetPosition - enemyPosition;
                    
                    particleManager.PlayParticle(enemyEquipWeapon.name, player.unitRoot.position + new Vector3(0,ConstVariables.CharacterHeight, 0), false);
                    player.Attacked(enemy.GetPower());
                    if (!cameraTween.IsPlaying())
                    {
                        cameraTween.Restart();
                    }

                    isAttack = true;
                    return false;
                }

                if (delayAttackDelaying)
                {
                    isAttack = true;
                    return false;
                }
                
                if (MoveEnemy(targetPosition, enemyPosition))
                {
                    isAttack = false || attackDelaying;
                    return true;
                }
                
                if (attackDelaying)
                {
                    isAttack = true;
                    return false;
                }
            }
            else
            {
                Debug.LogError("There is No Enemy in position " + enemyPosition);
            }
            isAttack = false;
            return false;
        }

        private bool BossBehavior(Vector2 bossPosition)
        {
            EnemyController enemy;
            if (enemies.TryGetValue(bossPosition, out enemy))
            {
                if (enemy as BossController)
                {
                    return (enemy as BossController).BossBehaviorAble(playerPosition);
                }
            }
            else
            {
                Debug.LogError("There is No Enemy: " + bossPosition);
            }
            return false;
        }
        
        /// <summary>
        /// 플레이어의 입력 방향에 적이 있으면 공격을 시도함.
        /// </summary>
        /// <param name="inputVec">플레이어의 입력 방향. 반드시 x, y 둘 중 하나만 절대값이 1이여야 함</param>
        /// <returns>적을 공격한 경우 true, 공격하지 못한경우 false 반환</returns>
        private bool TryPlayerAttackAble(Vector2 inputVec, bool isCrit)
        {
            bool ret = false;
            WeaponScriptableObject weapon = player.GetEquippedWeapon();
            EnemyController attackTarget = null;
            WeaponScriptableObject playerEquipWeapon = player.GetEquippedWeapon();
            
            switch (weapon.attackDirection)
            {
                case AttackDirection.DIR_4:
                    for (int i = 1; i <= weapon.range; i++)
                    {
                        EnemyController enemy;
                        if (enemies.TryGetValue(playerPosition + inputVec * i, out enemy))
                        {
                            ret = true;
                            enemy.Attacked(player.GetPower());
                            attackTarget = enemy;
                            if (!weapon.isSplash)
                            {
                                break;
                            }
                            particleManager.PlayParticle(playerEquipWeapon.name,  enemy.unitRoot.position + new Vector3(0,ConstVariables.CharacterHeight, 0), isCrit);
                        }
                    }
                    break;
                case AttackDirection.DIR_8:
                    for (int i = 1; i <= weapon.range; i++)
                    {
                        for (int j = -i; j <= i; j++)
                        {
                            EnemyController enemy;
                            Vector2 targetPos = playerPosition + inputVec * i;
                            if (inputVec.x != 0)
                            {
                                targetPos.y += j;
                            }
                            else if (inputVec.y != 0)
                            {
                                targetPos.x += j;
                            } 
                            if (enemies.TryGetValue(targetPos, out enemy))
                            {
                                ret = true;
                                enemy.Attacked(player.GetPower());
                                attackTarget = enemy;
                                if (!weapon.isSplash)
                                {
                                    break;
                                }
                                particleManager.PlayParticle(playerEquipWeapon.name,  enemy.unitRoot.position + new Vector3(0,ConstVariables.CharacterHeight, 0), isCrit);
                            }
                        }
                    }
                    break;
            }

            if (ret)
            {
                player.Attack();
                if (playerEquipWeapon.isSplash)
                {
                }
                else if (attackTarget != null)
                {
                    particleManager.PlayParticle(playerEquipWeapon.name, attackTarget.unitRoot.position + new Vector3(0,ConstVariables.CharacterHeight, 0), isCrit);
                }
            }

            return ret;
        }
        
        public void SearchTiles()
        {
            Debug.Log("Call Search Tiles");
            dpClass dp = new dpClass(ConstVariables.maxDetactRange, playerPosition);
            Queue<TileInfo> queue = new Queue<TileInfo>();
            queue.Enqueue(new TileInfo(playerPosition, 0));

            while (queue.Count > 0)
            {
                TileInfo tileInfo = queue.Dequeue();

                if (tileInfo.depth == ConstVariables.maxDetactRange)
                {
                    continue;
                }
                
                for (int i = 0; i < 4; i++)
                {
                    int nX = (int)tileInfo.position.x + ConstVariables.dX[i];
                    int nY = (int)tileInfo.position.y + ConstVariables.dY[i];

                    if (dp[nX, nY] < tileInfo.depth + 1)
                    {
                        continue;
                    }
                
                    if (mapHandler.IsInsideMap(nX, nY))
                    {
                        ObjectType tileObject = mapHandler.GetPoint(nX, nY);
                        switch (tileObject)
                        {
                            case ObjectType.Block:
                                continue;
                                break;
                            case ObjectType.Load:
                                if (dp[nX, nY] > tileInfo.depth + 1)
                                {
                                    dp[nX, nY] = tileInfo.depth + 1;
                                    queue.Enqueue(new TileInfo(new Vector2(nX, nY), tileInfo.depth + 1));
                                }
                                break;
                            case ObjectType.Boss:
                                if (dp[nX, nY] <= tileInfo.depth + 1)
                                {
                                    break;
                                }
                                if (BossBehavior(new Vector2(nX, nY)))
                                {
                                    bool isAttack;
                                    if (EnemyBehavior(new Vector2((int)tileInfo.position.x, (int)tileInfo.position.y),
                                            new Vector2(nX, nY), out isAttack))
                                    {
                                        dp[nX, nY] = tileInfo.depth + 1;
                                        queue.Enqueue(new TileInfo(new Vector2(nX, nY), tileInfo.depth + 1));
                                    }
                                    else if (isAttack)
                                    {
                                        dp[nX, nY] = tileInfo.depth + 1;
                                    }
                                }
                                else
                                {
                                    dp[nX, nY] = 0;
                                    for (int j = 0; j < 4; j++)
                                    {
                                        int nnX = nX + ConstVariables.dX[j];
                                        int nnY = nY + ConstVariables.dY[j];
                                        
                                        ObjectType nextTileObject = mapHandler.GetPoint(nnX, nnY);
                                        if (nextTileObject.Equals(ObjectType.Boss))
                                        {
                                            dp[nnX, nnY] = 0;
                                        }
                                    }
                                }
                                break;
                            case ObjectType.Enemy:
                                if (dp[nX, nY] > tileInfo.depth + 1)
                                {
                                    bool isAttack;
                                    if (EnemyBehavior(new Vector2((int)tileInfo.position.x, (int)tileInfo.position.y),
                                            new Vector2(nX, nY), out isAttack))
                                    {
                                        dp[nX, nY] = tileInfo.depth + 1;
                                        queue.Enqueue(new TileInfo(new Vector2(nX, nY), tileInfo.depth + 1));
                                    }
                                    else if (isAttack)
                                    {
                                        dp[nX, nY] = tileInfo.depth + 1;
                                    }
                                }
                                break;
                        }
                    }
                }
            }
            
            
        }

       
        
        #region Inner Class
        
        private class dpClass
        {
            private int[,] dp;
            private int offsetX;
            private int offsetY;

            public dpClass(int width, int height, int offsetX, int offsetY)
            {
                dp = new int[width * 2 + 1, height * 2 + 1];
                this.offsetX = offsetX;
                this.offsetY = offsetY;

                // 초기값 설정
                for (int i = 0; i < width * 2 + 1; i++)
                {
                    for (int j = 0; j < height * 2 + 1; j++)
                    {
                        dp[i,j] = int.MaxValue;
                    }
                }
            }

            public dpClass(int detectRange, Vector2 playerPos)
            {
                dp = new int [detectRange * 2 + 3, detectRange * 2 + 3];
                this.offsetX = (int)playerPos.x - detectRange - 1;
                this.offsetY = (int)playerPos.y - detectRange - 1;
                
                
                // 초기값 설정
                for (int i = 0; i < detectRange  * 2 + 1; i++)
                {
                    for (int j = 0; j < detectRange * 2 + 1; j++)
                    {
                        dp[i,j] = int.MaxValue;
                    }
                }
                dp[detectRange, detectRange] = 0;
            }
            
            public int this[int row, int column]
            {
                get
                {
                    return dp[row - offsetX, column - offsetY];
                }
                set
                {
                    dp[row - offsetX, column - offsetY] = value;
                }
            }
        }

        private struct TileInfo
        {
            public Vector2 position;
            public int depth;

            public TileInfo(Vector2 position, int depth)
            {
                this.position = position;
                this.depth = depth;
            }
        }

        #endregion

        #region Odin

        private bool IsPlayerNotSetup()
        {
            return player == null;
        }

        #endregion
    }
}
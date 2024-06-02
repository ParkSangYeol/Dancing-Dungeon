#define TEST_MOVE_WITHOUT_NOTE
using System;
using System.Collections.Generic;
using CombatScene.Enemy;
using CombatScene.Player;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

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
            
            if (!TryPlayerAttackAble(inputVec))
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
        private MapHandler mapHandler;

        [InfoBox("플레이어를 넣어주세요!", InfoMessageType.Error, "IsPlayerNotSetup")]
        [SerializeField]
        private PlayerController player;
        private Vector2 playerPosition;
        private Dictionary<Vector2, EnemyController> enemies = new Dictionary<Vector2, EnemyController>();
        
        private void Awake()
        {
            if (mapHandler == null)
            {
                mapHandler = GetComponent<MapHandler>();
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

        public void MovePlayer(Vector2 targetPosition)
        {
            if (mapHandler.SetMapObject(targetPosition, ObjectType.Player))
            {
                mapHandler.SetMapObject(playerPosition, ObjectType.Load);
                playerPosition = targetPosition;
            }
            SearchTiles();
        }
        
        public void MoveEnemy(Vector2 targetPosition, Vector2 enemyPosition)
        {
            ObjectType tileObject = mapHandler.GetPoint(targetPosition);
            
            if (tileObject.Equals(ObjectType.Player) || tileObject.Equals(ObjectType.Enemy))
            {
                // 플레이어나 적 위치로는 이동 불가
                return;
            }
            
            if (mapHandler.SetMapObject(targetPosition, ObjectType.Enemy))
            {
                mapHandler.SetMapObject(enemyPosition, ObjectType.Load);
                EnemyController enemy = enemies[enemyPosition];
                enemy.MoveCharacter(targetPosition);
                
                enemies.Remove(enemyPosition);
                enemies.Add(targetPosition, enemy);
            }
        }

        public void AddEnemy(Vector2 position, EnemyController enemy)
        {
            enemies.Add(position, enemy);
            mapHandler.SetMapObject(position, ObjectType.Enemy);
        }

        private void PlayerBehavior(Vector2 inputVec)
        {
            if (!(inputVec.x == 0 && Mathf.Abs(inputVec.y) == 1) && !(inputVec.y == 0 && Mathf.Abs(inputVec.x) == 1))
            {  
                // 잘못된 입력값
                Debug.LogError("입력된 값이 잘못되었습니다. 입력된 값: " + inputVec);
                return;
            }
            
            player.LookAt(inputVec);
            
            if (!TryPlayerAttackAble(inputVec))
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
        
        private void EnemyBehavior(Vector2 targetPosition, Vector2 enemyPosition)
        {
            EnemyController enemy = enemies[enemyPosition];
            if (enemy.CanAttack(playerPosition))
            {
                // 플레이어 공격
                enemy.Attack();
                player.Attacked(enemy.GetPower());
            }
            else
            {
                MoveEnemy(targetPosition, enemyPosition);
            }
        }

        /// <summary>
        /// 플레이어의 입력 방향에 적이 있으면 공격을 시도함.
        /// </summary>
        /// <param name="inputVec">플레이어의 입력 방향. 반드시 x, y 둘 중 하나만 절대값이 1이여야 함</param>
        /// <returns>적을 공격한 경우 true, 공격하지 못한경우 false 반환</returns>
        private bool TryPlayerAttackAble(Vector2 inputVec)
        {
            bool ret = false;
            WeaponScriptableObject weapon = player.GetEquippedWeapon();
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
                            if (!weapon.isSplash)
                            {
                                break;
                            }
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
                                if (!weapon.isSplash)
                                {
                                    break;
                                }
                            }
                        }
                    }
                    break;
            }

            if (ret)
            {
                player.Attack();
            }

            return ret;
        }
        
        public void SearchTiles()
        {
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

                    if (dp[nX, nY] <= tileInfo.depth + 1)
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
                            case ObjectType.Enemy:
                                if (dp[nX, nY] > tileInfo.depth + 1)
                                {
                                    dp[nX, nY] = tileInfo.depth + 1;
                                    EnemyBehavior(new Vector2((int)tileInfo.position.x, (int)tileInfo.position.y), new Vector2(nX, nY));
                                    queue.Enqueue(new TileInfo(new Vector2(nX, nY), tileInfo.depth + 1));
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
                dp = new int [detectRange * 2 + 1, detectRange * 2 + 1];
                this.offsetX = (int)playerPos.x - detectRange;
                this.offsetY = (int)playerPos.y - detectRange;
                
                
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
using System;
using System.Collections.Generic;
using CombatScene.Enemy;
using CombatScene.Player;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CombatScene
{
    public class CombatManager : MonoBehaviour
    {
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

            playerPosition = player.transform.position;
        }

        public void MovePlayer(Vector2 targetPosition)
        {
            if (mapHandler.SetMapObject(targetPosition, ObjectType.Player))
            {
                mapHandler.SetMapObject(playerPosition, ObjectType.Load);
                playerPosition = targetPosition;
                SearchTiles();
            }
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

        private void PlayerBehavior()
        {
                
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
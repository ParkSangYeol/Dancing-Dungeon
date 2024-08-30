using System.Collections.Generic;
using Priority_Queue;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

namespace CombatScene.Enemy
{
    public class BossController : EnemyController
    {
        [SerializeField] 
        private Collider2D bossArea;
        
        [SerializeField]
        private Vector2 defaultPosition;

        [SerializeField] 
        private HPBarHandler hpBarHandler;
        
        private void Start()
        {
            base.Start();
            defaultPosition = this.transform.position;
            combatManager.AddBoss(this.transform.position, this);
            if (hpBarHandler == null)
            {
                hpBarHandler = GetComponentInChildren<HPBarHandler>();
            }

            if (hpBarHandler != null)
            {
                onHitEvent.AddListener((currnetHP) =>
                {
                    hpBarHandler.SetHPUI(currnetHP, characterData.hp);
                });
            }
        }
        
        
        /// <summary>
        /// 보스의 행동 가능 여부를 반환
        /// </summary>
        /// <param name="playerPosition"></param>
        /// <returns>보스가 공격 혹은 이동을 할 수 있는 경우 True, 아닐 경우 False 반환</returns>
        public bool BossBehaviorAble(Vector2 playerPosition)
        {
            if (!bossArea.OverlapPoint(playerPosition))
            {
                // 보스를 초기 위치로 이동시키기
                ReturnBossToDefaultPosition();
                CancelDelayedAttack();
                return false;
            }
            // 보스 체력 UI 표시
            // TODO

            Debug.Log("Player가 보스 영역으로 들어왔습니다. 보스 이름: " + gameObject.name);
            return true;
        }

        private void ReturnBossToDefaultPosition()
        {
            // A* 알고리즘 사용
            Dictionary<Vector2, Vector2> parents = new Dictionary<Vector2, Vector2>();
            SearchTileNode startNode = new SearchTileNode(this.transform.position);
            Vector2 areaVec = bossArea.bounds.size;
            FastPriorityQueue<SearchTileNode> pq = new FastPriorityQueue<SearchTileNode>((int)((areaVec.x + 1) * (areaVec.y + 1)));
            parents.Add(startNode.position, startNode.position);
            SearchTileNode current = startNode;
            pq.Enqueue(startNode, Mathf.Abs(startNode.position.x - defaultPosition.x) + Mathf.Abs(startNode.position.y - defaultPosition.y));

            while (pq.Count != 0)
            {
                current = pq.Dequeue();
                if (current.position.Equals(defaultPosition))
                {
                    // 초기 위치 확인
                    break;
                }
                for (int i = 0; i < 4; i++)
                {
                    int nextX = (int)current.position.x + ConstVariables.dX[i];
                    int nextY = (int)current.position.y + ConstVariables.dY[i];

                    if (combatManager.mapHandler.GetPoint(nextX, nextY).Equals(ObjectType.Load) && bossArea.OverlapPoint(new Vector2(nextX, nextY) )&&!parents.ContainsKey(new Vector2(nextX, nextY)))
                    {
                        SearchTileNode nextNode = new SearchTileNode(new Vector2(nextX, nextY));
                        nextNode.cost = current.cost + 1;
                        float nextPriority = Mathf.Abs(current.position.x - defaultPosition.x) +
                                             Mathf.Abs(current.position.y - defaultPosition.y) + current.cost + 1;
                        parents.Add(new Vector2(nextX, nextY), current.position);
                        pq.Enqueue(nextNode, nextPriority);
                    }
                }
            }
            
            Vector2 movePos = current.position;
            while (!parents[movePos].Equals(parents[parents[movePos]]))
            {
                movePos = parents[movePos];
            }
            combatManager.MoveEnemy(movePos, this.transform.position);
        }
    }

    class SearchTileNode : FastPriorityQueueNode
    {
        public Vector2 position { get; private set; }
        public int cost = 0;

        public SearchTileNode(Vector2 position)
        {
            this.position = position;
        }
    }
}
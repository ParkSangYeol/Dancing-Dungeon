using System.Collections;
using UnityEngine;

namespace CombatScene.Enemy
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField]
        private CombatManager combatManager;
        [SerializeField]
        private Animator animator;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            if (combatManager == null)
            {
                combatManager = transform.Find("GameManager").GetComponent<CombatManager>();
            }
            
            combatManager.AddEnemy(transform.position, this);
            
            if (animator == null)
            {
                animator = transform.GetChild(0).GetComponent<Animator>();
            }
            
        }

        public void MoveCharacter(Vector2 targetPosition)
        {
            StartCoroutine(MoveTo(targetPosition, 0.4f));
        }
        
        /// <summary>
        /// 특정한 타겟으로 플레이어를 일정시간동안 이동시키는 코루틴
        /// </summary>
        /// <param name="addPos"> 이동시킬 타겟 위치(현재 위치에서 더해주는 값)</param>
        /// <param name="duration"> 이동 시간(초)</param>
        /// <returns></returns>
        IEnumerator MoveTo(Vector2 targetPosition, float duration)
        {
            // 플레이어 이동 애니메이션 재생
            animator.SetTrigger("Move");
            // 이동
            Vector2 startPos = transform.position;
            Vector2 targetPos = targetPosition;
            float time = 0;
            while (duration > time)
            {
                transform.position = Vector2.Lerp(startPos, targetPos, time / duration);
                time += Time.deltaTime;
                yield return null;
            }
            transform.position = targetPos;
        }
    }
}
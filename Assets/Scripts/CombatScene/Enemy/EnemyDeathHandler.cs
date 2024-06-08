using UnityEngine;

namespace CombatScene.Enemy
{
    public class EnemyDeathHandler : MonoBehaviour
    {
        
        public void DestroyCharacter()
        {
            // 애니메이션에서 이벤트로 호출
            StopAllCoroutines();
            gameObject.transform.parent.gameObject.SetActive(false);
        }

    }
}

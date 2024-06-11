using UnityEngine;

namespace CombatScene.Enemy
{
    public class EnemyDeathHandler : MonoBehaviour
    {
        
        public void DestroyCharacter()
        {
            StopAllCoroutines();
            gameObject.transform.parent.gameObject.SetActive(false);
        }

    }
}

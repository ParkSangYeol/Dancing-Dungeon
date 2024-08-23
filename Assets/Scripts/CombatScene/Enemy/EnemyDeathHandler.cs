using System;
using UnityEngine;

namespace CombatScene.Enemy
{
    public class EnemyDeathHandler : MonoBehaviour
    {
        private CombatSceneUIManager _combatSceneUIManager;

        public void Start()
        {
            _combatSceneUIManager = GameObject.Find("CombatUiManager").GetComponent<CombatSceneUIManager>();
        }

        public void DestroyCharacter()
        {
            if (gameObject.CompareTag("Boss"))
            {
                _combatSceneUIManager.DieBossCount();
            }
            else
            {
               _combatSceneUIManager.DieEnemyCount(); 
            }
            
            StopAllCoroutines();
            gameObject.transform.parent.gameObject.SetActive(false);
            
        }

    }
}

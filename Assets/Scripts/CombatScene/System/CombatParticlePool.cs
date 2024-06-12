using System.Collections.Generic;
using CombatScene.System.Particle;
using Unity.VisualScripting;
using UnityEngine;

namespace CombatScene.System.Particle
{
   public class CombatParticlePool
    {
        private List<CombatParticle> combatParticles;
        private int idx;
        private ParticleSystem VFX;
        private Vector3 poolPosition;
        private ParticleManager particleManager;
        
        public CombatParticlePool(ParticleSystem vfxPrefab, Vector3 poolPosition, ParticleManager particleManager, int spawnCount = 1)
        {
            combatParticles = new List<CombatParticle>();
            idx = 0;
            VFX = vfxPrefab;
            this.poolPosition = poolPosition;
            this.particleManager = particleManager;

            for (int i = 0; i < spawnCount; i++)
            {
                SpawnParticle(true);
            }
        }
    
        public CombatParticle GetCombatParticle()
        {
            CombatParticle combatParticle = combatParticles[idx++];
            idx %= combatParticles.Count;

            if (combatParticle.gameObject.activeInHierarchy)
            {
                // 임시 오브젝트 생성
                return SpawnParticle(false);
            }

            return combatParticle;
        }
    
        private CombatParticle SpawnParticle(bool save)
        {
            ParticleSystem particleSystem = particleManager.InstantiateParticle(VFX);
            particleSystem.GetComponent<Renderer>().sortingLayerName = "UpOfCharacter";
            particleSystem.transform.position = poolPosition;
            particleSystem.gameObject.layer = LayerMask.NameToLayer("CombatVFX");
            particleSystem.gameObject.SetActive(false);

            CombatParticle combatParticle = particleSystem.AddComponent<CombatParticle>();
            combatParticle.combatParticlePool = this;
            if (save)
            {
                combatParticles.Add(combatParticle);
                combatParticle.isSave = true;
            }
            else
            {
                combatParticle.isSave = false;
            }

            return combatParticle;
        }
        
        public void ReturnToPool(CombatParticle combatParticle)
        {
            if (combatParticle.isSave)
            {
                Debug.Log("Return To Pool " + combatParticle);
                combatParticle.gameObject.SetActive(false);
                combatParticle.transform.position = poolPosition;
                combatParticle.particleSystem.Stop();
            }
            else
            {
                particleManager.DestroyParticle(combatParticle.gameObject);
            }
        }
    }
}
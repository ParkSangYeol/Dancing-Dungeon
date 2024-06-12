using System;
using System.Collections.Generic;
using Coffee.UIExtensions;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

namespace CombatScene.System.Particle
{
    public class ParticleManager : MonoBehaviour
    {
        private Dictionary<string, CombatParticlePool> particlePools = new Dictionary<string, CombatParticlePool>();

        [SerializeField] 
        private List<WeaponScriptableObject> weaponList;

        [SerializeField] 
        private Vector3 poolPosition;

        [SerializeField] 
        [AssetsOnly] 
        private AudioClip normalAtkSound;
        [SerializeField] 
        [AssetsOnly] 
        private AudioClip criticalAtkSound;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            foreach (var weapon in weaponList)
            {
                particlePools.Add(weapon.name ,new CombatParticlePool(weapon.VFX, poolPosition, this, 3));
            }
        }

        /// <summary>
        /// 특정 위치에 무기 이름에 해당하는 파티클 생성
        /// </summary>
        /// <param name="weaponName">생성할 파티클의 무기 이름</param>
        /// <param name="spawnPosition">파티클을 생성할 위치</param>
        /// <param name="isCrit">크리티컬 여부</param>
        public void PlayParticle(string weaponName, Vector3 spawnPosition, bool isCrit = false)
        {
            CombatParticlePool combatParticlePool;
            if (particlePools.TryGetValue(weaponName, out combatParticlePool))
            {
                CombatParticle combatParticle = combatParticlePool.GetCombatParticle();
                combatParticle.transform.position = spawnPosition;
                combatParticle.gameObject.SetActive(true);
                combatParticle.particleSystem.Play();
                if (isCrit)
                {
                    combatParticle.PlaySFX(criticalAtkSound);
                }
                else
                {
                    combatParticle.PlaySFX(normalAtkSound);
                }
            }
        }

        public ParticleSystem InstantiateParticle(ParticleSystem VFX)
        {
            return Instantiate(VFX, transform);
        }
        
        public void DestroyParticle(GameObject particle)
        {
            Destroy(particle);
        }
    }
}
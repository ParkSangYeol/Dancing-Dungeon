using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CombatScene.System.Particle
{
    public class ParticleManager : MonoBehaviour
    {
        private Dictionary<string, CombatParticlePool> particlePools = new Dictionary<string, CombatParticlePool>();

        [SerializeField] 
        private List<WeaponScriptableObject> weaponList;
        
        [SerializeField]
        [AssetsOnly]
        private ParticleSystem itemParticlePrefab;
        private ItemParticle itemParticle;

        [SerializeField] 
        private Vector3 poolPosition;

        [SerializeField] 
        [AssetsOnly] 
        private AudioClip normalAtkSound;
        [SerializeField] 
        [AssetsOnly] 
        private AudioClip criticalAtkSound;
        [SerializeField] 
        [AssetsOnly] 
        private AudioClip itemInteractSound;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            foreach (var weapon in weaponList)
            {
                particlePools.Add(weapon.name ,new CombatParticlePool(weapon.VFX, poolPosition, this, 3));
            }

            SetItemParticle();
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

        public void PlayItemInteractParticle(Vector3 targetPosition)
        {
            targetPosition.x += ConstVariables.tileSizeX / 2;
            targetPosition.y += ConstVariables.tileSizeY / 2;
            itemParticle.transform.position = targetPosition;
            itemParticle.gameObject.SetActive(true);
            itemParticle.PlayParticle();
        }

        public ParticleSystem InstantiateParticle(ParticleSystem VFX)
        {
            return Instantiate(VFX, transform);
        }

        public void SetItemParticle()
        {
            ParticleSystem particle = InstantiateParticle(itemParticlePrefab);
            particle.gameObject.SetActive(false);
            particle.transform.position = poolPosition;
            itemParticle = particle.GetComponent<ItemParticle>();
            itemParticle.audioSource.clip = itemInteractSound;
        }
        
        public void DestroyParticle(GameObject particle)
        {
            Destroy(particle);
        }
    }
}
using System;
using System.Collections.Generic;
using Coffee.UIExtensions;
using Unity.VisualScripting;
using UnityEngine;

namespace CombatScene.System.Particle
{
    public class ParticleManager : MonoBehaviour
    {
        private Dictionary<string, ParticleSystem> particlePools = new Dictionary<string, ParticleSystem>();

        [SerializeField] 
        private List<WeaponScriptableObject> weaponList;

        [SerializeField] 
        private Vector3 poolPosition;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            foreach (var weapon in weaponList)
            {
                ParticleSystem particleSystem = Instantiate(weapon.VFX, transform);
                particleSystem.GetComponent<Renderer>().sortingLayerName = "UpOfCharacter";
                particleSystem.transform.position = poolPosition;
                particleSystem.gameObject.layer = LayerMask.NameToLayer("CombatVFX");
                particleSystem.gameObject.SetActive(false);

                CombatParticle combatParticle = particleSystem.AddComponent<CombatParticle>();
                combatParticle.particlePoolManager = this;
                particlePools.Add(weapon.name, particleSystem);
            }
        }

        /// <summary>
        /// 특정 위치에 무기 이름에 해당하는 파티클 생성
        /// </summary>
        /// <param name="weaponName">생성할 파티클의 무기 이름</param>
        /// <param name="spawnPosition">파티클을 생성할 위치</param>
        /// <param name="rotation">플레이어의 입력 방향</param>
        public void PlayParticle(string weaponName, Vector3 spawnPosition, Vector2 inputVec)
        {
            Debug.Log("Start Play Particle");
            ParticleSystem particleSystem;
            if (particlePools.TryGetValue(weaponName, out particleSystem))
            {
                Debug.Log("Find Particle + " + particleSystem);
                particleSystem.transform.position = spawnPosition;
                particleSystem.gameObject.SetActive(true);
                particleSystem.Play();
            }
        }

        public void ReturnToPool(ParticleSystem particleSystem)
        {
            Debug.Log("Return To Pool " + particleSystem);
            particleSystem.gameObject.SetActive(false);
            particleSystem.transform.position = poolPosition;
            particleSystem.Stop();
        } 
    }
}
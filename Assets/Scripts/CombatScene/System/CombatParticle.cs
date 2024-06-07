using UnityEngine;

namespace CombatScene.System.Particle
{
    public class CombatParticle : MonoBehaviour
    {
        private ParticleSystem particleSystem;
        public ParticleManager particlePoolManager;

        private void Start()
        {
            particleSystem = GetComponent<ParticleSystem>();
        }

        private void OnParticleSystemStopped()
        {
            particlePoolManager.ReturnToPool(particleSystem);
        }
    }

    
}
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CombatScene.System.Particle
{
    public class CombatParticle : MonoBehaviour
    {
        public ParticleSystem particleSystem { get; private set; }

        public AudioSource audioSource { get; private set; }

        public CombatParticlePool combatParticlePool;

        public bool isSave;

        private void Awake()
        {
            particleSystem = GetComponent<ParticleSystem>();
            audioSource = GetComponent<AudioSource>();
        }
        
        private void ReturnParticleSystem()
        {
            combatParticlePool.ReturnToPool(this);
        }
        
        public void PlaySFX(AudioClip audioClip)
        {
            if (audioSource == null) 
                return;
            audioSource.pitch = Random.Range(0.8f, 1.2f);
            audioSource.PlayOneShot(audioClip);
            Invoke("ReturnParticleSystem", particleSystem.main.duration);
        }
    }

    
}
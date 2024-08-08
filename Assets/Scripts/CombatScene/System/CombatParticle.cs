using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CombatScene.System.Particle
{
    public class CombatParticle : MonoBehaviour
    {
        public ParticleSystem particleSystem { get; private set; }

        private SFXPlayer sfxPlayer;

        public CombatParticlePool combatParticlePool;

        public bool isSave;

        private void Awake()
        {
            particleSystem = GetComponent<ParticleSystem>();
            sfxPlayer = transform.GetComponentInChildren<SFXPlayer>();
        }
        
        private void ReturnParticleSystem()
        {
            combatParticlePool.ReturnToPool(this);
        }
        
        public void PlaySFX(AudioClip audioClip)
        {
            if (sfxPlayer == null) 
                return;
            sfxPlayer.SetAudioClip(audioClip);
            sfxPlayer.PlayWithRandomPitch(new Vector2(0.9f, 1.1f));
            Invoke("ReturnParticleSystem", particleSystem.main.duration);
        }
    }

    
}
using UnityEngine;

namespace CombatScene
{
    public class ItemParticle : MonoBehaviour
    {
        private ParticleSystem particleSystem;
        public AudioSource audioSource;
        
        private void Awake()
        {
            particleSystem = GetComponent<ParticleSystem>();
            audioSource = GetComponent<AudioSource>();
        }
        
        public void PlayParticle()
        {
            particleSystem.Play();
            if (audioSource == null) 
                return;
            audioSource.pitch = Random.Range(0.8f, 1.2f);
            audioSource.PlayOneShot(audioSource.clip);
        }
    }
}
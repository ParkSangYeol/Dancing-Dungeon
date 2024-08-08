using UnityEngine;

namespace CombatScene
{
    public class ItemParticle : MonoBehaviour
    {
        private ParticleSystem particleSystem;
        private SFXPlayer sfxPlayer;
        
        private void Awake()
        {
            particleSystem = GetComponent<ParticleSystem>();
            sfxPlayer = transform.GetComponentInChildren<SFXPlayer>();
        }
        
        public void PlayParticle()
        {
            particleSystem.Play();
            if (sfxPlayer == null) 
                return;
            sfxPlayer.PlayWithRandomPitch(new Vector2(0.9f, 1.1f));
        }

        public void SetAudioClip(AudioClip audioClip)
        {
            sfxPlayer.SetAudioClip(audioClip);
        }
    }
}
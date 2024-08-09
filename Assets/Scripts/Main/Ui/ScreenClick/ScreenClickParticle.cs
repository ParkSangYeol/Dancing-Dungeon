using Coffee.UIExtensions;
using UnityEngine;
using UnityEngine.UI.Extensions;

namespace Main.UI
{
    public class ScreenClickParticle : MonoBehaviour
    {
        public UIParticle particleSystem { private set; get; }
        public ScreenClickParticlePool screenClickParticlePool;
        public bool isSave;

        private void Awake()
        {
            // Component 설정
            particleSystem = GetComponent<UIParticle>();
        }
    
        private void ReturnParticleSystem()
        {
            screenClickParticlePool.ReturnToPool(this);
        }
    
        public void PlayParticle()
        {
            this.gameObject.SetActive(true);
            particleSystem.Play();
            Invoke("ReturnParticleSystem", particleSystem.particles[0].main.duration);
        }
    }
}

using System.Collections.Generic;
using Coffee.UIExtensions;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

namespace Main.UI
{
    public class ScreenClickParticlePool
    {
        private List<ScreenClickParticle> screenClickParticles;
        private int idx;
        private UIParticle VFXTemplate;
        private Vector3 poolDefaultPosition;
        private ScreenClickHandler screenClickHandler;

        public ScreenClickParticlePool(UIParticle vfxTemplate, Vector3 defaultPosition, ScreenClickHandler screenClickHandler, int spawnCount = 1)
        {
            screenClickParticles = new List<ScreenClickParticle>();
            idx = 0;
            VFXTemplate = vfxTemplate;
            this.poolDefaultPosition = poolDefaultPosition;
            this.screenClickHandler = screenClickHandler;

            for (int i = 0; i < spawnCount; i++)
            {
                SpawnParticle(true);
            }
        }
        
        public ScreenClickParticle GetScreenClickParticle()
        {
            ScreenClickParticle screenClickParticle = screenClickParticles[idx++];
            idx %= screenClickParticles.Count;

            if (screenClickParticle.gameObject.activeInHierarchy)
            {
                // 임시 오브젝트 생성
                return SpawnParticle(false);
            }

            return screenClickParticle;
        }
        
        private ScreenClickParticle SpawnParticle(bool save)
        {
            UIParticle particleSystem = screenClickHandler.InstantiateParticle(VFXTemplate);
            particleSystem.transform.position = poolDefaultPosition;

            ScreenClickParticle screenClickParticle = particleSystem.AddComponent<ScreenClickParticle>();
            screenClickParticle.screenClickParticlePool = this;
            
            if (save)
            {
                screenClickParticles.Add(screenClickParticle);
                screenClickParticle.isSave = true;
            }
            else
            {
                screenClickParticle.isSave = false;
            }

            particleSystem.gameObject.SetActive(false);
            
            return screenClickParticle;
        }
        
        public void ReturnToPool(ScreenClickParticle screenClickParticle)
        {
            if (screenClickParticle.isSave)
            {
                screenClickParticle.gameObject.SetActive(false);
                screenClickParticle.transform.position = poolDefaultPosition;
                screenClickParticle.particleSystem.Stop();
            }
            else
            {
                screenClickHandler.DestroyParticle(screenClickParticle.gameObject);
            }
        }
    }
}
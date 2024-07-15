using System;
using UnityEngine;

namespace Main.UI
{
    public class SetSoundBarAnimation : MonoBehaviour
    {
        [SerializeField] 
        private Animator animator;
        private static readonly int Blend = Animator.StringToHash("Blend");
        [SerializeField] 
        private Animator noteAnimator;
        private static readonly int NoteEnable = Animator.StringToHash("NoteEnable");
        
        [SerializeField]
        private AudioSource audioSource;
        private float[] samples = new float[128];

        private float GetAudioLevel()
        {
            audioSource.GetOutputData(samples, 0);
            float sum = 0f;
        
            for (int i = 0; i < samples.Length; i++)
            {
                sum += Mathf.Abs(samples[i]);
            }
        
            return Mathf.Clamp01(Mathf.Clamp01(sum / samples.Length) * 2);
        }
        private void SetSoundBarAnimatorFloat(float value)
        {
            animator.SetFloat(Blend, value);
        }
        private void SetSoundBarAnimatorFloat()
        {
            animator.SetFloat(Blend, GetAudioLevel());
        }
        
        private void SetComponents()
        {
            if (animator == null)
            {
                Debug.LogError(this.gameObject.name + "의 Animator값이 설정되어 있지 않습니다.");
            }
            if (audioSource == null)
            {
                Debug.LogError(this.gameObject.name + "의 AudioSource값이 설정되어 있지 않습니다.");
            }
        }

        public void SetNoteParticleBlend(float value)
        {
            if (value != 0f)
            {
                noteAnimator.SetBool(NoteEnable, true); 
            }
            else
            {
                noteAnimator.SetBool(NoteEnable, false); 
            }
        }
        
        private void Start()
        {
            SetComponents();
            InvokeRepeating(nameof(SetSoundBarAnimatorFloat), 0, 0.1f);
        }
    }
}

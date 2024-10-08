using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Main.UI
{
    public class SoundBarManager : MonoBehaviour
    {
        [SerializeField] 
        private Transform handle;

        [SerializeField] 
        private SoundType soundType;
        private Slider slider;
        
        private BarState currentState;
        private BarState willState;

        private void Awake()
        {
            slider = this.GetComponent<Slider>();
        }

        public void Start()
        {
            if (slider != null)
            {
                UnityAction<float> func = soundType switch
                {
                    SoundType.MASTER => SoundManager.Instance.ChangeAllVolume,
                    SoundType.BGM => SoundManager.Instance.ChangeBGMVolume,
                    SoundType.SFX => SoundManager.Instance.ChangeSFXVolume,
                    _ => null
                };
                if (func != null)
                {
                    slider.onValueChanged.AddListener(func);
                }
            }
        }

        public void OnPointerEnter()
        {
            willState = BarState.Hover;
            if (currentState.Equals(BarState.Click))
            {
                return;
            }

            ChangeBarState(BarState.Hover);
        }
        
        public void OnPointerExit()
        {
            willState = BarState.Normal;
            if (currentState.Equals(BarState.Click))
            {
                return;
            }

            ChangeBarState(BarState.Normal);
        }

        public void OnPointerDown()
        {
            ChangeBarState(BarState.Click);
        }

        public void OnPointerUp()
        {
            ChangeBarState(willState);
        }

        private void ChangeBarState(BarState newState)
        {
            if (currentState.Equals(newState))
            {
                return;
            }

            switch (newState)
            {
                case BarState.Normal:
                    handle.DOScale(1, 0.2f);
                    break;
                case BarState.Hover:
                    handle.DOScale(1.2f, 0.2f);
                    break;
                case BarState.Click:
                    handle.DOScale(0.9f, 0.1f);
                    break;
            }
            
            currentState = newState;
        }
        
        private enum BarState
        {
            Normal,
            Hover,
            Click
        }

        private enum SoundType
        {
            MASTER,
            BGM,
            SFX
        }
    }

}
using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Main.UI
{
    public class DropDownAnimationHandler : MonoBehaviour
    {
        [SerializeField] 
        private DOTweenAnimation doTweenAnimation;
        
        private void OnEnable()
        {
            doTweenAnimation.DORestart();
        }

        private void OnDisable()
        {
            doTweenAnimation.DOPlayBackwards();
        }
    }
}
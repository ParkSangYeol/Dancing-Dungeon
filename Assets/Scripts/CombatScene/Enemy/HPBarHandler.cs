using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CombatScene.Enemy
{
    public class HPBarHandler : MonoBehaviour
    {
        [Title("Components")] 
        [SerializeField] 
        private RectTransform fillImageInner;
        [SerializeField] 
        private RectTransform fillImageOuter;

        [Title("Variables")] 
        [SerializeField]
        private float moveDuration = 0.5f;
        [SerializeField]
        private Ease moveEase = Ease.Linear;

        private void Start()
        {
            if (fillImageInner == null)
            {
                fillImageInner = transform.Find("IMG_Inner").GetComponent<RectTransform>();
            }
            
            if (fillImageOuter == null)
            {
                fillImageInner = transform.Find("IMG_Outer").GetComponent<RectTransform>();
            }
        }

        private void MoveInnerImage(float x)
        {
            if (DOTween.IsTweening("InnerMove"))
            {
                DOTween.TweensById("InnerMove")[0].Kill();
            }
            fillImageInner.DOLocalMoveX(x, moveDuration).SetEase(moveEase).SetId("InnerMove");
        }

        public void SetHPUI(float currentHP, float maxHP)
        {
            float decreaseHP = maxHP - currentHP;
            float moveX = -decreaseHP / maxHP * fillImageInner.rect.width;
            Vector3 newPosition = new Vector3(moveX, 0, 0);
            fillImageOuter.localPosition = newPosition;
            MoveInnerImage(moveX);
        }
    }

}
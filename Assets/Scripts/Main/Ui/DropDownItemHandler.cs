using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Main.UI
{
    public class DropDownItemHandler : MonoBehaviour
    {
        [SerializeField] 
        private TMP_Text label;

        [SerializeField] 
        private ScrollRect scrollRect;

        [SerializeField]
        private EventTrigger eventTrigger;
        
        [SerializeField] 
        private Color textColor;
        private Color defaultColor;

        public void OnPointEnter()
        {
            label.DOColor(textColor, 0.1f);
        }

        public void OnPointExit()
        {
            label.DOColor(defaultColor, 0.1f);
        }
        
        private void Start()
        {
            defaultColor = label.color;
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.Scroll;
            entry.callback.AddListener((data) =>
            {
                scrollRect.OnScroll(data as PointerEventData);
            });
            eventTrigger.triggers.Add(entry);
        }
    }
}
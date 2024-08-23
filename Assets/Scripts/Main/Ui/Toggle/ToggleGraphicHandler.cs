using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Main.UI 
{
    public class ToggleGraphicHandler : MonoBehaviour
    {
        [SerializeField] 
        private GameObject toggleOffGameObject;
        [SerializeField] 
        private GameObject toggleOnGameObject;

        [SerializeField] 
        private Toggle toggle;

        private void Start()
        {
            toggle.onValueChanged.AddListener((on) =>
            {
                toggleOffGameObject.SetActive(!on);
                toggleOnGameObject.SetActive(on);
            });
        }
    }

}
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Main.UI
{
    public class ToggleDataSaver : MonoBehaviour
    {
        [SerializeField] 
        private string key;

        [SerializeField] 
        private bool defaultCheck;
        
        [SerializeField] 
        private Toggle toggle;
        
        private void Start()
        {
            toggle.isOn = PlayerPrefs.HasKey(key) ? bool.Parse((ReadOnlySpan<char>)PlayerPrefs.GetString(key)) : defaultCheck;

            toggle.onValueChanged.AddListener(OnChangeValue);
        }

        private void OnChangeValue(bool value)
        {
            PlayerPrefs.SetString(key, value.ToString());
            PlayerPrefs.Save();
        }
    }
}
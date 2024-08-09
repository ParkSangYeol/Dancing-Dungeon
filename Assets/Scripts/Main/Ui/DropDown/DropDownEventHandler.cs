using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace Main.UI
{
    public class DropDownEventHandler : MonoBehaviour
    {
        [SerializeField] 
        private TMP_Dropdown dropdown;

        private void Start()
        {
            dropdown.value = LocalizationSettings.AvailableLocales.Locales.IndexOf(LocalizationSettings.SelectedLocale);
        }

        public void OnLanguageChange(Int32 index)
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
        }
    }
}
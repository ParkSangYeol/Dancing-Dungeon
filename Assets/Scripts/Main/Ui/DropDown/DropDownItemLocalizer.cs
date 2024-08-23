using System;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

namespace Main.UI
{
    public class DropDownItemLocalizer : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text textObject;

        [SerializeField] 
        private SerializedDictionary<string, TMP_FontAsset> fontAssets;
        private void Start()
        {
            SetLocalFont();
        }

        private void SetLocalFont()
        {
            textObject.font = textObject.text switch
            {
                "简体中文" => //중국어 간체
                    fontAssets["Chinese"],
                "繁體中文" => //중국어 번체
                    fontAssets["Taiwanese"],
                "English" => //영어
                    fontAssets["English"],
                "日本語" => //일본어
                    fontAssets["Japanese"],
                "한국어" => //한국어
                    fontAssets["Korean"],
                "Русский язык" => //러시아어 
                    fontAssets["Russian"],
                "Español" => //스페인어
                    fontAssets["Spanish"],
                _ => textObject.font
            };
        }
    }

}
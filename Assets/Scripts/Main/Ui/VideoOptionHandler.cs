using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;


namespace Main.UI
{
    public class VideoOptionHandler : MonoBehaviour
    {
        private int screenWidth;
        private int screenHeight;
        private FullScreenMode screenMode;
        private int frame;
        private int brightness;

        [SerializeField] 
        private TMP_Dropdown resolutionDropdown;

        [SerializeField] 
        private TMP_Dropdown frameDropdown;
        
        [SerializeField] 
        private Toggle fullscreenToggle;
        
        [SerializeField] 
        private Toggle windowToggle;
        
        [SerializeField] 
        private List<Resolution> resolutions;
        
        [SerializeField] 
        private List<int> frames;

        [SerializeField]
        private Slider slider;

        [SerializeField] 
        private TMP_Text frameText;

        private void Start()
        {
            SetupScreen();
            SetupFrame();
            SetupBrightness();
            SetupResolutionDropdown();
            SetFrameDropdown(LocalizationSettings.SelectedLocale);
            SetupToggle();
            LocalizationSettings.SelectedLocaleChanged += SetFrameDropdown;
            this.gameObject.SetActive(false);
        }

        private void SetupScreen()
        {
            screenWidth = PlayerPrefs.HasKey("ScreenWidth")? PlayerPrefs.GetInt("ScreenWidth") : Screen.width;
            screenHeight = PlayerPrefs.HasKey("ScreenHeight")? PlayerPrefs.GetInt("ScreenHeight") : Screen.height;

            if (PlayerPrefs.HasKey("FullScreenMode") &&
                Enum.TryParse(PlayerPrefs.GetString("FullScreenMode"), out screenMode))
            {
                
            }
            else
            {
                screenMode = FullScreenMode.ExclusiveFullScreen;
            }

            Screen.SetResolution(screenWidth, screenHeight, screenMode);
        }

        private void SetupFrame()
        {
            frame = PlayerPrefs.HasKey("Frame")? PlayerPrefs.GetInt("Frame") : Application.targetFrameRate;
            Application.targetFrameRate = frame;
            QualitySettings.vSyncCount = 0;
        }

        private void SetupBrightness()
        {
            brightness = PlayerPrefs.HasKey("Brightness") ? PlayerPrefs.GetInt("Brightness") : 2;
            VolumeObject.Instance.SetVolumeProfile(brightness);

            slider.value = brightness - 2;
        }
        
        private void SetupResolutionDropdown()
        {
            if (resolutionDropdown == null)
            {
                return;
            }
            
            foreach (var resolution in resolutions)
            {
                TMP_Dropdown.OptionData data = new TMP_Dropdown.OptionData();
                data.text = resolution.ToString();
                
                resolutionDropdown.options.Add(data);
                if (resolution.width.Equals(screenWidth) && resolution.height.Equals(screenHeight))
                {
                    resolutionDropdown.value = resolutionDropdown.options.Count - 1;
                }
            }
        }

        private void SetFrameDropdown(Locale locale)
        {
            frameDropdown.options.Clear();
            foreach (var frameValue in frames)
            {
                TMP_Dropdown.OptionData data = new TMP_Dropdown.OptionData();
                if (frameValue == -1)
                {
                    data.text = LocalizationSettings.StringDatabase.GetLocalizedString("DefaultGameStringTable", "NoLimit");
                }
                else
                {
                    data.text = frameValue.ToString();
                }
                frameDropdown.options.Add(data);
                
                if (this.frame == frameValue)
                {
                    frameDropdown.value = frameDropdown.options.Count - 1;
                }
            }
            ForceUpdateDropdownFrame();
        }

        private void ForceUpdateDropdownFrame()
        {
            Debug.Log("value: " + frameDropdown.value + " count: " + frameDropdown.options.Count);
            if (frameDropdown.value == frameDropdown.options.Count - 1)
            {
                frameText.text = LocalizationSettings.StringDatabase.GetLocalizedString("DefaultGameStringTable", "NoLimit");
            }
        }
        
        private void SetupToggle()
        {
            if (screenMode == FullScreenMode.ExclusiveFullScreen)
            {
                fullscreenToggle.isOn = true;
                windowToggle.isOn = false;
            }
            else
            {
                fullscreenToggle.isOn = false;
                windowToggle.isOn = true;
            }
        }
        
        private void ChangeScreenMode(FullScreenMode mode)
        {
            screenMode = mode;
            Screen.SetResolution(screenWidth, screenHeight, screenMode);
                
            PlayerPrefs.SetString("ScreenMode", screenMode.ToString());
            PlayerPrefs.Save();
        }

        public void OnChangeFrame(Int32 index)
        {
            frame = frames[index];
            Application.targetFrameRate = frame;
            
            PlayerPrefs.SetInt("Frame", frame);
            PlayerPrefs.Save();
        } 
        
        public void OnChangeResolution(Int32 index)
        {
            screenWidth = resolutions[index].width;
            screenHeight = resolutions[index].height;
            
            Screen.SetResolution(screenWidth, screenHeight, screenMode);
            
            PlayerPrefs.SetInt("ScreenWidth",screenWidth);
            PlayerPrefs.SetInt("ScreenHeight",screenHeight);
            PlayerPrefs.Save();
        }

        public void OnChangeScreenModeToFullScreen(bool value)
        {
            if (!value)
            {
                return;
            }
            ChangeScreenMode(FullScreenMode.ExclusiveFullScreen);
        }

        public void OnChangeScreenModeToWindow(bool value)
        {
            if (!value)
            {
                return;
            }
            ChangeScreenMode(FullScreenMode.Windowed);
        }

        public void OnChangeBrightness(float value)
        {
            brightness = (int)value + 2;
            
            ColorAdjustments colorAdjustments = null;
            VolumeObject.Instance.SetVolumeProfile(brightness);
            
            PlayerPrefs.SetInt("Brightness", brightness);
            PlayerPrefs.Save();
        }
        
        [Serializable]
        public class Resolution
        {
            [HorizontalGroup]
            public int width;
            [HorizontalGroup]
            public int height;

            public Resolution(int w, int h)
            {
                this.width = w;
                this.height = h;
            }
            
            public override string ToString()
            {
                return width + " X " + height;
            }
        }
    }
}
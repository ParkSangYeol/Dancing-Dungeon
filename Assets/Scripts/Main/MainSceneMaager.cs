using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainSceneMaager : MonoBehaviour
{
    
       [SerializeField] private GameObject mainPannel;
       [SerializeField]private GameObject optionPannel;
       [SerializeField] private AudioSource mainAudioSource;
       [SerializeField] private Slider slider;
        public string nextScene;
        public Image fadeImage; // 페이드 효과를 위한 이미지
        public float fadeDuration = 1.0f; // 페이드 지속 시간
      
    void Awake()
    {
        mainAudioSource = GetComponent<AudioSource>();
        
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Time.timeScale=1;
        if(PlayerPrefs.HasKey("AllVolume"))
        {
            slider.value = PlayerPrefs.GetFloat("AllVolume");
        }
        else
        {
            slider.value = 0.8f;
        }
        StartCoroutine(FadeIn());
        mainPannel.SetActive(true);
        optionPannel.SetActive(false);
        loadBgm();
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void loadBgm()
    {
        AudioClip bgmClip = Resources.Load<AudioClip>("MainBgm");
        mainAudioSource.clip = bgmClip;
        mainAudioSource.loop = true;
        mainAudioSource.Play();

    }
    public void StartGame()
    {
        StartCoroutine(FadeOutAndLoadScene(nextScene));
    }
    public void VisibleOptionPanel()
    {
        optionPannel.SetActive(true);
        mainPannel.SetActive(false);
    }

    public void VisibleMainPanel()
    {
        optionPannel.SetActive(false);
        mainPannel.SetActive(true);
    }
    public void SetVolume()
    {
        mainAudioSource.volume = slider.value;
        PlayerPrefs.SetFloat("AllVolume",slider.value);
        PlayerPrefs.Save();
    }
    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
     private IEnumerator FadeIn()
    {
       
        fadeImage.color = new Color(0, 0, 0, 1);
        fadeImage.gameObject.SetActive(true);

        
        for (float t = fadeDuration; t > 0; t -= Time.deltaTime)
        {
            float alpha = t / fadeDuration;
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        
        fadeImage.color = new Color(0, 0, 0, 0);
        fadeImage.gameObject.SetActive(false);
    }
    private IEnumerator FadeOutAndLoadScene(string sceneName)
    {
       
        fadeImage.gameObject.SetActive(true);
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float alpha = t / fadeDuration;
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

       
        fadeImage.color = new Color(0, 0, 0, 1);

        
        SceneManager.LoadScene(sceneName);
    }

}
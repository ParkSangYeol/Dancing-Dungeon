using System;
using UnityEditor.SearchService;
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
        SceneManager.LoadScene(nextScene);
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

}
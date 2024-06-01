using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class UiManager : MonoBehaviour
{
    private static UiManager instance;
    private GameObject optionPanel;
    private GameObject mainPanel;
    private AudioSource audioSource;
    private Slider volumeSlider; // 슬라이더 UI

    // Battle BGM clips
    private List<AudioClip> battleBgms;
    private int currentBgmIndex;
    private bool isBattleScene;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject); 
            return;
        }
        
        optionPanel = GameObject.Find("Canvas/OptionPannel");
        mainPanel = GameObject.Find("Canvas/MainPannel");
        volumeSlider = GameObject.Find("Canvas/OptionPannel/Sound/VolumeSlider").GetComponent<Slider>();
        if(optionPanel != null && mainPanel != null && volumeSlider != null)
        {
            optionPanel.SetActive(false);
            mainPanel.SetActive(true);
            Debug.Log("존재");
        }
        audioSource = gameObject.AddComponent<AudioSource>();
        AudioClip bgmClip = Resources.Load<AudioClip>("MainBgm"); 
        if (bgmClip != null)
        {
            audioSource.clip = bgmClip;
            audioSource.loop = true; // BGM 반복 재생 설정
            audioSource.Play(); // BGM 재생
        }
        else
        {
            Debug.Log("bgm존재");
        }

        // Battle BGM clips
        battleBgms = new List<AudioClip>
        {
            Resources.Load<AudioClip>("BattleBgm"),
            Resources.Load<AudioClip>("BattleBgm2"),
            
        };

        currentBgmIndex = -1;
        isBattleScene = false;
    }

    void Start()
    {
        if (volumeSlider != null)
        {
            volumeSlider.value = audioSource.volume;
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }
        else
        {
            Debug.LogError("볼륨 슬라이더가 없어~");
        }
    }

    void Update()
    {
        if (isBattleScene && !audioSource.isPlaying)
        {
            PlayRandomBgm();
        }
    }

    public void VisibleOptionPanel()
    {
        optionPanel.SetActive(true);
        mainPanel.SetActive(false);
    }

    public void VisibleMainPanel()
    {
        optionPanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    public void NextScene()
    {
        SceneManager.LoadScene("NoteSystemDev2");
        isBattleScene = true; // 전투 씬으로 전환
        PlayRandomBgm();
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }

    private void PlayRandomBgm()
    {
        if (battleBgms.Count == 0) return;

        int newBgmIndex;
        do
        {
            newBgmIndex = Random.Range(0, battleBgms.Count);
        } while (newBgmIndex == currentBgmIndex);

        currentBgmIndex = newBgmIndex;
        audioSource.clip = battleBgms[currentBgmIndex];
        audioSource.loop = false; // 개별 곡은 루프하지 않음
        audioSource.Play();
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using CombatScene.Player;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.UI;

public class UiManager : MonoBehaviour
{
    public static UiManager instance { get; private set; }
    private GameObject optionPanel;
    private GameObject mainPanel;
    private GameObject hpPanel;
    private GameObject comboPanel;
    private GameObject GameOverPanel;
    private AudioSource audioSource;
    private AudioSource particleSoundSource;
    private Slider volumeSlider;
    private Button startButton;
    private Button optionButton;
    private Button finishButton;
    private Button backButton;

    private List<AudioClip> battleBgms;
    private List<AudioClip> particleSounds;
    private AudioClip bgmClip;
    private int currentBgmIndex;
    private bool isBattleScene;
    private GameObject player;
    private int combo;
    
    
    private int maxCombo=0;
    private int perfectCombo=0;
    private int greatCombo=0;
    private int badCombo=0;
    private int missCombo=0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        audioSource = gameObject.AddComponent<AudioSource>();
        particleSoundSource = gameObject.AddComponent<AudioSource>();
        bgmClip = Resources.Load<AudioClip>("MainBgm");
        if (bgmClip != null)
        {
            PlayBgm(bgmClip);
        }
        else
        {
            Debug.Log("BGM exists");
        }

        battleBgms = new List<AudioClip>
        {
            Resources.Load<AudioClip>("BattleBgm"),
            Resources.Load<AudioClip>("BattleBgm2"),
        };
        particleSounds = new List<AudioClip>
        {
            Resources.Load<AudioClip>("PerfectTiming"),
            Resources.Load<AudioClip>("GreatTiming"),
            Resources.Load<AudioClip>("BadTiming"),
            Resources.Load<AudioClip>("MissTiming"),
        };
        Debug.Log("Particle sounds size: " + particleSounds.Count);

        currentBgmIndex = -1;
        isBattleScene = false;
        FindInMainScene();
    }

    void Start()
    {
        
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
       if (scene.name == "MainScene")
        {
            // 메인 씬이 로드되었을 때 FindInMainScene 메서드를 호출합니다.
            FindInMainScene();
        }
        else if(scene.name =="NoteSystemDev2")
        {
            FindObjectinCombat();
        }
        
    }
    void Update()
    {
        if(SceneManager.GetActiveScene().name=="NoteSystemDev2")
        {
            if(player.GetComponent<PlayerController>().hp <=0 || Input.GetKeyDown(KeyCode.Q))
            {
                GameOverPanel.SetActive(true);
            }
            else
            {
                SetHpUi();
            }
        }
        
        if (isBattleScene && !audioSource.isPlaying)
        {
            PlayRandomBgm();
        }
       
    }

    private void FindObjectinCombat()
    {
       
        player= GameObject.Find("Player");
        if (player != null)
        {
            Debug.Log("Player found");
        }
        else
        {
            Debug.LogError("Player not found");
        }
        

        
        hpPanel = GameObject.Find("Canvas/HpPanel");
        if(hpPanel)
        {
            Debug.Log("패널 찾음");
        }
        
        
        comboPanel = GameObject.Find("Canvas/ComboPanel");
        if (comboPanel != null)
        {
            Debug.Log("ComboPanel found");
        }
        else
        {
            Debug.LogError("ComboPanel not found");
        }
    
            GameOverPanel = GameObject.Find("Canvas/GameOverPanel");
            if (GameOverPanel != null)
            {
                finishButton = GameObject.Find("MenuButton").GetComponent<Button>();
                finishButton.onClick.AddListener(NextScene);
                GameOverPanel.SetActive(false);
                Debug.Log("GameOverPanel found");
            }
            
    }
    
    void SetAllChildrenActive(GameObject parent, bool isActive)
    {
        foreach (Transform child in parent.transform)
        {
            child.gameObject.SetActive(isActive);
            SetAllChildrenActive(child.gameObject, isActive);
        }
    }
    public void FindInMainScene()
    {
        GameObject canvas = GameObject.Find("Canvas");
        SetAllChildrenActive(canvas,true);
        volumeSlider = GameObject.Find("Canvas/OptionPannel/Sound/VolumeSlider").GetComponent<Slider>();
        startButton = GameObject.Find("Canvas/MainPannel/Start").GetComponent<Button>();
        optionButton = GameObject.Find("Canvas/MainPannel/Options").GetComponent<Button>();
        backButton =GameObject.Find("Canvas/OptionPannel/Back/BackButton").GetComponent<Button>();
        optionPanel = GameObject.Find("Canvas/OptionPannel");
        mainPanel = GameObject.Find("Canvas/MainPannel");
        startButton.onClick.AddListener(NextScene);
        optionButton.onClick.AddListener(VisibleOptionPanel);
        backButton.onClick.AddListener(VisibleMainPanel);
        volumeSlider.onValueChanged.AddListener(SetVolume);
        
        
        mainPanel.SetActive(true);
        optionPanel.SetActive(false);
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
        if(SceneManager.GetActiveScene().name=="MainScene")
        {
            SceneManager.LoadScene("NoteSystemDev2");
            isBattleScene = true;
            PlayRandomBgm();
            
            
            
        }
        else if(SceneManager.GetActiveScene().name == "NoteSystemDev2")
        {
             SceneManager.LoadScene("MainScene");
             isBattleScene=false;
             PlayBgm(bgmClip);
             
        }
    }

    public void SetVolume(float volume)
    {
        particleSoundSource.volume = volume * 1.5f;
        audioSource.volume = volume * 0.5f;
    }
    public void PlayBgm(AudioClip bgm)
    {
        audioSource.clip = bgm;
        audioSource.loop = true;
        audioSource.Play();
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
        audioSource.loop = false;
        audioSource.Play();
    }

    public void PlayPerfectSound()
    {
        particleSoundSource.PlayOneShot(particleSounds[0]);
    }

    public void PlayGreatSound()
    {
        particleSoundSource.PlayOneShot(particleSounds[1]);
    }

    public void PlayBadSound()
    {
        particleSoundSource.PlayOneShot(particleSounds[2]);
    }

    public void PlayMissSound()
    {
        particleSoundSource.PlayOneShot(particleSounds[3]);
    }

    public void SetHpUi()
    {
        if (hpPanel != null && player != null)
        {
            TextMeshProUGUI hpText = hpPanel.GetComponentInChildren<TextMeshProUGUI>();
            if (hpText != null)
            {
                hpText.text = player.GetComponent<PlayerController>().hp.ToString();
            }
            else
            {
                Debug.LogError("TextMeshProUGUI component is missing in HpPanel");
            }
        }
    }
    public void SetCombo(int com,string timing)
    {
        combo = com;
        comboPanel.GetComponentInChildren<TextMeshProUGUI>().text="Combo : "+combo;
        maxCombo = Mathf.Max(maxCombo,combo);
        if(timing =="Perfect")
        {
            perfectCombo++;
        }
        else if(timing =="Great")
        {
            greatCombo++;
        }
        else if(timing == "Bad")
        {
            badCombo++;
        }
        else if(timing == "Miss")
        {
            missCombo++;
        }
    }
    public int PerfectCombo
    {
        get { return perfectCombo; }
    }

    public int GreatCombo
    {
        get { return greatCombo; }
    }

    public int BadCombo
    {
        get { return badCombo; }
    }

    public int MissCombo
    {
        get { return missCombo; }
    }
   
    

}
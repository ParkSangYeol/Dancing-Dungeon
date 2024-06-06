using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using CombatScene.Player;
using TMPro;
using Unity.VisualScripting;

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

    private List<AudioClip> battleBgms;
    private List<AudioClip> particleSounds;
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
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        optionPanel = GameObject.Find("Canvas/OptionPannel");
        mainPanel = GameObject.Find("Canvas/MainPannel");
        volumeSlider = GameObject.Find("Canvas/OptionPannel/Sound/VolumeSlider").GetComponent<Slider>();
        if (optionPanel != null && mainPanel != null && volumeSlider != null)
        {
            optionPanel.SetActive(false);
            mainPanel.SetActive(true);
            Debug.Log("Panels and slider exist");
        }
        audioSource = gameObject.AddComponent<AudioSource>();
        particleSoundSource = gameObject.AddComponent<AudioSource>();
        AudioClip bgmClip = Resources.Load<AudioClip>("MainBgm");
        if (bgmClip != null)
        {
            audioSource.clip = bgmClip;
            audioSource.loop = true;
            audioSource.Play();
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
            Debug.LogError("Volume slider is missing");
        }

        FindObjectinCombat();
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "NoteSystemDev2")
        {
            if (player == null || hpPanel == null)
            {
                FindObjectinCombat();
            }

            if (hpPanel != null && player != null)
            {
                SetHpUi();
            }
            if(player.GetComponent<PlayerController>().hp <=0 || Input.GetKeyDown(KeyCode.Q))
            {
                GameOverPanel.SetActive(true);
            }
        }
        if (isBattleScene && !audioSource.isPlaying)
        {
            PlayRandomBgm();
        }
    }

    private void FindObjectinCombat()
    {
        if (player == null)
        {
            player = GameObject.Find("Player");
            if (player != null)
            {
                Debug.Log("Player found");
            }
            else
            {
                Debug.LogError("Player not found");
            }
        }

        if (hpPanel == null)
        {
            hpPanel = GameObject.Find("Canvas/HpPanel");
            if (hpPanel != null)
            {
                Debug.Log("HpPanel found");
            }
            else
            {
                Debug.LogError("HpPanel not found");
            }
        }
        if(comboPanel ==null)
        {
            comboPanel = GameObject.Find("Canvas/ComboPanel");
            if (comboPanel != null)
            {
                Debug.Log("ComboPanel found");
            }
            else
            {
                Debug.LogError("ComboPanel not found");
            }
        }
        if(GameOverPanel ==null)
        {
            GameOverPanel = GameObject.Find("Canvas/GameOverPanel");
            if (GameOverPanel != null)
            {
                GameOverPanel.SetActive(false);
                Debug.Log("GameOverPanel found");
            }
            else
            {
                Debug.LogError("GameOverPanel not found");
            }
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
        isBattleScene = true;
        PlayRandomBgm();
    }

    public void SetVolume(float volume)
    {
        particleSoundSource.volume = volume * 1.5f;
        audioSource.volume = volume * 0.5f;
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

using System.Collections.Generic;
using CombatScene;
using CombatScene.Player;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatSceneUIManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] GameObject overPannel;
    [SerializeField] GameObject hpPannel;
    [SerializeField] GameObject comboPannel;
    [SerializeField] AudioSource combatAudioSource;
    private List<AudioClip> battleBgms;
    private int currentBgmIndex;
    [SerializeField]private GameObject player;
    public string nextScene;

    private int combo=0;
    private int maxCombo=0;
    private int perfectCombo=0;
    private int greatCombo=0;
    private int badCombo=0;
    private int missCombo=0;
    void Awake()
    {
        combatAudioSource = GetComponent<AudioSource>();
    }
    void Start()
    {
        if(PlayerPrefs.HasKey("AllVolume"))
        {
            combatAudioSource.volume = PlayerPrefs.GetFloat("AllVolume");
        }
        battleBgms = new List<AudioClip>
        {
            Resources.Load<AudioClip>("BattleBgm"),
            Resources.Load<AudioClip>("BattleBgm2"),
        };
        overPannel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (hpPannel!= null && player != null)
        {
            SetHpUi();
        }
        if(player.GetComponent<PlayerController>().hp <=0 || Input.GetKeyDown(KeyCode.Q))
        {
            overPannel.SetActive(true);
        }
        if(!combatAudioSource.isPlaying)
        {
            PlayRandomBgm();
        }
        
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
        combatAudioSource.clip = battleBgms[currentBgmIndex];
        combatAudioSource.loop = false;
        combatAudioSource.Play();
    }
    
    public void SetHpUi()
    {
        if (hpPannel!= null && player != null)
        {
            TextMeshProUGUI hpText = hpPannel.GetComponentInChildren<TextMeshProUGUI>();
            if (hpText != null && player.GetComponent<PlayerController>().hp>0)
            {
                hpText.text = player.GetComponent<PlayerController>().hp.ToString();
            }
            else
            {
                hpText.text ="0";
                
            }
        }
    }
    public void SetCombo(int com,string timing)
    {
        combo = com;
        comboPannel.GetComponentInChildren<TextMeshProUGUI>().text="Combo : "+combo;
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
     public int GetPerfectCombo
    {
        get { return perfectCombo; }
    }

    public int GetGreatCombo
    {
        get { return greatCombo; }
    }

    public int GetBadCombo
    {
        get { return badCombo; }
    }

    public int GetMissCombo
    {
        get { return missCombo; }
    }
    public void GoMain()
    {
        SceneManager.LoadScene(nextScene);
    }
    
}

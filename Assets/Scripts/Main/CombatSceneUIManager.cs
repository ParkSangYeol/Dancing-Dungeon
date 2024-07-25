using System.Collections.Generic;
using CombatScene.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;
using CombatScene;
using Unity.VisualScripting;


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
    [SerializeField]private GameObject weaponPannel;
    [SerializeField]private GameObject shieldPannel;
    [SerializeField]private HitScanByRay hitScanByRay;


    private Image weaponimage;
    private TextMeshProUGUI powerText;
    private TextMeshProUGUI weaponName;
    private TextMeshProUGUI range;
    private TextMeshProUGUI splashText;
    private TextMeshProUGUI directionText;


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
            
        };
        overPannel.SetActive(false);
        weaponimage=weaponPannel.transform.Find("WeaponImage").GetComponent<Image>();
        powerText=weaponPannel.transform.Find("Power").GetComponent<TextMeshProUGUI>();
        range = weaponPannel.transform.Find("Range").GetComponent<TextMeshProUGUI>();
        weaponName= weaponPannel.transform.Find("WeaponName").GetComponent<TextMeshProUGUI>();
        splashText =weaponPannel.transform.Find("Splash").GetComponent<TextMeshProUGUI>();
        directionText =weaponPannel.transform.Find("Direction").GetComponent<TextMeshProUGUI>();
        SetWeapon(player.GetComponent<PlayerController>().GetEquippedWeapon());
        SetShield();
        SetHpUi();
        
    }

    // Update is called once per frame
    void Update()
    {
        SetHpUi();
        SetShield();
        if(player.GetComponent<PlayerController>().hp <=0 || Input.GetKeyDown(KeyCode.Q))
        {
        
            overPannel.SetActive(true);
            Time.timeScale=0;
            
        }
        if(!combatAudioSource.isPlaying)
        {
            PlayBgmOneShot();
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
    private void PlayBgmOneShot()
    {
        combatAudioSource.clip=battleBgms[0];
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
    public void SetShield()
    {
        shieldPannel.GetComponentInChildren<TextMeshProUGUI>().text = player.GetComponent<PlayerController>().shield.ToString();
    }
    public void SetCombo(string timing)
    {
 

        if (timing == "Miss")
        {
            combo = 0; // Miss 발생 시 콤보 초기화
            missCombo++;
            hitScanByRay.HappenMiss();
        }
        else if (timing == "Bad")
        {
            combo = 0;
            badCombo++;
        }
        else
        {
            combo++; // 다른 경우에 콤보 업데이트
            maxCombo = Mathf.Max(maxCombo, combo);

            if (timing == "Perfect")
            {
                perfectCombo++;
            }
            else if (timing == "Great")
            {
                greatCombo++;
            }
            else if (timing == "Bad")
            {
                badCombo++;
            }
        }

        // 콤보 패널 업데이트
        comboPannel.GetComponentInChildren<TextMeshProUGUI>().text = "Combo : " + combo;
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
    public void SetWeapon(WeaponScriptableObject weaponScriptableObject)
    {
        if(weaponScriptableObject==null)
        {
            weaponScriptableObject= player.GetComponent<PlayerController>().GetEquippedWeapon();
        }
        weaponimage.sprite=weaponScriptableObject.thumbnail;
        powerText.text = "Power : " + weaponScriptableObject.power+" + "+player.GetComponent<PlayerController>().GetPlayerPower();
        weaponName.text ="Weapon : "+weaponScriptableObject.name;
        range.text ="Range : "+ weaponScriptableObject.range;
        if(weaponScriptableObject.isSplash)
        {
            splashText.text ="다중공격";
        }
        else
        {
            splashText.text="단일 공격";
        }
        switch(weaponScriptableObject.attackDirection)
        {
            case AttackDirection.DIR_8:
                directionText.text = "8방향";
                break;
            case AttackDirection.DIR_4:
                directionText.text = "4방향";
                break;
            default:
                directionText.text = "에러";
                break;

        }

        
        
        
    }
    
}
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
    private List<AudioClip> battleBgms;
    private int currentBgmIndex;
    [SerializeField]private GameObject player;
    public string nextScene;
    [SerializeField]private GameObject weaponPannel;
    [SerializeField]private GameObject shieldPannel;
    [SerializeField]private GameObject powerPanel;
    [SerializeField]private HitScanByRay hitScanByRay;


    private Image weaponimage;
    private TextMeshProUGUI weaponName;
    private TextMeshProUGUI range;
    private TextMeshProUGUI splashText;
    private TextMeshProUGUI hpText;
    private TextMeshProUGUI shieldText;
    private TextMeshProUGUI powerText;
    private PlayerController playerController;
    private BGMPlayer bgmPlayer;

    private int combo=0;
    private int maxCombo=0;
    private int perfectCombo=0;
    private int greatCombo=0;
    private int badCombo=0;
    private int missCombo=0;
    private int enemyKill = 0;
    private int bossKill = 0;
    
    void Awake()
    {
        bgmPlayer = GetComponent<BGMPlayer>();
    }
    void Start()
    {
        battleBgms = new List<AudioClip>
        {
            Resources.Load<AudioClip>("BattleBgm"),
            
        };
        
        // 초기 변수 설정
        weaponimage=weaponPannel.transform.Find("WeaponImage").GetComponent<Image>();
        weaponName= weaponPannel.transform.Find("WeaponName").GetComponent<TextMeshProUGUI>();
        range = weaponPannel.transform.Find("Range").GetComponent<TextMeshProUGUI>();
        splashText =weaponPannel.transform.Find("Splash & Direction").GetComponent<TextMeshProUGUI>();
        hpText = hpPannel.GetComponentInChildren<TextMeshProUGUI>();
        shieldText = shieldPannel.GetComponentInChildren<TextMeshProUGUI>();
        powerText = powerPanel.GetComponentInChildren<TextMeshProUGUI>();
        playerController = player.GetComponent<PlayerController>();
        
        // 초기 UI 설정
        WeaponScriptableObject playerWeapon = playerController.GetEquippedWeapon();
        SetWeapon(playerWeapon);
        SetPower(playerWeapon);
        SetShield();
        SetHpUi();
        
        // 초기 BGM 실행
        PlayBgmOneShot();
    }

    // Update is called once per frame
    void Update()
    {
        SetHpUi();
        SetShield();
        if(playerController.hp <=0 || Input.GetKeyDown(KeyCode.Q))
        {
        
            overPannel.SetActive(true);
            //Time.timeScale=0;
            
        }
        if(!bgmPlayer.IsBGMPlaying())
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
        
        bgmPlayer.SetAudioClip(battleBgms[currentBgmIndex]);
        bgmPlayer.Play();
        
        /*
        combatAudioSource.clip = battleBgms[currentBgmIndex];
        combatAudioSource.loop = false;
        combatAudioSource.Play();
        */
    }
    private void PlayBgmOneShot()
    {
        bgmPlayer.SetAudioClip(battleBgms[0]);
        bgmPlayer.Play();
        
        /*
        combatAudioSource.clip=battleBgms[0];
        combatAudioSource.loop = false;
        combatAudioSource.Play();
        */
    }
    
    private void SetHpUi()
    {
        if (hpPannel!= null && player != null)
        {
            if (hpText != null && playerController.hp>0)
            {
                hpText.text = playerController.hp.ToString();
            }
            else
            {
                hpText.text ="0";
                
            }
        }
    }
    private void SetShield()
    {
        shieldText.text = playerController.shield.ToString();
    }
    
    private void SetPower(WeaponScriptableObject weaponScriptableObject)
    {
        powerText.text = (weaponScriptableObject.power + playerController.GetPlayerPower()).ToString();
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
            weaponScriptableObject= playerController.GetEquippedWeapon();
        }
        weaponimage.sprite=weaponScriptableObject.thumbnail;
        weaponName.text = weaponScriptableObject.name; 
        range.text ="Range : "+ weaponScriptableObject.range;
        
        string splash = weaponScriptableObject.isSplash ? "다중 공격" : "단일 공격";
        string attackDir = weaponScriptableObject.attackDirection switch
        {
            AttackDirection.DIR_8 => "8방향",
            AttackDirection.DIR_4 => "4방향",
            _ => "에러"
        };

        splashText.text = attackDir + " " + splash;
    }

    public void DieEnemyCount()
    {
        enemyKill++;
    }

    public void DieBossCount()
    {
        bossKill++;
        enemyKill++;
    }

    public int GetEnemyDie()
    {
        return enemyKill;
    }

    public int GetBoosDie()
    {
        return bossKill;
    }

    public int GetMaxCombo()
    {
        return maxCombo;
    }

}
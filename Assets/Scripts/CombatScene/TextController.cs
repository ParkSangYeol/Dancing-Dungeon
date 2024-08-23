using System;
using TMPro;
using UnityEngine;

public class TextController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private TextMeshProUGUI[] overTextArr;// 1. 죽인 일반 몹 2. 죽인 보스몹 3. 맥스 콤보 4. 얻은 돈
    [SerializeField] private CombatSceneUIManager _combatSceneUIManager;
    void Start()
    {
        _combatSceneUIManager = GameObject.Find("CombatUiManager").GetComponent<CombatSceneUIManager>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetText()
    {
        
        overTextArr[0].text = "죽인 적 : " + _combatSceneUIManager.GetEnemyDie();
        overTextArr[1].text = "죽인 보스 : " + _combatSceneUIManager.GetBoosDie();
        overTextArr[2].text = "최대 콤보 : " + _combatSceneUIManager.GetMaxCombo();
        int earnMoney = _combatSceneUIManager.GetEnemyDie() * 1 + _combatSceneUIManager.GetBoosDie() * 5 +
                        _combatSceneUIManager.GetMaxCombo();
        overTextArr[3].text = "얻은 돈 : " + earnMoney;
        UpdatePlayerPrefebs_Combat();
    }

    public void UpdatePlayerPrefebs_Combat()
    {
        int killEnemy = PlayerPrefs.GetInt("KillEnemy", 0);
        PlayerPrefs.SetInt("KillEnemy",killEnemy+_combatSceneUIManager.GetEnemyDie());
        int killBoss = PlayerPrefs.GetInt("KillBoss",0);
        PlayerPrefs.SetInt("KillBoss",killBoss+_combatSceneUIManager.GetBoosDie());
        int money = PlayerPrefs.GetInt("PlayerMoney");
        int earnMoney = _combatSceneUIManager.GetEnemyDie() * 1 + _combatSceneUIManager.GetBoosDie() * 5 +
                        _combatSceneUIManager.GetMaxCombo();
        PlayerPrefs.SetInt("PlayerMoney",money+earnMoney);
        int saveEarnMoney = PlayerPrefs.GetInt("EarnMoney", 0);
        PlayerPrefs.SetInt("EarnMoney",saveEarnMoney+earnMoney);
        PlayerPrefs.Save();
    }

    private void OnEnable()
    {
        SetText();
    }
}

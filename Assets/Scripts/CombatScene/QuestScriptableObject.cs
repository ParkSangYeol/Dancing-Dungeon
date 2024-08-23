using UnityEngine.UI;
using UnityEngine;
using Sirenix.OdinInspector;
using Unity.VisualScripting;

public enum AchivementType
{
    KillEnemy,
    KillBoss,
    EarnGold,
    PlayTime
};

public enum AchivementGift
{
    Gold,
    Item
};

public enum Repetible
{
    NoRepeat,
    Repeat
};
[CreateAssetMenu(fileName = "New Achievement", menuName = "Achievement/Achievement")]

public class Achievement : ScriptableObject
{
    public AchivementType type; //어떠한 것을 체크하는지
    
    public AchivementGift gift; // 달성 보상

    public Repetible isRepeat;
    
    private int currentState; // 현재 진행정도
    
    private bool isAchive; // 달성했는지

    public int increaseAmount_Level; // 도전과제를 달성하기 위한 조건값

    public int increaseAmount_Gift; // 보상을 얼마나 줄지에 대한 값
    
    [PreviewField(Height = 100)] // Sprite의 미리보기 기능
    public Sprite questImage;
    
    public string questName;
    //구현기능 1. 퀘스트 완료 기능 각각 타입별로 2. 보상을 지급하는 기능 3. 반복과제시 다음 퀘스트를 위한 새로운 과제 생성

    
    public void QuestCheck() //퀘스트가 완료 되었다면 그에 맞게 보상을 지급해주는 함수
    {
        Debug.Log("처음 playtime 레벨값 : "+ increaseAmount_Level);
        currentState = PlayerPrefs.GetInt(type.ToString(),0); // 1. 현재 진행여부 체크
        
        if (CheckState()) // 현재 진행
        {
            // 보상 지급
            if (gift == AchivementGift.Gold)
            {
                PayReward_Money();
                increaseAmount_Level *= 2;
                PlayerPrefs.SetInt(type.ToString()+"Level", increaseAmount_Level);
                PlayerPrefs.Save();
            }

            if (gift == AchivementGift.Item)
            {
                //이건 상점 인벤토리 
            }
        }
    }

    public bool CheckState() // 퀘스트를 완료했는지 검사
    {
        switch (type)
        {
            case AchivementType.KillEnemy:
                currentState = PlayerPrefs.GetInt("KillEnemy", 0);
                increaseAmount_Level = PlayerPrefs.GetInt("KillEnemyLevel", increaseAmount_Level);
                if (currentState >= increaseAmount_Level)
                {
                    return true;
                }
                break;

            case AchivementType.KillBoss:
                currentState = PlayerPrefs.GetInt("KillBoss", 0);
                increaseAmount_Level = PlayerPrefs.GetInt("KillBossLevel", increaseAmount_Level);
                if (currentState >= increaseAmount_Level)
                {
                    
                    return true;
                }
                break;

            case AchivementType.EarnGold:
                currentState = PlayerPrefs.GetInt("EarnGold", 0);
                increaseAmount_Level = PlayerPrefs.GetInt("EarnGoldLevel", increaseAmount_Level);
                if (currentState >= increaseAmount_Level)
                {
                 
                    return true;
                }
                break;

            case AchivementType.PlayTime:
                Debug.Log("StateCheck 에서의 플탐 레벨 " + increaseAmount_Level);
                if (PlayerPrefs.GetInt("PlayTimeLevel") == 0)
                {
                    PlayerPrefs.SetInt("PlayTimeLevel",increaseAmount_Level);
                }
                currentState = PlayerPrefs.GetInt("PlayTime", 1);
                increaseAmount_Level = PlayerPrefs.GetInt("PlayTimeLevel",increaseAmount_Level);
                if (currentState >= increaseAmount_Level)
                {
                    return true;
                }
                break;
        }

        return false;
    }

    

    public void PayReward_Money()
    {
        int currentMoney = PlayerPrefs.GetInt("PlayerMoney", 0);
        PlayerPrefs.SetInt("PlayerMoney",currentMoney+increaseAmount_Gift);
        Debug.Log("보상!! "+PlayerPrefs.GetInt("PlayerMoney", 0));
        int currnetEarnMoney = PlayerPrefs.GetInt("EarnMoney", 0);
        PlayerPrefs.SetInt("EarnMoney",currnetEarnMoney+increaseAmount_Gift);
    }

    public Sprite GetSprite()
    {
        return questImage;
    }

    public int GetLevel()
    {
        return PlayerPrefs.GetInt(type.ToString() + "Level");
    }

    public int GetCurrentState()
    {
        int currnet = PlayerPrefs.GetInt(type.ToString(),0);
        return currnet;

    }
    


}
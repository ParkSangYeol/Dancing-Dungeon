using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public int hpUpgradeCost;//업그레이드 비용(hp)
    public int attackUpgradeCost;// 업그레이드 비용(공격)
    public int hpUpAmount; // 한번 증가시킬때 얼마나 증가하는지
    public int attackUpAmount;//""
    [SerializeField]
    private GameObject statePannel;
    [SerializeField]
    private GameObject eqiquementPanel;
    
    public GameObject hpfirstPlaceParent;  // 첫째 자리 숫자의 부모 오브젝트
    public GameObject hpsecondPlaceParent; // 둘째 자리 숫자의 부모 오브젝트
    public GameObject hpthirdPlaceParent;  // 셋째 자리 숫자의 부모 오브젝트

    private GameObject[] hpfirstPlaceNumbers;  // 첫째 자리 숫자 배열
    private GameObject[] hpsecondPlaceNumbers; // 둘째 자리 숫자 배열
    private GameObject[] hpthirdPlaceNumbers;  // 셋째 자리 숫자 배열

    public GameObject atkfirstPlaceParent;  // 첫째 자리 숫자의 부모 오브젝트
    public GameObject atksecondPlaceParent; // 둘째 자리 숫자의 부모 오브젝트
    public GameObject atkthirdPlaceParent;  // 셋째 자리 숫자의 부모 오브젝트

    private GameObject[] atkfirstPlaceNumbers;  // 첫째 자리 숫자 배열
    private GameObject[] atksecondPlaceNumbers; // 둘째 자리 숫자 배열
    private GameObject[] atkthirdPlaceNumbers;  // 셋째 자리 숫자 배열

    private int playerMoney;
    private int playerHp;
    private int playerAtk;
    //능력치 강화 부분

    public GameObject hairPart;
    public GameObject clothPart;
    public GameObject pantsPart;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start() {
        //hp 숫자배열 가져오기
        hpfirstPlaceNumbers = GetChildObjects(hpfirstPlaceParent);
        hpsecondPlaceNumbers = GetChildObjects(hpsecondPlaceParent);
        hpthirdPlaceNumbers = GetChildObjects(hpthirdPlaceParent);
        
        //공격 숫자배열 가져오기
        atkfirstPlaceNumbers =GetChildObjects(atkfirstPlaceParent);
        atksecondPlaceNumbers = GetChildObjects(atksecondPlaceParent);
        atkthirdPlaceNumbers = GetChildObjects(atkthirdPlaceParent);
        
        //기존 hp,공격 값 가져오기
        playerHp = PlayerPrefs.GetInt("PlayerHp", 0);
        playerAtk =PlayerPrefs.GetInt("PlayerAttack",0);
        Debug.Log(playerHp+" "+playerAtk);
        
       
        string playerHpString = playerHp.ToString("D3"); 
        string playerAtkString = playerAtk.ToString("D3");

        // 기존 hp,공격값으로 숫자배열들 초기화
        ActivateNumber(hpfirstPlaceNumbers, playerHpString[0] - '0');
        ActivateNumber(hpsecondPlaceNumbers, playerHpString[1] - '0');
        ActivateNumber(hpthirdPlaceNumbers, playerHpString[2] - '0');
        
        ActivateNumber(atkfirstPlaceNumbers, playerAtkString[0] - '0');
        ActivateNumber(atksecondPlaceNumbers, playerAtkString[1] - '0');
        ActivateNumber(atkthirdPlaceNumbers, playerAtkString[2] - '0');


        //추후 돈 표시 기능 개발
        playerMoney = PlayerPrefs.GetInt("PlayerMoney",0);
    }
    public void OnStatePanel()
    {
        eqiquementPanel.SetActive(false);
        statePannel.SetActive(true);
    }
    public void OnOneqiquementPanel()
    {
        statePannel.SetActive(false);
        eqiquementPanel.SetActive(true);
    }
    public void OnHairPart()
    {
        
        clothPart.SetActive(false);
        pantsPart.SetActive(false);
        hairPart.SetActive(true);
    }
    public void OnClothPart()
    {
        pantsPart.SetActive(false);
        hairPart.SetActive(false);
        clothPart.SetActive(true);
    }
    public void OnPantsPart()
    {
        hairPart.SetActive(false);
        clothPart.SetActive(false);
        pantsPart.SetActive(true);
    }
     
    void ActivateNumber(GameObject[] numberArray, int number)
    {
        for (int i = 0; i < numberArray.Length; i++)
        {
            numberArray[i].SetActive(i == number);
        }
    }
    public void UpgradeHP()
    {
        if(playerHp<1000)
        {
        
            // 버튼을 누르면 수행되는 작업
            //1) 조건검사(돈이 있는지)
            //2) PlayerPrefeb의 hp 재설정
            playerHp += hpUpAmount;
            PlayerPrefs.SetInt("PlayerHp", playerHp);
            playerMoney-=hpUpgradeCost;
            PlayerPrefs.SetInt("PlayerMoney",playerMoney);
            PlayerPrefs.Save();
            string playerHpString = playerHp.ToString("D3"); 

        
            ActivateNumber(hpfirstPlaceNumbers, playerHpString[0] - '0');
            ActivateNumber(hpsecondPlaceNumbers, playerHpString[1] - '0');
            ActivateNumber(hpthirdPlaceNumbers, playerHpString[2] - '0');
        }

        
    }
     public void UpgradeAttack()
    {
        //if(playerMoney<=attackUpgradeCost)
        
            // 버튼을 누르면 수행되는 작업
            //1) 조건검사(돈이 있는지)
            //2) PlayerPrefeb의 hp 재설정
            if(playerAtk<1000)
            {
                playerAtk += attackUpAmount;
                PlayerPrefs.SetInt("PlayerAttack", playerAtk);
                playerMoney-=attackUpgradeCost;
                PlayerPrefs.SetInt("PlayerMoney",playerMoney);
                PlayerPrefs.Save();
                string playerAtkString = playerAtk.ToString("D3"); 

            
                ActivateNumber(atkfirstPlaceNumbers, playerAtkString[0] - '0');
                ActivateNumber(atksecondPlaceNumbers, playerAtkString[1] - '0');
                ActivateNumber(atkthirdPlaceNumbers, playerAtkString[2] - '0');
            }

        
    }

     GameObject[] GetChildObjects(GameObject parent)
    {
        int childCount = parent.transform.childCount;
        GameObject[] childObjects = new GameObject[childCount];
        for (int i = 0; i < childCount; i++)
        {
            childObjects[i] = parent.transform.GetChild(i).gameObject;
        }
        return childObjects;
    }
    public void Test()
    {
        PlayerPrefs.SetInt("PlayerAttack", 0);
        PlayerPrefs.SetInt("PlayerHp",0);
    }

}

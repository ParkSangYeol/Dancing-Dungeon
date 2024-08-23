using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public int hpUpgradeCost;//업그레이드 비용(hp)
    public int attackUpgradeCost;// 업그레이드 비용(공격)
    public int hpUpAmount; // 한번 증가시킬때 얼마나 증가하는지
    public int attackUpAmount;//""
    public TextMeshProUGUI moneyText;
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
    
    private int hairinventory; 
    private int clothinventory;
    private int pantsinventory;
    private int raceinventory;

    private string purchaseItem;
    //능력치 강화 부분

    public GameObject hairPart;
    public GameObject clothPart;
    public GameObject pantsPart;
    public GameObject racePart;

    //Spum sprite 변경
    public SPUM_SpriteList sPUM_SpriteList;

    public GameObject purchaseCheckPanel;
    public GameObject shortMoneyPanel;

   

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
        //test용 
        
        //기존 hp,공격 값 가져오기
        playerHp = PlayerPrefs.GetInt("PlayerHp", 0);
        playerAtk =PlayerPrefs.GetInt("PlayerAttack",0);
        //돈 정보 가져오기
        playerMoney = PlayerPrefs.GetInt("PlayerMoney",1000);
        SetMoneyText();
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
        Debug.Log(playerMoney);
        

    
        
        
        
    }

    public void InitializePlayPrefebsInventory()
    {
        
            hairinventory = PlayerPrefs.GetInt("HairInventory", 0);
            clothinventory = PlayerPrefs.GetInt("ClothInventory" , 0);
            pantsinventory = PlayerPrefs.GetInt("PantsInventory", 0);
        
        
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
        racePart.SetActive(false);
        clothPart.SetActive(false);
        pantsPart.SetActive(false);
        hairPart.SetActive(true);
    }
    public void OnClothPart()
    {
        racePart.SetActive(false);
        pantsPart.SetActive(false);
        hairPart.SetActive(false);
        clothPart.SetActive(true);
    }
    public void OnPantsPart()
    {
        racePart.SetActive(false);
        hairPart.SetActive(false);
        clothPart.SetActive(false);
        pantsPart.SetActive(true);
    }

    public void OnRacePart()
    {
        hairPart.SetActive(false);
        clothPart.SetActive(false);
        pantsPart.SetActive(false);
        racePart.SetActive(true);
        
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
        if(playerHp<1000 && playerMoney>hpUpgradeCost)
        {
        
            // 버튼을 누르면 수행되는 작업
            //1) 조건검사(돈이 있는지)
            //2) PlayerPrefeb의 hp 재설정
            playerHp += hpUpAmount;
            PlayerPrefs.SetInt("PlayerHp", playerHp);
            playerMoney-=hpUpgradeCost;
            PlayerPrefs.SetInt("PlayerMoney",playerMoney);
            PlayerPrefs.Save();
            SetMoneyText();
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
            if(playerAtk<1000 && playerMoney>attackUpgradeCost)
            {
                playerAtk += attackUpAmount;
                PlayerPrefs.SetInt("PlayerAttack", playerAtk);
                playerMoney-=attackUpgradeCost;
                PlayerPrefs.SetInt("PlayerMoney",playerMoney);
                PlayerPrefs.Save();
                SetMoneyText();
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
        PlayerPrefs.SetInt("PlayerMoney",1000);
        for (int i = 1; i <= 6; i++)
        {
            string hairname = "Hair" + i;
            string clothename = "Cloth" + i;
            string pantsname = "Pants" + i;
            PlayerPrefs.SetInt(hairname,0);
            PlayerPrefs.SetInt(clothename,0);
            PlayerPrefs.SetInt(pantsname,0);
        }
    }

    public void SetMoneyText()
    {
        moneyText.text = ""+playerMoney;
        Debug.Log("돈 업데이트");
    }

    public void SetPurchaseString(string name)
    {
        purchaseItem = name;
    }
    
    public void PurchaseItem(int cost)
    {
        string item = purchaseItem;
        if (playerMoney >= cost)
        {
            playerMoney -= cost;
            PlayerPrefs.SetInt("PlayerMoney", playerMoney);
            PlayerPrefs.SetInt(item, 1);
            string inventorytype = item.Substring(0, item.Length - 1);
            string inventoryitem = inventorytype + "Inventory";
            //인벤토리 몇번에 저장되는지 알려주기 위함. 처음엔 1 그다음엔 2 또 그다음엔 3번째칸에 산 아이템이 들어감. 
            PlayerPrefs.SetInt(inventoryitem,TypeTest(inventoryitem)+1);
            int inventoryindex = PlayerPrefs.GetInt(inventoryitem); //HairInventory
            
            //몇번아이템을 샀는지도 중요하다.
            string test =PlayerPrefs.GetString(inventoryitem + inventoryindex, "null");
            
            PlayerPrefs.SetString(inventoryitem+inventoryindex,item);//Ex) HairInventory1 
            string purchase = PlayerPrefs.GetString(inventoryitem + inventoryindex, "null");
            
            Debug.Log(purchase);

            PlayerPrefs.Save();
            ChangePurchaseButtonText(item);
            SetMoneyText();
            
            
            
            
            
        }
    }

    public void ChangePurchaseButtonText(string name)
    {
        GameObject purchaseButton = GameObject.Find(name + "PurchaseButton");
        if(PlayerPrefs.GetInt(name) == 1)
        {
            purchaseButton.GetComponentInChildren<TextMeshProUGUI>().text = "구매완료";
            purchaseButton.GetComponent<Button>().interactable = false;
        }
    }
    

    public int TypeTest(string str)
    {
        return PlayerPrefs.GetInt(str, 0);

    }
    

    public void GoMain()
    {
        SceneManager.LoadScene("Scenes/Build/MainScene");
    }

    public void ExitButtonFunc()
    {
        if (purchaseCheckPanel.activeInHierarchy)
        {
            purchaseCheckPanel.SetActive(false);
        }
        else if (shortMoneyPanel.activeInHierarchy)
        {
            shortMoneyPanel.SetActive(false);
        }
    }

    public void OnPurchaseCheckPanel(int cost) //가진 돈보다 적다면 구매불가 패널 띄우기
    {
        
        if (playerMoney >= cost)
        {
            purchaseCheckPanel.SetActive(true);
            shortMoneyPanel.SetActive(false);
        }
        else
        {
            purchaseCheckPanel.SetActive(false);
            shortMoneyPanel.SetActive(true);
        }
        
    }

    void InitializePlayerPrefeb()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
    




}

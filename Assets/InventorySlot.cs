using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class inventorySlot : MonoBehaviour
{
    public InventoryManager _inventoryManager;
    public SPUM_SpriteList spumSpriteList;

    void Start()
    {
        Debug.Log(gameObject.name+PlayerPrefs.GetString(gameObject.name));
        StartCoroutine(Initialize());
    }

    private IEnumerator Initialize()
    {
        // 코루틴으로 초기화 대기
        yield return new WaitUntil(() => _inventoryManager != null && spumSpriteList != null);

        // 게임 오브젝트 이름을 키로 PlayerPrefs에서 값을 가져옴, 그 칸에 들어갈 아이템이름.
        string savedItem = PlayerPrefs.GetString(gameObject.name, null);

        if (PlayerPrefs.HasKey(gameObject.name)) // 그 칸에 아이템이 존재한다면
        {
            

            if (gameObject.name.Contains("Hair"))
            {
                
                ProcessItem("Hair", _inventoryManager.hairpath, _inventoryManager.hairImage, savedItem);
                
            }
            else if (gameObject.name.Contains("Cloth"))
            {
                ProcessItem("Cloth", _inventoryManager.clothpath, _inventoryManager.clothImage, savedItem);
            }
            else if (gameObject.name.Contains("Pants"))
            {
                ProcessItem("Pants", _inventoryManager.pantspath, _inventoryManager.pantsImage, savedItem);
            }
            else if (gameObject.name.Contains("Race"))
            {
                ProcessItem("Race", _inventoryManager.racepath, _inventoryManager.raceImage, savedItem);
            }
            

            StartCoroutine(WaitSetting());
        }
        else
        {
            // 아이템이 없는 경우 이미지 비활성화
            Debug.Log("너 실행되어야지");
            var imageComponent = gameObject.GetComponent<Image>();
            if (imageComponent != null)
            {
                imageComponent.enabled = false;
            }
        }
    }

    private void ProcessItem(string itemType, string[] itemPaths, Sprite[] itemImages, string savedItem)
    {
        string name = gameObject.name; //ex) HairInventory1
        char lastChar = name[name.Length - 1]; //버튼번호 의미
        char lastItemChar = savedItem[savedItem.Length - 1]; // 그 인벤토리에 들어간 아이템의 숫자만 따기. ex) Hair2
        Debug.Log("인벤토리 번호는"+lastChar+" 인벤토리에 들어간 아이템은 "+lastItemChar);

        if (char.IsDigit(lastChar) && char.IsDigit(lastItemChar)) //
        {
            int lastDigit = int.Parse(lastChar.ToString());//인벤토리 마지막 이름이 숫자인지
            int lastItemDigit = int.Parse(lastItemChar.ToString());//인벤토리에 들어가는 아이템 숫자만 따기 

            Debug.Log("해당 인벤토리의 숫자는 : "+lastDigit+"이고 인벤토리에 들어간 아이템의 숫자는 "+lastItemDigit);

            var imageComponent = gameObject.GetComponent<Image>();
            if (imageComponent != null && lastItemDigit - 1 < itemImages.Length)
            {
                imageComponent.sprite = itemImages[lastItemDigit - 1];//이미지 설정
                spumSpriteList.InitializedPath(itemType, itemPaths[lastItemDigit - 1], lastDigit-1);
            }
        }
    }

    private IEnumerator WaitSetting()
    {
        
        yield return new WaitForSeconds(1.0f);
        Debug.Log("초기화 완료");
    }
}

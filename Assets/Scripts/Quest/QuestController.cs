using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;

public class QuestController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Achievement quest;

    private int test = 0;
    private Button button;

    private void Awake()
    {
        
    }

    void Start()
    {
        ForTest();
        Debug.Log(gameObject.name);
        Debug.Log("gameObject : "+gameObject.name+"  "+quest.increaseAmount_Level);
        button = transform.GetChild(3).GetComponent<Button>();
        button.onClick.AddListener(() => QuestButtonFunc(quest));
        transform.GetChild(0).GetComponent<Image>().sprite = quest.GetSprite();
        

        UpdateState();

    }

    public void UpdateState()
    {
        transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = quest.GetCurrentState()+ " / " + quest.GetLevel();
        if (quest.CheckState())
        {
            button.interactable = true;
            button.GetComponentInChildren<TextMeshProUGUI>().text = "success";
               
        }
        else
        {
            button.interactable = false;
            button.GetComponentInChildren<TextMeshProUGUI>().text = "fail";
        }
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }
    //  버튼을 누르면 퀘스트 체크후 보상 지급 만약에 완료조건에 미달이라면 버튼은 안보이게 끄기 
    //그렇다면 이렇게 하자 start때 퀘스트 완료 여부를 전부 체크 그리고 완료한 상태라면 버튼을 보이게해서 
    void QuestButtonFunc(Achievement quest)
    {
        Debug.Log("버튼 누를 때의 퀘스트 완료 상태 "+quest.CheckState());
        quest.QuestCheck(); // 버튼을 누르면 퀘스트 보상 지급 
        //지급 후에 반복과제이기 때문에 진행상황을 바꿔줘야해 어카지?
        UpdateState();
    }
    void ForTest()
    {
        PlayerPrefs.SetInt("KillEnemyLevel",1);
        PlayerPrefs.SetInt("PlayTimeLevel",1);
    }
}

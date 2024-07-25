using UnityEngine;
using UnityEngine.SceneManagement;

public class InventoryManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject hairPanel;
    public GameObject clothePanel;
    public GameObject pantsPanel;
    public GameObject racePanel;
    public string[] hairpath;
    public string[] clothpath;
    public string[] pantspath;
    public string[] racepath;
    public Sprite[] hairImage;
    public Sprite[] clothImage;
    public Sprite[] pantsImage;
    public Sprite[] raceImage;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPartPanel(string part)
    {
        switch (part)
        {
            case "Hair" :
                hairPanel.SetActive(true);
                clothePanel.SetActive(false);
                pantsPanel.SetActive(false);
                racePanel.SetActive(false);
                break;
            case  "Cloth":
                racePanel.SetActive(false);
                hairPanel.SetActive(false);
                clothePanel.SetActive(true);
                pantsPanel.SetActive(false);
                break;
            case "Pants":
                hairPanel.SetActive(false);
                clothePanel.SetActive(false);
                racePanel.SetActive(false);
                pantsPanel.SetActive(true);
                break;
            case "Race":
                hairPanel.SetActive(false);
                clothePanel.SetActive(false);
                pantsPanel.SetActive(false);
                racePanel.SetActive(true);
                break;
            
        }
    }

    public void GoMenu()
    {
        SceneManager.LoadScene("Scenes/Dev/GD/MainScene/MainScene");
    }
    
    
    
    
    // 산 옷들을 보여줘야함. 칸에서 띄어줘야함 어떻게?
    //PlayerPrefeb 에서 이 아이템들이 존재하는지 그리고 이것들을 산 순서대로 저장할것
}

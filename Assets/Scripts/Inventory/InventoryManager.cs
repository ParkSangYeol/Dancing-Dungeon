using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject hairPanel;
    public GameObject clothePanel;
    public GameObject pantsPanel;
    
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
                break;
            case  "Cloth":
                hairPanel.SetActive(false);
                clothePanel.SetActive(true);
                pantsPanel.SetActive(false);
                break;
            case "Pants":
                hairPanel.SetActive(false);
                clothePanel.SetActive(false);
                pantsPanel.SetActive(true);
                break;
            
        }
    }
    // 산 옷들을 보여줘야함.
}

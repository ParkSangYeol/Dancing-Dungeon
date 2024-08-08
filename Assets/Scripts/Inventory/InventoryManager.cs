using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    public Image fadeImage;
    public float fadeDuration;
    public Button[] buttonArray;
    [SerializeField] private float buttonDelay;
    
    void Start()
    {
        StartCoroutine(ClickButtonsInReverse());
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
    private IEnumerator FadeIn()
    {
       
        fadeImage.color = new Color(0, 0, 0, 1);
        fadeImage.gameObject.SetActive(true);

        
        for (float t = fadeDuration; t > 0; t -= Time.deltaTime)
        {
            float alpha = t / fadeDuration;
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        
        fadeImage.color = new Color(0, 0, 0, 0);
        fadeImage.gameObject.SetActive(false);
    }
    IEnumerator ClickButtonsInReverse()
    {
        StartCoroutine(FadeIn());
        for (int i = buttonArray.Length - 1; i >= 0; i--)
        {
            
            buttonArray[i].onClick.Invoke();
            Debug.Log(i+"번째 버튼 눌림");
            
            yield return new WaitForSeconds(buttonDelay);
        }
    }
    
    
    
    
}

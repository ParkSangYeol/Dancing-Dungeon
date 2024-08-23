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
    public GameObject applyPanel;
    
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
        //SceneManager.LoadScene("Scenes/Dev/GD/MainScene/MainScene");
        SceneManager.LoadScene("Scenes/Build/MainScene");
    }

    public void OnApplyPannel()
    {
        applyPanel.SetActive(true);
    }
    private IEnumerator FadeIn()
    {
        float fadeSpeedMultiplier = 0.5f; // 페이드 속도를 느리게 할 수 있는 변수

        fadeImage.color = new Color(0, 0, 0, 1); // 처음에 완전히 불투명하게 설정
        fadeImage.gameObject.SetActive(true);

        // 페이드인: 점차 투명해지게 설정
        for (float t = 0; t < fadeDuration; t += Time.deltaTime * fadeSpeedMultiplier)
        {
            float alpha = 1 - (t / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        fadeImage.color = new Color(0, 0, 0, 0); // 페이드 완료 후 완전히 투명하게 설정
        fadeImage.gameObject.SetActive(false);
    }


    IEnumerator ClickButtonsInReverse()
    {
       
        for (int i = buttonArray.Length - 1; i >= 0; i--)
        {
            
            buttonArray[i].onClick.Invoke();
            Debug.Log(i+"번째 버튼 눌림");
            
            yield return new WaitForSeconds(buttonDelay);
        }
    }
    
    
    
    
}

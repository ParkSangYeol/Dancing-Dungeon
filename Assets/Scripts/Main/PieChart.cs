using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshProUGUI 사용을 위해 추가
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI.Extensions;

public class PieChart : MonoBehaviour
{
    public GameObject slicePrefab; // 파이 조각을 나타내는 프리팹
    public Transform chartContainer; // 그래프를 표시할 컨테이너
    public Font customFont; // 에디터에서 할당할 폰트
    public RectTransform graphAnchor; // 그래프의 위치를 지정할 빈 게임오브젝트

    // 그래프 애니메이션 관련 함수
    public float fillGraphDuration = 1f;
    public Ease easeGraph = Ease.Flash;
    public Color graphColor;
    
    // 게임 오버 패널의 텍스트 필드 참조
    public TextMeshProUGUI perfectText;
    public TextMeshProUGUI greatText;
    public TextMeshProUGUI badText;
    public TextMeshProUGUI missText;

     
    // 각 결과를 나타내는 값
    private int perfectCount;
    private int greatCount;
    private int badCount;
    private int missCount;

    [SerializeField]private CombatSceneUIManager combatSceneUIManager;
    [SerializeField] 
    private TMP_Text percentText; 
    
    private void OnEnable()
    {
        StartCoroutine(ShowPieChart());
    }

    /*
    IEnumerator ShowPieChart()
    {
        Time.timeScale = 1f;
        perfectCount = combatSceneUIManager.GetPerfectCombo;
        greatCount = combatSceneUIManager.GetGreatCombo;
        badCount = combatSceneUIManager.GetBadCombo;
        missCount = combatSceneUIManager.GetMissCombo;
        float total = perfectCount + greatCount + badCount + missCount;

        if (total == 0)
        {
            Debug.LogWarning("Total count is zero, no data to display in the pie chart.");
            yield break;
        }

        float perfectRatio = perfectCount / total;
        float greatRatio = greatCount / total;
        float badRatio = badCount / total;
        float missRatio = missCount / total;

        // 텍스트 필드에 비율 설정
        perfectText.text = "Perfect: " + (perfectRatio * 100).ToString("F2") + "%";
        perfectText.color=  Color.green;
        greatText.text = "Great: " + (greatRatio * 100).ToString("F2") + "%";
        greatText.color = Color.blue;
        badText.text = "Bad: " + (badRatio * 100).ToString("F2") + "%";
        badText.color=Color.yellow;
        missText.text = "Miss: " + (missRatio * 100).ToString("F2") + "%";
        missText.color = Color.red;

        float zRotation = 0f;

        yield return StartCoroutine(CreateSlice("Perfect", perfectRatio, Color.green, zRotation));
        zRotation -= perfectRatio * 360f;
        yield return StartCoroutine(CreateSlice("Great", greatRatio, Color.blue, zRotation));
        zRotation -= greatRatio * 360f;
        yield return StartCoroutine(CreateSlice("Bad", badRatio, Color.yellow, zRotation));
        zRotation -= badRatio * 360f;
        yield return StartCoroutine(CreateSlice("Miss", missRatio, Color.red, zRotation));
        zRotation -= missRatio * 360f;

        // 그래프 위치 조정
        if (graphAnchor != null)
        {
            RectTransform chartRectTransform = chartContainer.GetComponent<RectTransform>();
            chartRectTransform.anchoredPosition = graphAnchor.anchoredPosition;
        }
    }
    */
    
    IEnumerator ShowPieChart()
    {
        Time.timeScale = 1f;
        perfectCount = combatSceneUIManager.GetPerfectCombo;
        greatCount = combatSceneUIManager.GetGreatCombo;
        badCount = combatSceneUIManager.GetBadCombo;
        missCount = combatSceneUIManager.GetMissCombo;
        float total = perfectCount + greatCount + badCount + missCount;
        float accuracy = (perfectCount + (greatCount * 0.8f)) / total;
        if (total == 0)
        {
            Debug.LogWarning("Total count is zero, no data to display in the pie chart.");
            yield break;
        }

        StartCoroutine(CreateSlice("Accuracy", accuracy, graphColor));
        int percent = (int)(accuracy * 100f);
        percentText.DOText(percent.ToString(), fillGraphDuration, false, ScrambleMode.Numerals)
            .SetEase(easeGraph);
        
        // 그래프 위치 조정
        if (graphAnchor != null)
        {
            RectTransform chartRectTransform = chartContainer.GetComponent<RectTransform>();
            chartRectTransform.anchoredPosition = graphAnchor.anchoredPosition;
        }
    }
    
    IEnumerator CreateSlice(string label, float fillAmount, Color color)
    {
        if (fillAmount <= 0) yield break;
        GameObject slice = Instantiate(slicePrefab, chartContainer);
        UICircle sliceImage = slice.GetComponent<UICircle>();
        sliceImage.color = color;

        sliceImage.SetArc(0);

        float fillTime = 0f;
        while (fillTime < fillGraphDuration)
        {
            float arcValue = DOVirtual.EasedValue(0f, fillGraphDuration, fillTime, easeGraph);
            sliceImage.SetArc(arcValue * fillAmount);
            fillTime += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        sliceImage.SetArc(fillAmount);
    }

    /*
    IEnumerator CreateSlice(string label, float fillAmount, Color color, float zRotation)
    {
        if (fillAmount <= 0) yield break;
        Debug.Log("Start Create PieChartSlice. current Time Scale: " + Time.timeScale);
        Debug.Log("PieChart Slice info. Label:" + label+ ", fillAmount: " + fillAmount);
        
        GameObject slice = Instantiate(slicePrefab, chartContainer);
        UICircle sliceImage = slice.GetComponent<UICircle>();
        sliceImage.color = color;

        RectTransform rectTransform = slice.GetComponent<RectTransform>();
        rectTransform.localRotation = Quaternion.Euler(new Vector3(0, 0, zRotation));;
        sliceImage.SetArc(0);

        while (sliceImage.Arc < fillAmount)
        {
            Debug.Log("PieChart Arc Change. arc: " + sliceImage.Arc);
            sliceImage.SetArc(sliceImage.Arc + Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        sliceImage.SetArc(fillAmount);
        
        yield return new WaitForSeconds(0.2f);
    }
    */
}
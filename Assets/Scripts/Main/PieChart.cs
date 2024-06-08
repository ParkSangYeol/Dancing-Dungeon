using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshProUGUI 사용을 위해 추가
using System.Collections.Generic;

public class PieChart : MonoBehaviour
{
    public GameObject slicePrefab; // 파이 조각을 나타내는 프리팹
    public Transform chartContainer; // 그래프를 표시할 컨테이너
    public Font customFont; // 에디터에서 할당할 폰트
    public RectTransform graphAnchor; // 그래프의 위치를 지정할 빈 게임오브젝트

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
    void Start()
    {
    
    }

    private void OnEnable()
    {
        ShowPieChart();
    }

    private void ShowPieChart()
    {
        perfectCount = combatSceneUIManager.GetPerfectCombo;
        greatCount = combatSceneUIManager.GetGreatCombo;
        badCount = combatSceneUIManager.GetBadCombo;
        missCount = combatSceneUIManager.GetMissCombo;
        float total = perfectCount + greatCount + badCount + missCount;

        if (total == 0)
        {
            Debug.LogWarning("Total count is zero, no data to display in the pie chart.");
            return;
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

        CreateSlice("Perfect", perfectRatio, Color.green, ref zRotation);
        CreateSlice("Great", greatRatio, Color.blue, ref zRotation);
        CreateSlice("Bad", badRatio, Color.yellow, ref zRotation);
        CreateSlice("Miss", missRatio, Color.red, ref zRotation);

        // 그래프 위치 조정
        if (graphAnchor != null)
        {
            RectTransform chartRectTransform = chartContainer.GetComponent<RectTransform>();
            chartRectTransform.anchoredPosition = graphAnchor.anchoredPosition;
        }
    }

    private void CreateSlice(string label, float fillAmount, Color color, ref float zRotation)
    {
        if (fillAmount <= 0) return;

        GameObject slice = Instantiate(slicePrefab, chartContainer);
        Image sliceImage = slice.GetComponent<Image>();
        sliceImage.fillAmount = fillAmount;
        sliceImage.color = color;

        RectTransform rectTransform = slice.GetComponent<RectTransform>();
        rectTransform.localRotation = Quaternion.Euler(new Vector3(0, 0, zRotation));
        zRotation -= fillAmount * 360f;
    }
}
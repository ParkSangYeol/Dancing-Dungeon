using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TextButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Color normalColor = Color.white;
    public Color hoverColor = Color.yellow;

    private TextMeshProUGUI buttonText;

    void Start()
    {
        
        buttonText = GetComponentInChildren<TextMeshProUGUI>();

        
        if (buttonText != null)
        {
            buttonText.color = normalColor;
        }
    }

    // 마우스가 버튼에 들어왔을 때 호출되는 메서드
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (buttonText != null)
        {
            buttonText.color = hoverColor;
        }
    }

    // 마우스가 버튼에서 나갔을 때 호출되는 메서드
    public void OnPointerExit(PointerEventData eventData)
    {
        if (buttonText != null)
        {
            buttonText.color = normalColor;
        }
    }
}

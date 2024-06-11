using UnityEngine;
using DG.Tweening;

public class TitleMove : MonoBehaviour
{
    private RectTransform title; // UI Text 또는 TMP Text의 RectTransform

    void Start()
    {
        title = GetComponent<RectTransform>();
        AnimateTitle();
    }

    void AnimateTitle()
    {
        // 초기 위치 저장
        Vector2 initialPosition = title.anchoredPosition;

        // 화면 상단에서 살짝 튕기는 위치 계산
        Vector2 bouncePosition = new Vector2(initialPosition.x, initialPosition.y + 500);

        // 목표 위치 설정
        Vector2 targetPosition = new Vector2(initialPosition.x, initialPosition.y);

        // DoTween Sequence를 사용하여 애니메이션 설정
        Sequence sequence = DOTween.Sequence();

        // 화면 상단에서 살짝 튕기는 효과를 위해 첫 번째 단계 추가 (위로 이동)
        sequence.Append(title.DOAnchorPos(bouncePosition, 1f).SetEase(Ease.OutBounce));

        // 최종 목표 위치로 이동하는 단계 추가 (초기 위치로 돌아옴)
        sequence.Append(title.DOAnchorPos(targetPosition, 1f).SetEase(Ease.OutBounce));

        // 애니메이션 실행
        sequence.Play();
    }
}

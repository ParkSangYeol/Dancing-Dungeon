using System.Collections;
using Sirenix.OdinInspector;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using NUnit.Framework.Constraints;

public class HitScanByRay : MonoBehaviour
{
    [InfoBox("Player 게임 오브젝트를 넣어주세요!", InfoMessageType.Error, "IsPlayerInputNotSet")]
    [SerializeField] 
    private PlayerInput playerInput;
    private InputAction moveAction;
    [InfoBox("UiParticle 게임 오브젝트를 넣어주세요!", InfoMessageType.Error, "UiParticleSystem NotSet")]
    [SerializeField] 
    private NoteParticleSystem noteParticleSystem;
    [SerializeField] CombatSceneUIManager combatSceneUIManager;
    private bool missHapppen = false;
    

    public UnityEvent<Vector2,string> OnPressedKey;
    public UnityEvent<string> OnTimingHit;
    private string currentHit = "";
    
    
    void Awake()
    {
        if (playerInput == null) {
            Debug.LogError("PlayerInput을 설정하지 않았습니다.");
            return;
        }
        if (noteParticleSystem == null) {
            Debug.LogError("UIParticleManager를 설정하지 않았습니다.");
            return;
        }
        moveAction = playerInput.actions["Move"];
        moveAction.started += CheckNote;
        if (OnTimingHit != null && noteParticleSystem != null)
        {
            OnTimingHit.AddListener(noteParticleSystem.PlayParticle);
        }
        else
        {
            Debug.LogWarning("OnTimingHit or NoteParticleSystem is not set or has been destroyed.");
        }

        
    }
    private void Start() {
       
    }

    void Update()
    {
        
    }

    private void OnEnable()
    {
        if (playerInput != null)
        {
            moveAction = playerInput.actions["Move"];
            if (moveAction != null)
            {
                moveAction.started += CheckNote;
                moveAction.Enable();
            }
            else
            {
                Debug.LogWarning("Move action is not found in PlayerInput.");
            }
        }
        else
        {
            Debug.LogWarning("PlayerInput is not set.");
        }
}


    private void OnDisable()
    {
        if (moveAction != null)
        {
            moveAction.Disable();
        }
        OnTimingHit.RemoveAllListeners();
    }

  void CheckNote(InputAction.CallbackContext context)
{
    if(missHapppen==false)
    {
        // 현재 게임 오브젝트가 유효한지 확인
        if (this == null || !gameObject.activeInHierarchy)
        {
            Debug.LogWarning("HitScanByRay is not active or has been destroyed.");
            return;
        }

        Vector2 moveDir = context.ReadValue<Vector2>();

        RaycastHit2D lefthit = Physics2D.Raycast(transform.position + Vector3.right * 300, Vector2.left, Mathf.Infinity, 1 << LayerMask.NameToLayer("LeftNote"));
        RaycastHit2D righthit = Physics2D.Raycast(transform.position + Vector3.left * 300, Vector2.right, Mathf.Infinity, 1 << LayerMask.NameToLayer("RightNote"));

        // 레이캐스트 충돌체 확인
        if (lefthit.collider == null && righthit.collider == null) return;
        if (lefthit.collider != null && righthit.collider != null) 
        {
            if ((lefthit.collider.CompareTag("LeftNote") && righthit.collider.CompareTag("RightNote") ) || 
                (lefthit.collider.CompareTag("RightNote") && righthit.collider.CompareTag("LeftNote")))
            {
                float left_x = lefthit.collider.transform.position.x;
                float right_x = righthit.collider.transform.position.x;
                float xDifference = right_x - left_x;
                    
                if(xDifference >= -200 && xDifference <= 200)
                {
                    currentHit = "Perfect";
                    Debug.Log("Perfect : Left X : " + left_x + " Right X : " + right_x + " Difference " + xDifference);
                    OnTimingHit?.Invoke(currentHit);
                    OnPressedKey?.Invoke(moveDir,currentHit);
                    lefthit.collider.gameObject.SetActive(false);
                    righthit.collider.gameObject.SetActive(false);
                    combatSceneUIManager.SetCombo(currentHit);

                }
                else if ((xDifference > 200 && xDifference <= 600) || (xDifference < -200 && xDifference >= -600))
                {
                    currentHit = "Great";
                    OnTimingHit?.Invoke(currentHit);
                    OnPressedKey?.Invoke(moveDir,currentHit);
                    lefthit.collider.gameObject.SetActive(false);
                    righthit.collider.gameObject.SetActive(false);
                
                    combatSceneUIManager.SetCombo(currentHit);
                }
                else if (xDifference > 600 && xDifference <= 1300)
                {
                    OnTimingHit?.Invoke(currentHit);
                    currentHit = "Bad";
                    combatSceneUIManager.SetCombo(currentHit);
                }
                else if (xDifference > 1300)
                {
                    OnTimingHit?.Invoke(currentHit);
                    Debug.Log("Miss : Left X : " + left_x + " Right X : " + right_x + " Difference " + xDifference);
                    currentHit = "Miss";
                    combatSceneUIManager.SetCombo(currentHit);
                }
                    
                
            }
        }
    }
}


    #region Odin 

    private bool IsPlayerInputNotSet() {
        return playerInput == null;
    }
    public void HappenMiss()
    {
        StartCoroutine(WaitMiss());
    }
    IEnumerator WaitMiss()
    {
        missHapppen=true;
        yield return new WaitForSeconds(0.3f);
        Debug.Log("쿄루틴 실행됨");
        missHapppen=false;
    }

    #endregion
}
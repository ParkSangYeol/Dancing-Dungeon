using System.Collections;
using Sirenix.OdinInspector;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class HitScanByRay : MonoBehaviour
{
    [InfoBox("Player 게임 오브젝트를 넣어주세요!", InfoMessageType.Error, "IsPlayerInputNotSet")]
    [SerializeField] 
    private PlayerInput playerInput;
    private InputAction moveAction;
    [InfoBox("UiParticle 게임 오브젝트를 넣어주세요!", InfoMessageType.Error, "UiParticleSystem NotSet")]
    [SerializeField] 
    private NoteParticleSystem noteParticleSystem;
    private int combo=0;
    

    public UnityEvent<Vector2> OnPressedKey;
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
        OnTimingHit.AddListener(noteParticleSystem.PlayParticle);
    }

    void Update()
    {
        Debug.DrawRay(transform.position, Vector2.left * 1000, Color.green, 2.0f);
        Debug.DrawRay(transform.position, Vector2.right * 1000, Color.red, 2.0f);
    }

    private void OnEnable()
    {
        moveAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
    }

    void CheckNote(InputAction.CallbackContext context)
    {
        Vector2 moveDir = context.ReadValue<Vector2>();

        RaycastHit2D lefthit = Physics2D.Raycast(transform.position + Vector3.right * 300, Vector2.left, Mathf.Infinity, 1 << LayerMask.NameToLayer("LeftNote"));
        RaycastHit2D righthit = Physics2D.Raycast(transform.position + Vector3.left * 300, Vector2.right, Mathf.Infinity, 1 << LayerMask.NameToLayer("RightNote"));
        
        if (lefthit.collider != null && righthit.collider != null) 
        {
            if ((lefthit.collider.CompareTag("LeftNote") && righthit.collider.CompareTag("RightNote") )|| (lefthit.collider.CompareTag("RightNote") && righthit.collider.CompareTag("LeftNote")))
            {
                float left_x = lefthit.collider.transform.position.x;
                float right_x = righthit.collider.transform.position.x;
                float xDifference = right_x - left_x;
                
                
                if(xDifference>=-200 && xDifference<=200)  
                {
                    Debug.Log("Perfect : Left X : "+left_x+" Right X : " + right_x+" Difference" +xDifference);
                    lefthit.collider.gameObject.SetActive(false);
                    righthit.collider.gameObject.SetActive(false);
                    currentHit="Perfect";
                    OnPressedKey.Invoke(moveDir);
                    UiManager.instance.PlayPerfectSound();
    
                    combo+=1;
                    UiManager.instance.SetCombo(combo,currentHit);
                }
                else if((xDifference>200 && xDifference<=600) || (xDifference < -200 && xDifference >= -600))
                {
                   currentHit = "Great";
                   OnPressedKey.Invoke(moveDir);// 플레이어 이동
                   UiManager.instance.PlayGreatSound();//효과음 발동
                   lefthit.collider.gameObject.SetActive(false);//노트 비활성화
                   righthit.collider.gameObject.SetActive(false);//노트 비활성화
                   combo+=1;
                   UiManager.instance.SetCombo(combo,currentHit);
                }
                else if(xDifference>600 && xDifference<=1300)
                {
                    currentHit = "Bad";
                    UiManager.instance.PlayBadSound();
                    combo=0;
                    UiManager.instance.SetCombo(combo,currentHit);
                }
                else if(xDifference>1300)
                {
                    Debug.Log("Miss : Left X : "+left_x+" Right X : " + right_x+" Difference" +xDifference);
                    currentHit = "Miss";
                    UiManager.instance.PlayMissSound();
                    combo=0;
                    UiManager.instance.SetCombo(combo,currentHit);
                }
                
                Debug.Log(currentHit);
                OnTimingHit.Invoke(currentHit);
                
            }
        }
    }
    public int GetCombo()
    {
        return combo;
    }

    #region Odin 

    private bool IsPlayerInputNotSet() {
        return playerInput == null;
    }

    #endregion
}

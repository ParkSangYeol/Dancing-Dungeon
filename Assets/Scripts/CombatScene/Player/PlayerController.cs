using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace  CombatScene.Player
{
    public class PlayerController : MonoBehaviour
    {
        [Title("Components")]
        [InfoBox("components의 경우 따로 설정하지 않으면 GetComponent를 호출합니다.", InfoMessageType.Info)]
        [SerializeField] 
        private PlayerInput playerInput;
        [SerializeField]
        private Animator animator;
        

        [Title("Data")] 
        [InfoBox("character data는 반드시 추가해야합니다!", InfoMessageType.Error, "IsCharacterDataNotSetup")]
        [SerializeField]
        private PlayerCharacterScriptableObject playerCharacterData;
        [InfoBox("default weapon은 반드시 추가해야합니다!", InfoMessageType.Error, "IsDefaultWeaponDataNotSetup")]
        [SerializeField]
        private WeaponScriptableObject defaultWeapon;

        [Title("Events")] 
        public UnityEvent OnPlayerDead;
        
        [Title("Variables")]
        private InputAction moveAction;

        [ShowInInspector]
        public int hp
        {
            get => _hp;
            set
            {
                _hp = value;
                if (_hp < 0)
                {
                    OnPlayerDead.Invoke();
                }
            }
        }
        private int _hp;
        
        [ShowInInspector]
        public float power { get => _power; set => _power = value < 0 ? 0 : value; }
        private float _power;
        
        [ShowInInspector]
        public float shield { get => _shield; set => _shield = value < 0 ? 0 : value; }
        private float _shield;
        
        [ShowInInspector]
        private WeaponScriptableObject equipWeapon;
        
        void Awake()
        {
            SetComponents();
            SetVariables();
            SetEvents();
        }

        private void OnEnable()
        {
            moveAction.Enable();
        }

        private void OnDisable()
        {
            moveAction.Disable();
        }
        
        private void MovePlayer(InputAction.CallbackContext context)
        {
            // TODO 박제에 맞춰 눌렀는지 확인
            // if ()
            {
                // 이동
                Vector2 moveVal = context.ReadValue<Vector2>();
                if (moveVal.x == 1 || moveVal.x == -1 || moveVal.y == 1 || moveVal.y == -1)
                {
                    if (Mathf.Abs(moveVal.x) == 1)
                    {
                        transform.localScale = new Vector3(-Mathf.Sign(moveVal.x), 1, 1);
                    } 
                    // 키보드를 하나만 입력. 실제 이동 구현
                    StartCoroutine(MoveTo(moveVal * ConstVariables.tileSize, 0.4f));
                }
                else
                {
                    // 두 개의 키를 동시에 입력
                    Debug.Log(GetType().Name + ": 두 개의 키를 동시에 눌렀습니다.");
                }
            }
        }

        /// <summary>
        /// 특정한 타겟으로 플레이어를 일정시간동안 이동시키는 코루틴
        /// </summary>
        /// <param name="addPos"> 이동시킬 타겟 위치(현재 위치에서 더해주는 값)</param>
        /// <param name="duration"> 이동 시간(초)</param>
        /// <returns></returns>
        IEnumerator MoveTo(Vector2 addPos, float duration)
        {
            // 플레이어 이동 애니메이션 재생
            animator.SetTrigger("Move");
            // 이동
            Vector2 startPos = transform.position;
            Vector2 targetPos = (Vector2)transform.position + addPos;
            float time = 0;
            while (duration > time)
            {
                transform.position = Vector2.Lerp(startPos, targetPos, time / duration);
                time += Time.deltaTime;
                yield return null;
            }
            transform.position = targetPos;
        }

        #region Init

        private void SetComponents()
        {
            // set Player Input
            if (playerInput == null)
            {
                playerInput = GetComponent<PlayerInput>();
            }

            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }
        }

        private void SetVariables()
        {
            // 컨트롤러 내부 변수 설정
            if (playerCharacterData == null)
            {
                Debug.LogError(gameObject.name + ": PlayerCharacterData가 없습니다!");
            }
            if (defaultWeapon == null)
            {
                Debug.LogError(gameObject.name + ": 기본 무기가 없습니다!");
            }
            
            this.hp = playerCharacterData.hp;
            this.power = playerCharacterData.defaultPower;
            this.shield = playerCharacterData.shield;
            this.equipWeapon = defaultWeapon;
            
            moveAction = playerInput.actions["Move"];
            
        }

        private void SetEvents()
        {
            moveAction.started += MovePlayer;
            
            OnPlayerDead.AddListener(() =>
            {
                animator.SetTrigger("Dead");
            });
        }
        
        #endregion

        #region Odin

        private bool IsCharacterDataNotSetup()
        {
            return playerCharacterData == null;
        }

        private bool IsDefaultWeaponDataNotSetup()
        {
            return defaultWeapon == null;
        }
        #endregion
    }
}
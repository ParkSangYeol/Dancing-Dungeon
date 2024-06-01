using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace  CombatScene.Player
{
    public class PlayerController : MonoBehaviour
    {
        #region Variables

        [Title("Components")]
        [InfoBox("components의 경우 따로 설정하지 않으면 GetComponent를 호출합니다.", InfoMessageType.Info)]
        [SerializeField]
        private PlayerInput playerInput;

        [SerializeField] private Animator animator;

        [InfoBox("GameManager의 MapHandler를 추가해주세요!", InfoMessageType.Error, "IsMapHandlerNotSetup")] [SerializeField]
        private MapHandler mapHandler;

        [InfoBox("GameManager의 CombatManager를 추가해주세요!", InfoMessageType.Error, "IsCombatManagerNotSetup")]
        [SerializeField]
        private CombatManager combatManager;

        [InfoBox("자식 컴포넌트의 UnityRoot를 추가해주세요.", InfoMessageType.Info)] [SerializeField]
        private Transform unitRoot;


        [Title("Data")]
        [InfoBox("character data는 반드시 추가해야합니다!", InfoMessageType.Error, "IsCharacterDataNotSetup")]
        [SerializeField]
        private PlayerCharacterScriptableObject playerCharacterData;

        [InfoBox("default weapon은 반드시 추가해야합니다!", InfoMessageType.Error, "IsDefaultWeaponDataNotSetup")] [SerializeField]
        private WeaponScriptableObject defaultWeapon;

        [Title("Events")] public UnityEvent OnPlayerDead;

        [Title("Variables")] private InputAction moveAction;

        [ShowInInspector]
        public float hp
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

        private float _hp;

        [ShowInInspector]
        public float power
        {
            get => _power;
            set => _power = value < 0 ? 0 : value;
        }

        private float _power;

        [ShowInInspector]
        public float shield
        {
            get => _shield;
            set => _shield = value < 0 ? 0 : value;
        }

        private float _shield;

        [ShowInInspector] private WeaponScriptableObject equipWeapon;

        #endregion

        #region EventMethods

        void Awake()
        {
            SetComponents();
            SetVariables();
            SetEvents();
        }

        private void Start()
        {
            SetExternalComponent();
        }

        private void OnEnable()
        {
            moveAction.Enable();
        }

        private void OnDisable()
        {
            moveAction.Disable();
        }

        #endregion


        private void MovePlayerForTest(InputAction.CallbackContext context)
        {
            // 이동
            Vector2 moveVal = context.ReadValue<Vector2>();
            if (moveVal.x == 1 || moveVal.x == -1)
            {
                unitRoot.localScale = new Vector3(-Mathf.Sign(moveVal.x), 1, 1);
                if (mapHandler.GetPoint((Vector2)transform.position + moveVal).Equals(ObjectType.Load))
                {
                    // 목표 타일이 도로인 경우 이동
                    StartCoroutine(MoveTo(moveVal * ConstVariables.tileSizeX, 0.4f));
                }
            }
            else if (moveVal.y == 1 || moveVal.y == -1)
            {
                if (mapHandler.GetPoint((Vector2)transform.position + moveVal).Equals(ObjectType.Load))
                {
                    // 목표 타일이 도로인 경우 이동
                    StartCoroutine(MoveTo(moveVal * ConstVariables.tileSizeY, 0.4f));
                }
            }
            else
            {
                // 두 개의 키를 동시에 입력
                Debug.Log(GetType().Name + ": 두 개의 키를 동시에 눌렀습니다.");
            }
        }

        public void MovePlayer(Vector2 moveVal)
        {
            // 이동
            if (moveVal.x == 1 || moveVal.x == -1)
            {
                unitRoot.localScale = new Vector3(-Mathf.Sign(moveVal.x), 1, 1);
                if (mapHandler.GetPoint((Vector2)transform.position + moveVal).Equals(ObjectType.Load))
                {
                    // 목표 타일이 도로인 경우 이동
                    StartCoroutine(MoveTo(moveVal * ConstVariables.tileSizeX, 0.4f));
                }
            }
            else if (moveVal.y == 1 || moveVal.y == -1)
            {
                if (mapHandler.GetPoint((Vector2)transform.position + moveVal).Equals(ObjectType.Load))
                {
                    // 목표 타일이 도로인 경우 이동
                    StartCoroutine(MoveTo(moveVal * ConstVariables.tileSizeY, 0.4f));
                }
            }
            else
            {
                // 두 개의 키를 동시에 입력
                Debug.Log(GetType().Name + ": 두 개의 키를 동시에 눌렀습니다.");
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
            combatManager.MovePlayer(targetPos);
            float time = 0;
            while (duration > time)
            {
                transform.position = Vector2.Lerp(startPos, targetPos, time / duration);
                time += Time.deltaTime;
                yield return null;
            }

            transform.position = targetPos;
        }

        #region About Combat

        public void Attacked(float damage)
        {
            hp -= Mathf.Max(1, damage - shield);
            if (hp > 0)
            {
                // 캐릭터가 아직 생존 중.
                // animator.SetTrigger("Hit");
            }
        }

        public WeaponScriptableObject GetEquippedWeapon()
        {
            return equipWeapon;
        }
        
        public void Attack()
        {
            animator.SetTrigger("Attack");    
        }

        public float GetPower()
        {
            return equipWeapon.power + playerCharacterData.defaultPower;
        }
    #endregion

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
                animator = transform.GetChild(0).GetComponent<Animator>();
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
        {            moveAction.started += MovePlayerForTest;

#if TEST_MOVE_WITHOUT_NOTE
#endif
            OnPlayerDead.AddListener(() =>
            {
                animator.SetTrigger("Dead");
            });
        }

        private void SetExternalComponent()
        {
            if (unitRoot == null)
            {
                unitRoot = transform.GetChild(0);
            }

            if (mapHandler == null)
            {
                mapHandler = transform.Find("GameManager").GetComponent<MapHandler>();
            }
            if (combatManager == null)
            {
                combatManager = transform.Find("GameManager").GetComponent<CombatManager>();
            }
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
        
        private bool IsMapHandlerNotSetup()
        {
            return mapHandler == null;
        }
        
        private bool IsCombatManagerNotSetup()
        {
            return combatManager == null;
        }
        #endregion
    }
}
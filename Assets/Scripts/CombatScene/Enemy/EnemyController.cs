using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace CombatScene.Enemy
{
    public class EnemyController : MonoBehaviour
    {
        #region Variables
        
        [Title("Components")]
        [InfoBox("GameManager의 CombatManager를 추가해주세요!", InfoMessageType.Error, "IsCombatManagerNotSetup")]
        [SerializeField]
        private CombatManager combatManager;
        [SerializeField]
        private Animator animator;
        [InfoBox("자식 컴포넌트의 UnityRoot를 추가해주세요.", InfoMessageType.Info)]
        [SerializeField]
        private Transform unitRoot;
        
        [Title("Data")] 
        [InfoBox("character data는 반드시 추가해야합니다!", InfoMessageType.Error, "IsCharacterDataNotSetup")]
        [SerializeField]
        private EnemyCharacterScriptableObject characterData;
        [InfoBox("default weapon은 반드시 추가해야합니다!", InfoMessageType.Error, "IsDefaultWeaponDataNotSetup")]
        [SerializeField]
        private WeaponScriptableObject defaultWeapon;
            
        
        [Title("Events")] 
        public UnityEvent OnEnemyDead;
        
        [Title("Variables")]

        [ShowInInspector]
        public float hp
        {
            get => _hp;
            set
            {
                _hp = value;
                if (_hp < 0)
                {
                    OnEnemyDead.Invoke();
                }
            }
        }
        private float _hp;
        
        [ShowInInspector]
        public float power { get => _power; set => _power = value < 0 ? 0 : value; }
        private float _power;
        
        [ShowInInspector]
        public float shield { get => _shield; set => _shield = value < 0 ? 0 : value; }
        private float _shield;
        
        [ShowInInspector]
        private WeaponScriptableObject equipWeapon;

        #endregion

        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            SetComponent();
            SetVariables();
        }

        public void MoveCharacter(Vector2 targetPosition)
        {
            Vector2 moveVec = transform.position - (Vector3)targetPosition;
            if (Mathf.Abs(moveVec.x) != 0)
            {
                unitRoot.localScale = new Vector3(Mathf.Sign(moveVec.x), 1, 1);
            }
            StartCoroutine(MoveTo(targetPosition, 0.25f));
        }
        
        /// <summary>
        /// 특정한 타겟으로 플레이어를 일정시간동안 이동시키는 코루틴
        /// </summary>
        /// <param name="targetPosition"> 이동시킬 타겟 위치(현재 위치에서 더해주는 값)</param>
        /// <param name="duration"> 이동 시간(초)</param>
        /// <returns></returns>
        IEnumerator MoveTo(Vector2 targetPosition, float duration)
        {
            // 플레이어 이동 애니메이션 재생
            animator.SetTrigger("Move");
            // 이동
            Vector2 startPos = transform.position;
            Vector2 targetPos = targetPosition;
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
            Debug.Log(gameObject.name + "'s hp is " + hp);
        }

        public bool CanAttack(Vector2 targetPos)
        {
            Vector2 distanceVec = new Vector2(Mathf.Abs(targetPos.x - transform.position.x), Mathf.Abs(targetPos.y - transform.position.y)); 
            int distance = int.MaxValue;
            switch (equipWeapon.attackDirection)
            {
                case AttackDirection.DIR_4:
                    distance = (int)(distanceVec.x + distanceVec.y);
                    break;
                case AttackDirection.DIR_8:
                    distance = (int)Mathf.Min(distanceVec.x, distanceVec.y);
                    distanceVec -= new Vector2(distance, distance);
                    distance += (int)(distanceVec.x + distanceVec.y);
                    break;
            }

            return distance <= equipWeapon.range;
        }

        public float GetPower()
        {
            return equipWeapon.power + characterData.defaultPower;
        }

        public void Attack()
        {
            animator.SetTrigger("Attack");    
        }
        
        public void DestroyCharacter()
        {
            // 애니메이션에서 이벤트로 호출
            Destroy(this.gameObject);
        }

        #endregion
        
        #region Init

        private void SetComponent()
        {
            if (combatManager == null)
            {
                combatManager = transform.Find("GameManager").GetComponent<CombatManager>();
            }
            combatManager.AddEnemy(transform.position, this);
            
            if (animator == null)
            {
                animator = transform.GetChild(0).GetComponent<Animator>();
            }
        }

        private void SetVariables()
        {
            if (characterData == null)
            {
                Debug.LogError(gameObject.name + ": characterData가 없습니다!");
            }
            if (defaultWeapon == null)
            {
                Debug.LogError(gameObject.name + ": 기본 무기가 없습니다!");
            }
            
            this.hp = characterData.hp;
            this.power = characterData.defaultPower;
            this.shield = characterData.shield;
            this.equipWeapon = defaultWeapon;
        }

        #endregion

        #region Odin

        private bool IsCharacterDataNotSetup()
        {
            return characterData == null;
        }

        private bool IsDefaultWeaponDataNotSetup()
        {
            return defaultWeapon == null;
        }
        
        private bool IsCombatManagerNotSetup()
        {
            return combatManager == null;
        }
        
        #endregion
    }
}
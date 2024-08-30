//#define TEST_MOVE_WITHOUT_NOTE

using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor.Localization.Plugins.XLIFF.V20;
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
        protected CombatManager combatManager;
        [SerializeField]
        private Animator animator;
        [InfoBox("자식 컴포넌트의 UnityRoot를 추가해주세요.", InfoMessageType.Info)]
        public Transform unitRoot;
        
        [InfoBox("\"UnitRoot/Root/BodySet/P_Body/ArmSet/ArmR/P_RArm/P_Weapon/R_Weapon\"을 넣어주세요.", InfoMessageType.Info)] 
        [SerializeField] 
        private SpriteRenderer weaponSpriteRenderer;

        [Title("Data")] 
        [InfoBox("character data는 반드시 추가해야합니다!", InfoMessageType.Error, "IsCharacterDataNotSetup")]
        [SerializeField]
        protected EnemyCharacterScriptableObject characterData;
        [InfoBox("default weapon은 반드시 추가해야합니다!", InfoMessageType.Error, "IsDefaultWeaponDataNotSetup")]
        [SerializeField]
        protected WeaponScriptableObject defaultWeapon;
        [InfoBox("WeaponScriptableObject은 반드시 추가해야합니다!", InfoMessageType.Error, "WeaponScriptableObject")]
            
        
        [Title("Events")] 
        public UnityEvent<Vector2> OnEnemyDead;
        
        [Title("Variables")] 
        private int currentAttackDelay;
        private int currentDelayedAttackDelay;
        private bool isDelayedAttackActive;
        private List<Vector2> delayedAttackPositions;
        public UnityEvent<float> onHitEvent;
        public ObjectType tileObjectType { get; private set; }
        
        [ShowInInspector]
        public float hp
        {
            get => _hp;
            set
            {
                _hp = value;
                if (_hp < 0)
                {
                    OnEnemyDead.Invoke(this.transform.position);
                    
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
        protected void Start()
        {
            combatManager = FindAnyObjectByType<CombatManager>();
            SetComponent();
#if TEST_MOVE_WITHOUT_NOTE
            SetVariables(); // 스폰할때마다 해주는 것으로 해결, 시간이 지남에 따라 적군이 강해지도록 하기위함.
#endif
            if (this as BossController)
            {
                SetVariables();
            }
            SetEvent();
            SetExternalComponent();
            Debug.Log("Enemy Power : " + this._power);
        }

        public void MoveCharacter(Vector2 targetPosition)
        {
            if(_hp>0)
            {
                Vector2 moveVec = transform.position - (Vector3)targetPosition;
                if (Mathf.Abs(moveVec.x) != 0)
                {
                    unitRoot.localScale = new Vector3(Mathf.Sign(moveVec.x), 1, 1);
                }
                StartCoroutine(MoveTo(targetPosition, 0.25f));
            }
        }
        
        /// <summary>
        /// 특정한 타겟으로 플레이어를 일정시간동안 이동시키는 코루틴
        /// </summary>
        /// <param name="targetPosition"> 이동시킬 타겟 위치(현재 위치에서 더해주는 값)</param>
        /// <param name="duration"> 이동 시간(초)</param>
        /// <returns></returns>
        protected IEnumerator MoveTo(Vector2 targetPosition, float duration)
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
        public void AddEnemyToSpawn()
        {
            combatManager.AddEnemy(transform.position,this);
        }
        
        public void Attacked(float damage)
        {
            hp -= Mathf.Max(1, damage - shield);
            if (hp > 0)
            {
                // 캐릭터가 아직 생존 중.
                // animator.SetTrigger("Hit");
                onHitEvent.Invoke(hp);
            }
            Debug.Log(gameObject.name + "'s hp is " + hp);
           
        }

        public void EquipWeapon(WeaponScriptableObject weapon)
        {
            if (weapon == null)
            {
                return;
            }

            equipWeapon = weapon;
            weaponSpriteRenderer.sprite = equipWeapon.weaponSprite;
        }

        /// <summary>
        /// 캐릭터의 공격 여부를 반환하는 함수
        /// </summary>
        /// <param name="targetPos">공격할 대상의 위치</param>
        /// <param name="attackDelaying">현재 캐릭터가 공격 후 딜레이 상태인지를 확인</param>
        /// <param name="delayedAttackDelaying">현재 캐릭터가 딜레이 공격으로 딜레이 상태인지 확인</param>
        /// <returns> 공격 가능여부를 반환 </returns>
        public bool CanAttack(Vector2 targetPos, out bool attackDelaying, out bool delayedAttackDelaying)
        {
            delayedAttackDelaying = false;
            attackDelaying = false;
            if (currentAttackDelay-- > 0)
            {
                attackDelaying = true;
                return false;
            }

            if (currentDelayedAttackDelay > 0)
            {
                delayedAttackDelaying = true;
                return false;
            }

            if (isDelayedAttackActive)
            {
                attackDelaying = true;
                return false;
            }
            
            
            Vector2 distanceVec = new Vector2(Mathf.Abs(targetPos.x - transform.position.x), Mathf.Abs(targetPos.y - transform.position.y)); 
            int distance = int.MaxValue;
            switch (equipWeapon.attackDirection)
            {
                case AttackDirection.DIR_4:
                    if (distanceVec.x == 0 || distanceVec.y == 0)
                    {
                        distance = (int)Mathf.Max(distanceVec.x ,distanceVec.y);
                    }
                    else
                    {
                        distance = Int32.MaxValue;
                    }
                    break;
                case AttackDirection.DIR_8:
                    distance = (int)Mathf.Min(distanceVec.x, distanceVec.y);
                    distanceVec -= new Vector2(distance, distance);
                    distance += (int)(distanceVec.x + distanceVec.y);
                    break;
            }
            
            if (distance <= equipWeapon.range && !isDelayedAttackActive)
            {
                if (equipWeapon.isDelayedAttack)
                {
                    Vector2 incVec = new Vector2(targetPos.x - transform.position.x, targetPos.y - transform.position.y); 

                    // 지연 공격을 진행
                    Vector2 dirVec = (Mathf.Abs(incVec.x) > Mathf.Abs(incVec.y)) ? new Vector2(Mathf.Sign(incVec.x), 0) : new Vector2(0, Mathf.Sign(incVec.y));
            
                    switch (equipWeapon.attackDirection)
                    {
                        case AttackDirection.DIR_4:
                            for (int i = 1; i <= equipWeapon.range; i++)
                            {
                                // 공격 범위의 값 가져오기
                                Vector2 targetPosition = (Vector2) this.transform.position + dirVec * i;
                                // 바닥 타일 변경
                                combatManager.attackFocusPool.PlaceAttackFocus(targetPosition);
                                
                                delayedAttackPositions.Add(targetPosition);
                            }
                            break;
                        case AttackDirection.DIR_8:
                            for (int i = 1; i <= equipWeapon.range; i++)
                            {
                                for (int j = -i; j <= i; j++)
                                {
                                    Vector2 targetPosition = (Vector2)this.transform.position + dirVec * i;
                                    if (dirVec.x != 0)
                                    {
                                        targetPosition.y += j;
                                    }
                                    else if (dirVec.y != 0)
                                    {
                                        targetPosition.x += j;
                                    }
                                    
                                    // 바닥 타일 변경
                                    combatManager.attackFocusPool.PlaceAttackFocus(targetPosition);
                                    
                                    delayedAttackPositions.Add(targetPosition);
                                }
                            }
                            break;
                    }
                    isDelayedAttackActive = true;
                    delayedAttackDelaying = true;
                    currentDelayedAttackDelay = equipWeapon.delayedAttackDelay;
                    return false;
                }
            
                currentAttackDelay = equipWeapon.attackDelay;
                return true;
            }

            return false;
        }

        public bool DelayedAttack(out List<Vector2> attackPositions)
        {
            if (!isDelayedAttackActive || --currentDelayedAttackDelay > 0)
            {
                attackPositions = null;
                return false;
            }
            this.Attack();
            attackPositions = delayedAttackPositions;
            isDelayedAttackActive = false;
            
            return true;
        }

        protected void CancelDelayedAttack()
        {
            foreach (var position in delayedAttackPositions)
            {
                combatManager.attackFocusPool.ReturnAttackFocus(position);
            }
            delayedAttackPositions.Clear();
        }
        public float GetPower()
        {
            return equipWeapon.power + characterData.defaultPower;
        }
        
        public void Attack()
        {
            animator.SetTrigger("Attack");    
        }
        
        public WeaponScriptableObject GetEquippedWeapon()
        {
            return equipWeapon;
        }

        #endregion
        
        #region Init

        private void SetComponent()
        {
            if (combatManager == null)
            {
                combatManager = GameObject.Find("GameManager").GetComponent<CombatManager>();
            }
            
#if TEST_MOVE_WITHOUT_NOTE
            combatManager.AddEnemy(transform.position, this);
#endif
            
            if (animator == null)
            {
                animator = transform.GetChild(0).GetComponent<Animator>();
            }
        }
        public void SetCombatManager(CombatManager combatmanager)
        {
            this.combatManager=combatmanager;
        }

        public void SetVariables()
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
            this.tileObjectType = (this as BossController) ? ObjectType.Boss : ObjectType.Enemy;
            
            
            this.currentAttackDelay = 0;
            this.isDelayedAttackActive = false;
            this.delayedAttackPositions = new List<Vector2>();
        }
        public void SetVariabePowerup(int upCapcity)// 스폰때마다 일정 조건마다 강해지도록함.
        {
            this.hp = characterData.hp+upCapcity+upCapcity;
            this.power = characterData.defaultPower+upCapcity;
            this.shield = characterData.shield+upCapcity;
            this.equipWeapon = defaultWeapon;
            this.tileObjectType = (this as BossController) ? ObjectType.Boss : ObjectType.Enemy;
            
            this.currentAttackDelay = 0;
            this.isDelayedAttackActive = false;
            this.delayedAttackPositions = new List<Vector2>();
        }
        private void SetEvent()
        {
            OnEnemyDead.AddListener((enemyTransform) =>
            {
                animator.SetTrigger("Dead");
                _hp=hp;
            });
            
            OnEnemyDead.AddListener((enemyTransform) =>
            {
                CancelDelayedAttack();
            });
        }
        private void SetExternalComponent()
        {
            if (unitRoot == null)
            {
                unitRoot = transform.GetChild(0);
            }
            
            if (weaponSpriteRenderer == null)
            {
                weaponSpriteRenderer = transform.Find("UnitRoot/Root/BodySet/P_Body/ArmSet/ArmR/P_RArm/P_Weapon/R_Weapon").GetComponent<SpriteRenderer>();
            } 
            weaponSpriteRenderer.sprite = equipWeapon.weaponSprite;
            combatManager.SetEnemyEvent((this as BossController) ?? this);
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
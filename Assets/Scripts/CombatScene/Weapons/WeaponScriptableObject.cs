using Sirenix.OdinInspector;
using UnityEngine;

namespace CombatScene
{
    [CreateAssetMenu(fileName = "WeaponScriptableObject", menuName = "Scriptable Objects/WeaponScriptableObject")]
    public class WeaponScriptableObject : ScriptableObject
    {
        [PreviewField(70, ObjectFieldAlignment.Right)]
        public Sprite thumbnail;
        [Title("무기 정보")] 
        [PreviewField(70, ObjectFieldAlignment.Right)]
        public Sprite weaponSprite;
        public string name;
        [Range(1, 3)]
        public int range;
        [Range(0, 500)]
        public int power;
        public AttackDirection attackDirection;
        [Range(0, 5)] 
        public int attackDelay;
        [OnValueChanged("DelayedAttackChecked")]
        public bool isDelayedAttack;
        [Range(1, 5)]
        [EnableIf("@isDelayedAttack")]
        public int delayedAttackDelay;
        [EnableIf("@!isDelayedAttack")]
        public bool isSplash;
        [AssetsOnly]
        public ParticleSystem VFX;
        
        public void DelayedAttackChecked()
        {
            if (isDelayedAttack)
            {
                isSplash = true;
            }
        }
    }

    public enum AttackDirection
    {
        DIR_8,
        DIR_4
    }
    
}
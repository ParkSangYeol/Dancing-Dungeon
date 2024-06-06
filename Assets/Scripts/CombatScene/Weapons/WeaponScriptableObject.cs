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
        [Range(0, 5)]
        public int power;
        public AttackDirection attackDirection;
        public bool isSplash;
        [AssetsOnly]
        public ParticleSystem VFX;
    }

    public enum AttackDirection
    {
        DIR_8,
        DIR_4
    }
    
}
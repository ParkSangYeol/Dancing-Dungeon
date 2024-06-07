using Sirenix.OdinInspector;
using UnityEngine;

namespace CombatScene
{
    [CreateAssetMenu(fileName = "ItemScriptableObject", menuName = "Scriptable Objects/ItemScriptableObject")]
    public class ItemScriptableObject : ScriptableObject
    {
        [EnumToggleButtons]
        public ItemType itemType;
        
        [DisableIf("@this.itemType == ItemType.WEAPON || this.itemType == ItemType.SPECIAL_WEAPON")]
        public float value;
        [EnableIf("@this.itemType == ItemType.WEAPON || this.itemType == ItemType.SPECIAL_WEAPON")]
        public WeaponScriptableObject weaponScriptableObject;

        [AssetsOnly] 
        public GameObject prefab;
        
        public int probability;
    }

    public enum ItemType
    {
        HEAL,
        POWER_UP,
        SHIELD,
        WEAPON,
        SPECIAL_WEAPON
    }
}

using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace CombatScene
{
    [CreateAssetMenu(fileName = "ItemListScriptableObject", menuName = "Scriptable Objects/ItemListScriptableObject")]
    public class ItemListScriptableObject : SerializedScriptableObject
    {
        public List<ItemScriptableObject> itemScriptableObjects;
        public Dictionary<WeaponScriptableObject, ItemScriptableObject> weaponItemMap = new Dictionary<WeaponScriptableObject, ItemScriptableObject>();

        private int sumProbability = -1;
        
        public ItemScriptableObject GetRandomItem()
        {
            if (sumProbability == -1)
            {
                GetSumOfProbability();
            }

            int select = Random.Range(0, sumProbability);
            
            foreach (var itemScriptableObject in itemScriptableObjects)
            {
                select -= itemScriptableObject.probability;
                if (select <= 0)
                {
                    return itemScriptableObject;
                }
            }

            return null;
        }

        public ItemScriptableObject GetWeaponItem(WeaponScriptableObject weaponScriptableObject)
        {
            ItemScriptableObject itemScriptableObject = null;
            weaponItemMap.TryGetValue(weaponScriptableObject, out itemScriptableObject);
            
            return itemScriptableObject;
        }
        
        private void GetSumOfProbability()
        {
            sumProbability = 0;
            foreach (var itemScriptableObject in itemScriptableObjects)
            {
                sumProbability += itemScriptableObject.probability;
            }
        }

        [Button]
        public void GetItemLists()
        {
            itemScriptableObjects.Clear();
            weaponItemMap.Clear();

            string currentPath = AssetDatabase.GetAssetPath(this);
            string currentDirectory = Path.GetDirectoryName(currentPath);

            string[] guids = AssetDatabase.FindAssets("t:scriptableobject", new[] { currentDirectory });

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                ItemScriptableObject scriptableObject = AssetDatabase.LoadAssetAtPath<ItemScriptableObject>(path);

                if (scriptableObject != null)
                {
                    itemScriptableObjects.Add(scriptableObject);
                    if (scriptableObject.itemType.Equals(ItemType.WEAPON) ||
                        scriptableObject.itemType.Equals(ItemType.SPECIAL_WEAPON))
                    {
                        weaponItemMap.Add(scriptableObject.weaponScriptableObject, scriptableObject);
                    }
                }
            }
        }
    }
}
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CombatScene
{
    public class ItemManager : MonoBehaviour
    {
        private Dictionary<Vector2, ItemInstance> droppedItems = new Dictionary<Vector2, ItemInstance>();
        
        [SerializeField] 
        private ItemListScriptableObject itemLists;
        [SerializeField]
        private CombatSceneUIManager combatSceneUIManager;
                
        
        public void PlaceItem(Vector2 placePosition, ItemScriptableObject itemScriptableObject)
        {
            if (droppedItems.ContainsKey(placePosition))
            {
                return;
            }
            GameObject dropGameObject = Instantiate(itemScriptableObject.prefab, placePosition, Quaternion.identity);
            ItemInstance itemInstance = dropGameObject.GetComponent<ItemInstance>();
            if (itemInstance == null)
            {
                itemInstance = dropGameObject.AddComponent<ItemInstance>();
            }
            itemInstance.itemScriptableObject = itemScriptableObject;
            droppedItems.Add(placePosition, itemInstance);
        }

        public bool GetDroppedItem(Vector2 position, out ItemInstance itemInstance)
        {
            if (droppedItems.TryGetValue(position, out itemInstance))
            {
                droppedItems.Remove(position);
                return true;
            }

            return false;
        }

        public ItemScriptableObject GetWeaponItem(WeaponScriptableObject weaponScriptableObject)
        {
            
            return itemLists.GetWeaponItem(weaponScriptableObject);
        }

        public void SpawnRandomItem(Vector2 placePosition)
        {
            ItemScriptableObject dropItem = itemLists.GetRandomItem();
            PlaceItem(placePosition, dropItem);
        }
    }
    
}
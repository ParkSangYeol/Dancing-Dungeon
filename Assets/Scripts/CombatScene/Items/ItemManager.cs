using System;
using System.Collections.Generic;
using CombatScene.System.Particle;
using Sirenix.OdinInspector;
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
        [SerializeField]
        [InfoBox("ParticleManager를 추가해주세요!", InfoMessageType.Error, "IsParticleManagerSetUp")]
        private ParticleManager particleManager;

        private void Start()
        {
            if (particleManager == null)
            {
                particleManager = GameObject.Find("GameManager").GetComponent<ParticleManager>();
            }
        }

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
                particleManager.PlayItemInteractParticle(position);
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

        #region Odin

        private bool IsParticleManagerSetUp()
        {
            return particleManager == null;
        }

        #endregion
    }
    
}
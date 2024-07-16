using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "ScriptableObjects/ItemDatabase", order = 1)]
public class ItemDatabase : ScriptableObject
{
    public List<Item> items;

    [System.Serializable]
    public class Item
    {
        public string itemID;
        public string itemName;
        public Sprite itemIcon;
        public string itemSpritePath; // Resources 경로
    }
}

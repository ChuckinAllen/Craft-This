using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PoschPlus.CraftingSystem
{
    //ToDo Turn this into a scriptableObject that is dynamicly created @ runtime.
    public class Inventory : MonoBehaviour
    {
        [Serializable]
        public class Items
        {
            [Tooltip("Simply the name of an item")]
            [field: SerializeField] public string Name { get; private set; }

            [Tooltip("The type of item data needed for a recipe.")]
            [field: SerializeField] public Ingredient Data { get; private set; }

            [Tooltip("Amount of this type of item.")]
            [field: SerializeField] public int Count { get; private set; }

            public void SetItemName(string name)
            {
                Name = name;
            }

            public void SetItemData(Ingredient data)
            {
                Data = data;
            }

            public void SetItemCount(int count)
            {
                Count = count;
            }

            public void IncreaseItemCount(int count)
            {
                Count += count;
            }

            public void DecreaseItemCount(int count)
            {
                Count -= count;
            }
        }


        [field: SerializeField]
        [field: ReadOnly]
        private Ingredient storedCraftableItem;

        public void SetCraftableItemToInvantoryToStore(Ingredient data)
        {
            storedCraftableItem = data;
        }

        public Ingredient GetCraftableItemToInvantoryToStore()
        {
            return storedCraftableItem;
        }

        [field: SerializeField]
        [field: ReadOnly]
        public List<Items> StoredInventoryItems { get; private set; } = new List<Items>();

        [HideInEditorMode]
        [Button("Add Item To Inventory")]
        public void AddItemToInventory(Ingredient data, int count = 1)
        {
            if (data == null) { Debug.LogWarning("No item was found, returning!"); return; }

            // if found existing item increment it
            Items existingItem = StoredInventoryItems.Find(i => i.Data == data);
            if (existingItem != null)
            {
                existingItem.IncreaseItemCount(count);
            }
            else
            {
                UpdateItemData(data, count);
            }
        }

        private void UpdateItemData(Ingredient data, int count)
        {
            Items newItem = new Items();
            newItem.SetItemData(data);
            newItem.SetItemName(data.name);
            newItem.SetItemCount(count);

            StoredInventoryItems.Add(newItem);
        }

        public void RemoveItem(Items item)
        {
            // Remove item from the list if count reaches zero
            if (item != null && StoredInventoryItems.Contains(item))
            {
                StoredInventoryItems.Remove(item);
            }
        }
    }
}

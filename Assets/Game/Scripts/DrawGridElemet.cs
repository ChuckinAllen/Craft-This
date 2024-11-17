#if UNITY_EDITOR
using PoschPlus.CraftingSystem;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


//This is basicly an editor script.
//This is used to display the crafting grid element in the editor UI.
public class DrawGridElemet : MonoBehaviour
{

    public int gridX = 3;
    public int gridY = 3;

    [ShowInInspector]
    [HorizontalGroup(Width = 400)]
    [TableMatrix(DrawElementMethod = "DrawGridElementsX",
        HideRowIndices = true, HideColumnIndices = true, SquareCells = true)]
    public Ingredient[,] Grid = new Ingredient[0, 0];

    [SerializeField] private Inventory inventory;

    [ReadOnly]
    [ShowInInspector]
    private List<Inventory.Items> storedInventoryItems = new List<Inventory.Items>();

    //[ReadOnly]
    //[ShowInInspector]
    //private List<string> itemNamesList = new() { "None" };

    //The amount of items that have been used in the crafting grid.
    //int usedSelectedItemsAmount = 0;
    //Dictionary<Inventory.Items, int> storedInvantoryItems = new Dictionary<Inventory.Items, int>();

    //This gets updated when an Item is selected
    //[ReadOnly]
    //[ShowInInspector]
    //private List<Inventory.Items> storedInventoryItemsInCraftingTable = new List<Inventory.Items>();

    bool noIngredentSet = true;

    private Ingredient DrawGridElementsX(Rect rect, Ingredient ingredient, int x, int y)
    {
        // Cache the inventory items
        storedInventoryItems = inventory.StoredInventoryItems;

        //storedInventoryItems[0].SetItemCount(0);

        // Keep track of the previously selected ingredient
        // to restore its count when deselected
        Ingredient previousIngredient = ingredient;

        // Create an array to hold item names for the dropdown, starting with "None"
        string[] itemNames = new string[storedInventoryItems.Count + 1];
        itemNames[0] = "None";

        // Fill the item names array with names from the inventory
        for (int i = 0; i < storedInventoryItems.Count; i++)
        {
            if (storedInventoryItems[i].Count > 0 || storedInventoryItems[i].Data == ingredient)
            {
                itemNames[i + 1] = storedInventoryItems[i].Name;
            }
        }

        int validIndex = 1; // Start at 1 because 0 is reserved for "None"

        // Fill the item names array with valid items (Count > 0 or selected in this slot)
        for (int i = 0; i < storedInventoryItems.Count; i++)
        {
            if (storedInventoryItems[i].Count > 0 || storedInventoryItems[i].Data == ingredient)
            {
                itemNames[validIndex] = storedInventoryItems[i].Name;
                validIndex++;
            }
        }

        int selectedIndex = 0; // Default to "None"

        // If an ingredient is selected, find its index in the inventory
        if (ingredient != null) // -- bug here
        {
            // short form of the for loop code 
            // selectedIndex = storedInventoryItems.FindIndex(item => item.Data == ingredient) + 1;
            for (int i = 0; i < storedInventoryItems.Count; i++)
            {
                if (storedInventoryItems[i].Data == ingredient)
                {
                    selectedIndex = i + 1; // Account for the "None" option at index 0
                    Debug.Log(selectedIndex);
                    break; // Exit loop after finding the matching item
                }
            }

            Rect fullBackgroundRect = new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight,
                rect.width, rect.height - EditorGUIUtility.singleLineHeight);

            Texture2D previewTexture = AssetPreview.GetAssetPreview(ingredient.Model);
            //Debug.Log(previewTexture);
            GUI.DrawTexture(fullBackgroundRect, previewTexture, ScaleMode.StretchToFill);

            if (noIngredentSet == true)
            {

            }
        }

        // Create a dropdown for selecting the item from the inventory
        Rect dropdownRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
        selectedIndex = EditorGUI.Popup(dropdownRect, selectedIndex, itemNames);

        // If an item is selected and the count is greater than 0,
        // decrease the count and update the ingredient
        if (selectedIndex > 0)
        {
            // Calculate the original index by excluding the "None" option
            int itemIndex = -1;
            for (int i = 0; i < storedInventoryItems.Count; i++)
            {
                if (storedInventoryItems[i].Name == itemNames[selectedIndex])
                {
                    itemIndex = i;
                    break;
                }
            }

            if (itemIndex >= 0 && storedInventoryItems[itemIndex].Count > 0)
            {
                // Check if we need to restore the previous ingredient to inventory
                if (previousIngredient != null)
                {
                    for (int i = 0; i < storedInventoryItems.Count; i++)
                    {
                        if (storedInventoryItems[i].Data == previousIngredient)
                        {
                            // Increment the count of the previous item (restore to inventory)
                            storedInventoryItems[i].IncreaseItemCount(1);
                            break;
                        }
                    }
                }

                // Set the selected ingredient from the inventory
                ingredient = storedInventoryItems[itemIndex].Data;



                // Decrease the item count by 1 (selecting the item decreases the count)
                storedInventoryItems[itemIndex].DecreaseItemCount(1);

                // Optional debug log to verify the count decrease
                //Debug.Log($"{storedInventoryItems[originalIndex].Name}
                //count decreased, remaining: {storedInventoryItems[originalIndex].Count}");

                // If the count reaches 0, optionally handle this (e.g., show out of stock)
                if (storedInventoryItems[itemIndex].Count <= 0)
                {

                    noIngredentSet = false;
                    //Debug.Log($"{storedInventoryItems[originalIndex].Name} is out of stock.");
                }

                // If the count reaches 0, optionally handle this (e.g., show out of stock)
                if (storedInventoryItems[itemIndex].Count <= 0)
                {
                    if (storedInventoryItems[itemIndex].Name != ingredient.name)
                    {
                        ingredient = null;
                    }
                    //Debug.Log($"{storedInventoryItems[originalIndex].Name} is out of stock.");
                }
            }
        }
        else
        {
            // If an item was selected before but now set to null, restore the previous item's count
            if (previousIngredient != null)
            {
                for (int i = 0; i < storedInventoryItems.Count; i++)
                {
                    if (storedInventoryItems[i].Data == previousIngredient)
                    {
                        // Restore the previous item's count in the inventory
                        storedInventoryItems[i].IncreaseItemCount(1);
                        Debug.Log($"{storedInventoryItems[i].Name} count restored, " +
                            $"new count: {storedInventoryItems[i].Count}");
                        break;
                    }
                }
            }

            // No item is selected, set ingredient to null

            //this.Grid = new Ingredient[gridX, gridY];

            ingredient = null;
            GUI.Label(rect, "Empty");
        }

        // Ensure the selected item always shows its name in all slots where it's selected
        // UpdateButton(ref rect, ref ingredient, x, y);
        // HandleDragAndDrop(rect, x, y, ref ingredient);

        return ingredient;
    }
}
#endif
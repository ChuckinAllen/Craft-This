using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PoschPlus.CraftingSystem
{
    public class Crafting : MonoBehaviour
    {
        /// <summary>
        /// replace this with recipe book later, adding more function
        /// I should be able to select any recipy within a book and craft that item.
        /// </summary>
        /// 
        [InlineEditor(InlineEditorObjectFieldModes.Foldout)]
        [SerializeField] private Recipe recipe;
        [Tooltip("This is the player's inventory, " +
            "if the player interacts with 'WORKBENCH' it should be selected at runtime.")]
        [SerializeField] private Inventory inventory;
        [SerializeField] private CreateItem createItem;
        [SerializeField] private Transform spawnPos;

        private enum CraftingState
        {
            NoGridItemCount,  // Crafting without a grid (based on item count)
            Grid,             // Crafting with a grid (item position matters)
            InWorld,          // Crafting within the world (using world-based crafting logic)
        }

        [SerializeField]
        private CraftingState state = CraftingState.NoGridItemCount;

        [ShowIf("state", CraftingState.Grid)]
        [SerializeField] private CraftingGrid grid;

        private void Start()
        {
            grid = FindAnyObjectByType<CraftingGrid>().GetComponent<CraftingGrid>();
        }

        [Button("Craft Item")]
        public void CraftItem()
        {
            switch (state)
            {
                case CraftingState.NoGridItemCount:
                    CraftWithNoGrid();
                    break;
                case CraftingState.Grid:
                    CraftWithGrid();
                    break;
                case CraftingState.InWorld:
                    CraftInWorld();
                    break;
            }
        }

        private void CraftWithNoGrid()
        {
            CheckRecipeItems();
        }

        private void CheckRecipeItems()
        {
            // Loop through the required ingredients in the recipe
            foreach (Ingredient ingredient in recipe.ingredients.Keys)
            {
                int requiredCount = recipe.ingredients[ingredient];

                // Find the matching item in the player's inventory
                foreach (Inventory.Items item in inventory.StoredInventoryItems)
                {
                    if (item.Data == ingredient && item.Count >= requiredCount)
                    {
                        // Craft the item and remove the ingredients
                        RemoveItemsFromInventory(requiredCount, item);
                        break; // Exit once the item is crafted
                    }
                }
            }
        }

        public void CraftWithGrid()
        {
            // Ensure both the crafting grid and recipe grid are not null
            if (grid.Grid != null && recipe.GridIngredients != null)
            {
                // Ensure the grid dimensions match (both rows and columns)
                if (grid.Grid.GetLength(0) == recipe.GridIngredients.GetLength(0) &&
                    grid.Grid.GetLength(1) == recipe.GridIngredients.GetLength(1))
                {
                    bool isMatchingRecipe = true;

                    // Compare each slot in the crafting grid with the recipe grid
                    for (int row = 0; row < grid.Grid.GetLength(0); row++)
                    {
                        for (int column = 0; column < grid.Grid.GetLength(1); column++)
                        {
                            Ingredient craftingIngredient = grid.Grid[row, column];
                            Ingredient recipeIngredient = recipe.GridIngredients[row, column];

                            // Check if the items are different based on their names or if one is null
                            if ((craftingIngredient == null && recipeIngredient != null) ||
                                (craftingIngredient != null && recipeIngredient == null) ||
                                (craftingIngredient != null && recipeIngredient != null &&
                                craftingIngredient.name != recipeIngredient.name))
                            {
                                isMatchingRecipe = false;
                                break; // Exit early if any mismatch is found
                            }
                        }
                        if (!isMatchingRecipe) break; // Break outer loop if mismatch found
                    }

                    // Check if the crafting grid matches the recipe
                    if (isMatchingRecipe)
                    {
                        Debug.Log("Crafted Item!");

                        // Craft the item and remove the ingredients from the grid and inventory
                        RemoveIngredientsFromGrid();
                        DeleteItemsFromUI();
                        AddCraftedItemToInventory();
                    }
                    else
                    {
                        Debug.Log("Crafting grid does not match the recipe.");
                    }
                }
                else
                {
                    Debug.LogError("The recipe grid and crafting grid do not have the same dimensions.");
                }
            }
            else
            {
                Debug.LogError("One of the grids is null.");
            }
        }

        private void DeleteItemsFromUI()
        {
            grid.DestroyItemToGridUI();
        }

        private void RemoveIngredientsFromGrid()
        {
            for (int row = 0; row < grid.Grid.GetLength(0); row++)
            {
                for (int column = 0; column < grid.Grid.GetLength(1); column++)
                {
                    Ingredient ingredient = grid.Grid[row, column];

                    if (ingredient != null)
                    {
                        // Find the corresponding item in the inventory and remove it
                        Inventory.Items item = inventory.StoredInventoryItems
                                               .FirstOrDefault(i => i.Data == ingredient);
                        if (item != null)
                        {
                            RemoveItemsFromInventory(1, item);
                        }
                    }
                }
            }
        }

        private void RemoveItemsFromInventory(int count, Inventory.Items item)
        {
            Debug.Log($"Crafted an item, decreasing ingredient count by {count}.");

            item.DecreaseItemCount(count);

            List<Inventory.Items> invantoryItems = inventory.StoredInventoryItems;

            if(item.Count == 0)
            {
                invantoryItems.Remove(item);
            }

            // Remove the item from the inventory if the count reaches zero
            if (item.Count == 0)
            {
                //inventory.RemoveItem(item);
                //Debug.Log("Removed item from inventory.");
            }

            //ToDo This is a quick fix to reset crafting grid after item is crafted
            //grid.UpdateCraftingTableSize();
        }

        private void AddCraftedItemToInventory()
        {
            createItem.CreateNewItem(recipe.CraftedItems, false);
            Debug.LogWarning("Added crafted item to inventory.");
        }

        private void CraftInWorld()
        {
            Debug.Log("Crafting in the world is not yet implemented.");
        }
    }
}

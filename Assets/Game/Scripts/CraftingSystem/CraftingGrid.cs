using Sirenix.OdinInspector;
using PoschPlus.CraftingSystem;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

//Player should be able to add to this at runtime.
public class CraftingGrid : MonoBehaviour
{
    public int gridX = 3;
    public int gridY = 3;

    [ShowInInspector]
    [HorizontalGroup(Width = 400)]
    [TableMatrix(DrawElementMethod = "DrawGridElements",
        HideRowIndices = true, HideColumnIndices = true, SquareCells = true)]
    public Ingredient[,] Grid = new Ingredient[0, 0];

    [SerializeField] private Inventory inventory;

    [ReadOnly]
    [ShowInInspector]
    private List<Inventory.Items> storedInventoryItems = new List<Inventory.Items>();

    [ReadOnly]
    [ShowInInspector]
    private List<GameObject> itemsStoredToGridUI = new List<GameObject>();

    private void Start()
    {
        UpdateCraftingTableSize();
    }

    [Button]
    public void UpdateCraftingTableSize()
    {
        Grid = new Ingredient[gridX, gridY];
    }

    public void AddGameObjectsToGridUI(string itemName, Vector2 gridPosition)
    {
        for (int i = 0; i < inventory.StoredInventoryItems.Count; i++)
        {
            if (inventory.StoredInventoryItems[i].Name == itemName)
            {
                Grid[(int)gridPosition.x -1, (int)gridPosition.y -1] = inventory.StoredInventoryItems[i].Data;
            }
        }
    }

    public void AddItemToGridUI(GameObject item)
    {
        itemsStoredToGridUI.Add(item);
    }

    public void RemoveItemToGridUI(GameObject item)
    {
        itemsStoredToGridUI.Remove(item);
    }

    public void DestroyItemToGridUI()
    {
        // Create a copy of the list to safely iterate and destroy items
        List<GameObject> itemsToDestroy = new List<GameObject>(itemsStoredToGridUI);

        foreach (GameObject item in itemsToDestroy)
        {
            Destroy(item);
            itemsStoredToGridUI.Remove(item);
        }
    }

    public void RemoveItemFromGridUI(string itemName, Vector2 gridPosition, Transform itemPosition)
    {
        for (int i = 0; i < inventory.StoredInventoryItems.Count; i++)
        {
            if (inventory.StoredInventoryItems[i].Name == itemName)
            {
                Grid[(int)gridPosition.x - 1, (int)gridPosition.y - 1] = null;
            }
        }
    }



    private Ingredient DrawGridElements(Rect rect, Ingredient ingredient)
    {
        storedInventoryItems = inventory.StoredInventoryItems;

        Ingredient previousIngredient = ingredient;

        // Create an array to hold item names for the dropdown, starting with "None"
        string[] itemNames = new string[storedInventoryItems.Count + 1];
        itemNames[0] = "None";

        int selectedIndex = 0; // Default to "None"

        // Fill the item names array with names from the inventory
        for (int i = 0; i < storedInventoryItems.Count; i++)
        {
            if (storedInventoryItems[i].Count > 0 || storedInventoryItems[i].Data == ingredient)
            {
                itemNames[i + 1] = storedInventoryItems[i].Name;

                //Debug.Log($"Ingredent {storedInventoryItems[i].Name}. number: {i} was added to the list of names!");
            }

            if (storedInventoryItems[i].Data == ingredient && ingredient != null)
            {
                selectedIndex = i + 1;

                //Debug.Log($"Selection Index: {selectedIndex} was updated!");
            }
        }

#if UNITY_EDITOR
        //ToDo refactor this into the DrawGridElemet Script.
        DrawGUI(ref rect, ingredient, itemNames, ref selectedIndex);

#endif

        for (int i = 0; i < storedInventoryItems.Count; i++)
        {
            // If an item is selected and the count is greater than 0,
            // decrease the count and update the ingredient

            if (storedInventoryItems[i].Name == itemNames[selectedIndex])
            {
                //int itemCount = storedInventoryItems[i].Count;

                if (selectedIndex > 0)
                {
                    // Set the selected ingredient from the inventory
                    ingredient = storedInventoryItems[i].Data;

                    // Decrease the item count by 1 (selecting the item decreases the count)

                    //itemCount--;

                    //ToDo change this this should not directly modify the invantory
                    storedInventoryItems[i].DecreaseItemCount(1); 
                    
                }

                if (storedInventoryItems[i].Data == previousIngredient)
                {
                    //Debug.Log($"Previous Ingredent {previousIngredient}");

                    // Increment the count of the previous item (restore to inventory)


                    //ToDo change this this should not directly modify the invantory
                    storedInventoryItems[i].IncreaseItemCount(1);
                }
            }
        }

        if (selectedIndex == 0)
        {
            // Restore the previous item count
            if (previousIngredient != null)
            {
                for (int i = 0; i < storedInventoryItems.Count; i++)
                {
                    if (storedInventoryItems[i].Data == previousIngredient)
                    {
                        // Restore the previous item's count in the inventory


                        //ToDo change this this should not directly modify the invantory
                        storedInventoryItems[i].IncreaseItemCount(1);
                        break;
                    }
                }
            }

            ingredient = null;
            GUI.Label(rect, "Empty");
        }

        // HandleDragAndDrop(rect, x, y, ref ingredient);

        return ingredient;
    }

#if UNITY_EDITOR
    private static void DrawGUI(ref Rect rect, Ingredient ingredient, string[] itemNames, ref int selectedIndex)
    {
        if (selectedIndex > 0)
        {
            Rect fullBackgroundRect = new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight,
            rect.width, rect.height - EditorGUIUtility.singleLineHeight);

            if(ingredient != null)
            {
                Texture2D previewTexture = AssetPreview.GetAssetPreview(ingredient.Model);
                if (previewTexture != null)
                {
                    GUI.DrawTexture(fullBackgroundRect, previewTexture, ScaleMode.StretchToFill);
                }
            }


        }

        // Create a dropdown for selecting the item from the inventory
        Rect dropdownRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
        selectedIndex = EditorGUI.Popup(dropdownRect, selectedIndex, itemNames);
    }


    //ToDo Fix Drag and Drop Editor UI
    #region Drag Drop WIP
    /*
    private void HandleDragAndDrop(Rect rect, int x, int y, ref Ingredient data)
    {
        int originalX = -1;
        int originalY = -1;

        // Start dragging if the mouse is pressed and the item exists
        if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition) &&
            data != null)
        {
            DragAndDrop.PrepareStartDrag();
            DragAndDrop.objectReferences = new UnityEngine.Object[] { data };
            originalX = x;       // Store the original position
            originalY = y;
            DragAndDrop.StartDrag($"Dragging {data.name}");
            Event.current.Use();  // Prevent further processing of this event
        }

        // Provide visual feedback during drag
        if (Event.current.type == EventType.DragUpdated && rect.Contains(Event.current.mousePosition))
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            Event.current.Use();
        }

        // Handle the drop
        if (Event.current.type == EventType.DragPerform && rect.Contains(Event.current.mousePosition))
        {
            DragAndDrop.AcceptDrag();
            if (DragAndDrop.objectReferences.Length > 0)
            {
                Ingredient droppedData = DragAndDrop.objectReferences[0] as Ingredient;
                if (droppedData != null)
                {
                    Grid[x, y] = droppedData;
                    data = droppedData;

                    if (originalX != x || originalY != y)
                    {
                        //PrefabMatrix[originalX, originalY] = null; 

                        //data = (ItemData)EditorGUIUtility.GetObjectPickerObject();
                        //PrefabMatrix[x, y] = null;

                        //data = null;
                    }

                    GUI.changed = true; // Notify Unity that the GUI has changed

                }
            }
            Event.current.Use();
        }
    }*/

    #endregion
#endif
}

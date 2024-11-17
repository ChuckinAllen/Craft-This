using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

//ToDO Crafting grid part of the recipy needs to be refactored out to just use the CraftingGrid Script 
//instead to remove some of the same duplicated code
namespace PoschPlus.CraftingSystem
{
    [HideMonoScript]
    [System.Serializable]
    //Recipy what will be used to build this item or how it will be built.
    [CreateAssetMenu(fileName = "New Recipe", menuName = "Create Recipe")]
    public class Recipe : SerializedScriptableObject
    {
        [Tooltip("The resulting item after crafting")]
        [field: InlineEditor]
        [field: SerializeField] public Ingredient CraftedItems { get; private set; }

        [Tooltip("This is the amout of items that will be crafted")]
        [field: PropertyRange(1, 128)]
        [field: SerializeField] public int CraftedItemCount { get; private set; } = 1;

        [field: SerializeField]
        //public Ingredient[] Ingredients_1 { get; private set; }

        [field: ShowInInspector]
        [Tooltip("This is any item that is in this recipy")]
        public Dictionary<Ingredient, int> ingredients = new Dictionary<Ingredient, int>();

        [HorizontalGroup(Width = 400)]
#if UNITY_EDITOR
        [TableMatrix(DrawElementMethod = "DrawElement",
        HideRowIndices = true, HideColumnIndices = true, SquareCells = true)]
#endif
        public Ingredient[,] GridIngredients;

        [PropertyRange(min: 1, max: 10)]
        public int gridX = 3;
        [PropertyRange(min: 1, max: 10)]
        public int gridY = 3;

#if UNITY_EDITOR
        private void DisplayCraftedItems(Rect rect, Ingredient data)
        {
            GUI.DrawTexture(rect, AssetPreview.GetAssetPreview(data.Model), ScaleMode.ScaleToFit);
        }

        [Button]
        private void UpdateCraftingTableSize()
        {
            GridIngredients = new Ingredient[gridX, gridY];
        }

        private Ingredient DrawElement(Rect rect, Ingredient data, int x, int y)
        {
            if (data != null)
            {
                if(data.Model != null)
                {
                    GUI.DrawTexture(rect, AssetPreview.GetAssetPreview(data.Model), ScaleMode.ScaleToFit);
                }
                
                GUIStyle textStyle = new GUIStyle(GUI.skin.box)
                {
                    alignment = TextAnchor.UpperLeft,
                    fontStyle = FontStyle.Bold,
                    normal = { textColor = Color.green }
                };

                string itemName = $"Item: {data.name}";
                GUI.Box(rect, itemName, textStyle);

                GUI.color = Color.white;
            }
            else
            {
                GUI.Label(rect, "Empty");
            }

            UpdateButton(ref rect, ref data, x, y);

            HandleDragAndDrop(rect, x, y, ref data);
            

            return data;
        }

        private void UpdateButton(ref Rect rect, ref Ingredient data, int x, int y)
        {
            int controlID = GUIUtility.GetControlID(FocusType.Passive);

            //GameObject gameObject = null;
            bool AllowSceneObjects = false;
            string searchField = string.Empty;
            //string buttonName = "Select an item";

            Rect buttonRect = new Rect(rect.x, rect.yMax - EditorGUIUtility.singleLineHeight, rect.width,
                EditorGUIUtility.singleLineHeight);
            if (GUI.Button(buttonRect, "Select Item"))
            {
                EditorGUIUtility.ShowObjectPicker<Ingredient>(data, AllowSceneObjects, searchField, controlID);
                Event.current.Use();
            }

            if (Event.current.commandName == "ObjectSelectorUpdated" && 
                EditorGUIUtility.GetObjectPickerControlID() == controlID)
            {
                data = (Ingredient)EditorGUIUtility.GetObjectPickerObject();
                GridIngredients[x, y] = data;

                GUI.changed = true;
            }
        }

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
                        GridIngredients[x, y] = droppedData;
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
        }
#endif
    }
}
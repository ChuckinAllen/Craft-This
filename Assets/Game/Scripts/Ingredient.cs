using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PoschPlus.CraftingSystem
{
    //Warrning Keep this item simple when using a SerizedScriptableObject
    //The more complicated it gets the slower the data will load.
    [CreateAssetMenu(fileName = "New Item", menuName = "Create Item")]
    public class Ingredient : SerializedScriptableObject  
    {
        //item instance
#if UNITY_EDITOR
        //[field: CustomValueDrawer("DisplayCraftedItems")]
#endif
        [field: SerializeField] public Mesh ItemMesh {  get; private set; }
        //[field: SerializeField] public Image ItemImage { get; private set; }

        [field: SerializeField] public Material Material { get; private set; }

        [AssetsOnly]
        [field: SerializeField] public GameObject Model;

        [SerializeField] private ItemType typeData;
#if UNITY_EDITOR

        [FolderPath]
        [SerializeField] private string allowedFolderPath = "Assets/Models"; // Replace with

        [DetailedInfoBox("Tries to Auto Find Asset in 'Assets/Models' folder. ",
            "If scritableObject name is the same as the model it will find it if not you can manualy select it.")]
        [Button("Find Asset")]
        private void FindAsset()
        {
            // Find and assign the model on enable
            FindModel();
        }

        private void FindModel()
        {
            // Get the name of this scriptable object
            string modelName = name; // The name of the ItemData asset

            // Search for the model asset in the allowed folder
            string[] guids = AssetDatabase.FindAssets(modelName + " t:GameObject", new[] { allowedFolderPath });
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                Model = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            }
            else
            {
                Debug.LogWarning($"No model found for {modelName} in {allowedFolderPath}");
            }
        }

        private void DisplayCraftedItems()
        {
            Rect rect = new Rect(0, 0, 0, 0);
            GUI.DrawTexture(rect, AssetPreview.GetAssetPreview(Model), ScaleMode.ScaleToFit);
        }
#endif
    }
}
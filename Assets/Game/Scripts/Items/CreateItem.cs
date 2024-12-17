
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

//ToDo every time this is called add the item to the playerInvantory
// or add it to the world.

//This should be called at the start of the game.
//It will be called to create each starting item
// with the starting item script.
namespace PoschPlus.CraftingSystem
{
    public class CreateItem : MonoBehaviour
    {
        [SerializeField] private Material blackMaterial;
        [SerializeField] private Shader shader;
        [SerializeField] private float defaultItemSize = 0.8f;
        [SerializeField] private Vector3 defaultLocationOfItemToCreate = new Vector3(0f,0f, 0.9f);
        [SerializeField] private Inventory inventory;
        //[SerializeField] private RenderTexture newRenderTexture;
        [SerializeField] private Transform spawnPositionUI;
        [SerializeField] private Transform itemToCraftSpawnPosition;
        [SerializeField] private Canvas itemCanvas;
        [SerializeField] private LayerMask ignoreRaycastLayer;
        //[SerializeField] private Transform ItemSpawnPosition;

        [SerializeField] private GameObject itemPrefabUI;

        [SerializeField] private bool disableItemNames = true;


        //[SerializeField] private LayerMask dragLayer; //dropLayer

        public static System.Action<Ingredient,bool> OnCreateNewItem;
        public static System.Action<Ingredient> OnCreateNewItemToCraft;

        private void OnEnable()
        {
            OnCreateNewItem += CreateNewItem;
            OnCreateNewItemToCraft += CreateNewItemToCraft;
        }

        private void OnDisable()
        {
            OnCreateNewItem -= CreateNewItem;
            OnCreateNewItemToCraft -= CreateNewItemToCraft;
        }

        public void CreateNewItem(Ingredient ingredient, bool randomPos)
        {
            GameObject instance = Instantiate(itemPrefabUI, spawnPositionUI);
            instance.name = ingredient.name;
            Debug.Log($"Item name: {instance.name}");

            inventory.AddItemToInventory(ingredient);

            Item itemData = instance.AddComponent<Item>();
            itemData.UpdateItemData(ingredient);

            ItemUI itemUI = instance.GetComponent<ItemUI>();
            DragAndDropUI dragAndDrop = instance.GetComponent<DragAndDropUI>();
            dragAndDrop.canvas = itemCanvas;
            ClampToCanvas clampToCanvas = instance.GetComponent<ClampToCanvas>();
            clampToCanvas.canvasRectTransform = itemCanvas.GetComponent<RectTransform>();

            if(disableItemNames == true)
            {
                itemUI.itemName.text = "";
            }
            else
            {
                itemUI.itemName.text = ingredient.name;
            }
            
            instance.transform.localPosition = Vector3.zero;

            if (randomPos)
            {
                SetItemUIPosition(Vector2.zero, instance, randomPos);
            }

            if (ingredient.Model == null)
            {
                Debug.Log($"Model:{ingredient.Model}");
                return;
            }
            GameObject item = Instantiate(ingredient.Model);
            item.transform.parent = itemUI.itemOrImageTransform;
            Debug.Log($"Parent {item.transform.parent}");

            item.transform.localPosition = Vector3.zero;
            Debug.Log($"Local Pos {item.transform.localPosition}");
            Debug.Log($"Global Pos {item.transform.position}");

            item.transform.localScale = new Vector3(defaultItemSize, defaultItemSize, defaultItemSize);
            Debug.Log($"local Scale {item.transform.localScale}");
            Debug.Log($"Global Scale {item.transform.lossyScale}");

            MeshRenderer meshRenderer = item.GetComponent<MeshRenderer>();
            Debug.Log(meshRenderer);

            Material material = ingredient.Material;
            Debug.Log(material);

            SetMaterial(meshRenderer, material);

            if (randomPos == false)
            {
                //item.transform.localPosition = new Vector3(0, -0.5f, 0);
                //item.transform.rotation = new Quaternion(0, 90, 0, 0);
            }
        }

        private void SetItemUIPosition(Vector2 pos, GameObject instance, bool randomPos)
        {
            if (randomPos)
            {
                // Get the canvas RectTransform to define boundaries
                RectTransform canvasRectTransform = itemCanvas.GetComponent<RectTransform>();

                // Define canvas boundaries (local space)
                Vector2 min = canvasRectTransform.rect.min;
                Vector2 max = canvasRectTransform.rect.max;

                // Generate random position within canvas bounds
                float randomX = Random.Range(min.x, max.x);
                float randomY = Random.Range(min.y, max.y);

                // Set the item's position
                instance.transform.localPosition = new Vector3(randomX, randomY, 0f);
            }
            else
            {
                // Set the item's position directly to the provided position
                instance.transform.position = pos;
            }
        }

        public void CreateNewItemToCraft(Ingredient ingredient)
        {
            Debug.LogWarning(ingredient);

            //inventory.AddItemToInventory(ingredient);
            inventory.SetCraftableItemToInvantoryToStore(ingredient);

            GameObject instance = Instantiate(itemPrefabUI, itemToCraftSpawnPosition);
            instance.name = ingredient.name;
            instance.GetComponent<DragAndDropUI>().enabled = false;
            instance.layer = ignoreRaycastLayer.value >> 1;

            var itemData = instance.AddComponent<Item>();
            itemData.UpdateItemData(ingredient);

            if (ingredient.Model != null)
            {
                GameObject item = Instantiate(ingredient.Model);
                ItemUI itemUI = instance.GetComponent<ItemUI>();

                itemUI.itemName.enabled = false;

                item.transform.parent = itemUI.itemOrImageTransform;
                //item.transform.rotation = new Quaternion(0, 90, 0, 0);
                item.transform.localScale = new Vector3(defaultItemSize, defaultItemSize, defaultItemSize);

                item.transform.localPosition = defaultLocationOfItemToCreate;

                MeshRenderer meshRenderer = item.GetComponent<MeshRenderer>();
                SetDarkMaterial(meshRenderer);
            }
        }

        private void SetDarkMaterial(MeshRenderer meshRenderer)
        {
            // Create a new material with a black color

            //Shader localShader = Shader.Find("Universal Render Pipeline/Lit");
            //Material blackMaterial = new Material(localShader);
            //blackMaterial.color = Color.black;

            // Set the material of the MeshRenderer to the black material
            meshRenderer.material = blackMaterial;
        }

        private void SetMaterial(MeshRenderer meshRenderer, Material material)
        {
            meshRenderer.material = material;
        }

        private GameObject SpawnNewItemUI(Ingredient ingredient, Vector2 pos,
            RenderTexture renderTexture, bool randomPos)
        {
            GameObject instance = CreateUI(ingredient);

            SetItemUIPositionOld(pos, instance, randomPos);

            UpdateUI(ingredient, renderTexture, instance);

            return instance;
        }


        private GameObject CreateUI(Ingredient ingredient)
        {
            GameObject instance = Instantiate(itemPrefabUI);
            instance.transform.SetParent(spawnPositionUI);
            instance.GetComponent<DragAndDropUI>().parentTransform = spawnPositionUI;
            instance.name = ingredient.name;
            return instance;
        }

        private void UpdateUI(Ingredient ingredient, RenderTexture renderTexture, GameObject instance)
        {
            ItemUI imageUI = instance.GetComponent<ItemUI>();
            var imageTransform = imageUI.itemOrImageTransform;
            imageUI.itemName.text = ingredient.name;
            imageTransform.GetComponent<RawImage>().texture = renderTexture;
        }

        private void SetItemUIPositionOld(Vector2 pos, GameObject instance, bool randomPos)
        {
            if (randomPos)
            {
                Vector3 localPosition = itemCanvas.transform.localPosition;

                instance.transform.localPosition = GetRandomPosition(localPosition);
            }
            else
            {
                instance.transform.position = pos;
            }
        }

        private Vector2 GetRandomPosition(Vector2 localPosition)
        {
            float posX = localPosition.x;
            float posY = localPosition.y;


            float randomX = Random.Range(0, posX);
            float randomY = Random.Range(0, posY);

            var pos = new Vector2(randomX, randomY);

            return pos;
        }
    }
}
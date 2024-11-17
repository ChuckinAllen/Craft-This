using UnityEngine;
using UnityEngine.UI;

//ToDo every time this is called add the item to the playerInvantory
// or add it to the world.

//This should be called at the start of the game.
//It will be called to create each starting item
// with the starting item script.
namespace PoschPlus.CraftingSystem
{
    public class CreateItem : MonoBehaviour
    {
        [SerializeField] private Vector3 defaultScaleOfItemToCreate = new Vector2(100,100);
        [SerializeField] private Inventory inventory;
        //[SerializeField] private RenderTexture newRenderTexture;
        [SerializeField] private Transform spawnPositionUI;
        [SerializeField] private Transform spawnPositionItems;
        [SerializeField] private Canvas itemCanvas;
        //[SerializeField] private Transform ItemSpawnPosition;

        [SerializeField] private GameObject itemPrefabUI;


        //[SerializeField] private LayerMask dragLayer; //dropLayer


        //WIP 
        public void CreateNewItem(Ingredient ingredient, Transform vectorPos)
        {
            //New GameObject
            GameObject instance = new GameObject(ingredient.name);
            ItemUI itemUI = instance.AddComponent<ItemUI>();

            //ToDo add text that is set to active when hovering over item 
            //itemUI.itemName.text = instance.name;

            //RectTransform

            GameObject item = Instantiate(ingredient.Model, vectorPos);

            item.transform.localScale = new Vector3 (10,10,10);

            item.transform.parent = vectorPos;
        }

        public void CreateNewItem(Ingredient ingredient, Transform itemTransform, bool randomPos, bool itemToCraft)
        {
            RenderTexture newRenderTexture = CreateNewRenderTexture(ingredient);

            Vector2 itemPos = itemTransform.position;

            Camera cam = CreateNewItemCamera(ingredient , itemPos.x, newRenderTexture, itemToCraft);

            GameObject item = SpawnNewItem(ingredient, itemPos, cam, itemToCraft);

            //ToDo Add Remove all old UI items

            //itemUI.GetComponent<DragAndDropUI>().enabled = dragAndDrop;
            //itemUI.GetComponent<Image>().enabled = dragAndDrop;
            //itemUI.GetComponent<ItemUI>().itemName.enabled = dragAndDrop;

            if(itemToCraft)
            {
                //itemUI.GetComponent<RectTransform>().sizeDelta = defaultScaleOfItemToCreate;
                //itemTransform.layer = LayerMask.NameToLayer("ItemToCraft");

                SetItemToCraft(newRenderTexture, itemTransform);
            }
            else
            {
                GameObject itemUI = SpawnNewItemUI(ingredient, itemPos, newRenderTexture, randomPos);
            }

            //ToDo Add simple function to remove all old invantory items

            inventory.AddItemToInventory(ingredient);
        }

        private void SetItemToCraft(RenderTexture renderTexture, Transform itemTransform)
        {
            itemTransform.GetComponent<RawImage>().texture = renderTexture;
        }

        public void CreateNewItem(Ingredient ingredient, Vector2 itemPos, bool randomPos)
        {
            RenderTexture newRenderTexture = CreateNewRenderTexture(ingredient);

            Camera cam = CreateNewItemCamera(ingredient, itemPos.x, newRenderTexture, false);

            GameObject item = SpawnNewItem(ingredient, itemPos, cam, false);

            //ToDo Add Remove all old UI items

            GameObject itemUI = SpawnNewItemUI(ingredient, itemPos, newRenderTexture, randomPos);

            //ToDo Add simple function to remove all old invantory items

            inventory.AddItemToInventory(ingredient);
        }



        private RenderTexture CreateNewRenderTexture(Ingredient ingredient)
        {
            int width = 256;
            int height = 256;
            int depth = 24;
            RenderTextureFormat format = RenderTextureFormat.Default;

            // Create a new RenderTexture with specified width, height, depth, and format
            //RenderTexture renderTexture = new RenderTexture(width, height, depth, format);

            RenderTexture renderTexture = new RenderTexture(width, height, depth, format)
            {
                // Set any additional properties here if needed, such as format, etc.
                antiAliasing = 1, // No anti-aliasing
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };
            renderTexture.name = ingredient.name;

            // Activate the RenderTexture so it's ready to use
            renderTexture.Create();

            // Optionally set this as the active RenderTexture if you want to use it immediately
            RenderTexture.active = renderTexture;

            return renderTexture;
        }

        private Camera CreateNewItemCamera(Ingredient ingredient, float pos, 
            RenderTexture renderTexture, bool noLighting)
        {
            GameObject cameraObject = new GameObject($"Cam: {ingredient.name}");

            Camera newCamera = cameraObject.AddComponent<Camera>();
            newCamera.transform.SetParent(spawnPositionItems);

            //Set Position to the items position.
            newCamera.transform.position = new Vector3(pos, 0, -2);
            newCamera.targetTexture = renderTexture;

            if (noLighting == false)
            {
                
                newCamera.clearFlags = CameraClearFlags.SolidColor;
                newCamera.backgroundColor = Color.clear;
            }
            else
            {
                // Set up the camera to ignore lighting and shadows
                newCamera.clearFlags = CameraClearFlags.SolidColor;
                newCamera.backgroundColor = Color.clear;
                newCamera.allowHDR = false;
                newCamera.allowMSAA = false;
                //newCamera.cullingMask = LayerMask.GetMask("UnlitLayer"); // Render only specific layers if needed

                // Ensure no shadows are rendered
                newCamera.usePhysicalProperties = false;
                newCamera.renderingPath = RenderingPath.VertexLit;
            }


            return newCamera;
        }

        private GameObject SpawnNewItem(Ingredient ingredient, Vector2 pos, Camera cam, bool itemToCraft)
        {
            GameObject item = new GameObject(ingredient.name);
            item.AddComponent<Item>().UpdateItemData(ingredient);
            item.name = ingredient.name;
            item.transform.SetParent(cam.transform);

            item.transform.position = new Vector3(pos.x, 0, 0);

            item.AddComponent<MeshFilter>().mesh = ingredient.ItemMesh;

            MeshRenderer meshRenderer = item.AddComponent<MeshRenderer>();

            if (itemToCraft)
            {
                SetDarkMaterial(meshRenderer);
            }
            else
            {
                meshRenderer.material = ingredient.Material;
            }

            

            return item;
        }

        private void SetDarkMaterial(MeshRenderer meshRenderer)
        {
            // Create a new material with a black color
            Material blackMaterial = new Material(Shader.Find("Standard"));
            blackMaterial.color = Color.black;

            // Set the material of the MeshRenderer to the black material
            meshRenderer.material = blackMaterial;
        }

        private GameObject SpawnNewItemUI(Ingredient ingredient, Vector2 pos,
            RenderTexture renderTexture, bool randomPos)
        {
            GameObject instance = CreateUI(ingredient);

            SetItemUIPosition(pos, instance, randomPos);

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
            var imageTransform = imageUI.itemImageTransform;
            imageUI.itemName.text = ingredient.name;
            imageTransform.GetComponent<RawImage>().texture = renderTexture;
        }

        private void SetItemUIPosition(Vector2 pos, GameObject instance, bool randomPos)
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
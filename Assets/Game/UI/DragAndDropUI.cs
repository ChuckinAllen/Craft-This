using PoschPlus.CraftingSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragAndDropUI : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    Vector3 offset;
    CanvasGroup canvasGroup;
    public LayerMask destinationLayer;

    public LayerMask itemToCraftLayer;

    //ToDo add function to this 
    //Make it so that I can readd items to the crafted space
    public LayerMask craftingPosition; 

    [SerializeField] private CraftingGrid grid;

    [SerializeField]
    public Transform parentTransform; //Gets set @ spawn

    [SerializeField]
    [ReadOnly]
    private Vector2 currentGridPos;

    void Awake()
    {
        if (gameObject.GetComponent<CanvasGroup>() == null)
            gameObject.AddComponent<CanvasGroup>();
        canvasGroup = gameObject.GetComponent<CanvasGroup>();

        grid = FindAnyObjectByType<CraftingGrid>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition + offset;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        offset = transform.position - Input.mousePosition;
        canvasGroup.alpha = 0.5f;
        canvasGroup.blocksRaycasts = false;

        transform.SetParent(parentTransform);

        RaycastResult raycastResult = eventData.pointerCurrentRaycast;

        string gridPositionName = raycastResult.gameObject.transform.parent.name;
        //Vector2 gridPosition = ConvertStringToVector2(gridPositionName);

        string itemName = this.gameObject.name;

        //transform.position = raycastResult.gameObject.transform.position;

        if(currentGridPos != Vector2.zero)
        {
            grid.RemoveItemFromGridUI(itemName, currentGridPos, parentTransform);

            grid.RemoveItemToGridUI(gameObject);
            currentGridPos = Vector2.zero;
        }
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        RaycastResult raycastResult = eventData.pointerCurrentRaycast;

        string gridPositionName = raycastResult.gameObject.transform.parent.name;

        string itemName = this.gameObject.name;

        if ((1 << raycastResult.gameObject?.layer) == destinationLayer.value)
        {
            Vector2 gridPosition = ConvertStringToVector2(gridPositionName);

            transform.position = raycastResult.gameObject.transform.position;
            transform.SetParent(raycastResult.gameObject.transform);

            currentGridPos = gridPosition;

            grid.AddGameObjectsToGridUI(itemName, gridPosition);

            grid.AddItemToGridUI(gameObject);
        }
        if((1 << raycastResult.gameObject?.layer) == itemToCraftLayer.value)
        {
            transform.position = raycastResult.gameObject.transform.position;
            transform.SetParent(raycastResult.gameObject.transform);

            ItemToCraft.itemIsCorrect?.Invoke(itemName);
            //Check for correct Item 
            //If correct Win Next Level
        }

        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
    }

    public Vector2 ConvertStringToVector2(string input)
    {
        // Remove the parentheses and split by the comma
        input = input.Trim('(', ')');
        string[] values = input.Split(',');

        // Parse the values into floats and create a new Vector2
        float x = float.Parse(values[0]);
        float y = float.Parse(values[1]);

        return new Vector2(x, y);
    }

}

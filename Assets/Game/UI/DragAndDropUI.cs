using PoschPlus.CraftingSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragAndDropUI : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;

    private Vector2 originalPosition;
    private Vector3 mouseOffset;

    [SerializeField] private LayerMask destinationLayer;
    [SerializeField] private LayerMask itemToCraftLayer;

    [SerializeField] private CraftingGrid grid;

    [SerializeField]
    public Transform parentTransform; // Gets set at spawn

    [SerializeField, ReadOnly]
    private Vector2 currentGridPos;

    private void Awake()
    {
        canvas = FindAnyObjectByType<Canvas>(); // Get the canvas in the scene
        rectTransform = GetComponent<RectTransform>();

        // Add and initialize the CanvasGroup if it doesn't exist
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
        canvasGroup = gameObject.GetComponent<CanvasGroup>();

        //grid = FindFirstObjectByType<CraftingGrid>();
        grid = FindAnyObjectByType<CraftingGrid>();

        parentTransform = transform.parent;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.SetParent(parentTransform);

        // Store the original position to allow resetting later
        originalPosition = rectTransform.anchoredPosition;

        // Calculate mouse offset relative to the item's position
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            rectTransform,
            eventData.position,
            canvas.worldCamera,
            out Vector3 worldMousePos);
        mouseOffset = rectTransform.position - worldMousePos;

        // Make the item semi-transparent and allow raycasting through it
        canvasGroup.alpha = 0.5f;
        canvasGroup.blocksRaycasts = false;

        // If the item is already in the grid, remove it
        if (currentGridPos != Vector2.zero)
        {
            string itemName = gameObject.name;
            grid.RemoveItemFromGridUI(itemName, currentGridPos, parentTransform);
            grid.RemoveItemToGridUI(gameObject);
            currentGridPos = Vector2.zero;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out Vector3 worldMousePos))
        {
            rectTransform.position = worldMousePos + mouseOffset;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Raycast to detect where the item is dropped
        RaycastResult raycastResult = eventData.pointerCurrentRaycast;

        if (raycastResult.gameObject == null)
        {
            // If no valid drop zone is found, keep the object at the drop location
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            return;
        }

        Debug.Log(raycastResult.gameObject.name);

        string itemName = gameObject.name;

        // Handle drop on the destination layer
        if (IsLayer(raycastResult.gameObject.layer, destinationLayer))
        {
            string parentItemName = raycastResult.gameObject.transform.parent.name;
            Vector2 gridPosition = ConvertStringToVector2(parentItemName);

            // Move the item to the destination and update the grid
            SetTransform(raycastResult.gameObject.transform);
            currentGridPos = gridPosition;
            grid.AddGameObjectsToGridUI(itemName, gridPosition);
            grid.AddItemToGridUI(gameObject);
        }
        // Handle drop on the item-to-craft layer
        else if (IsLayer(raycastResult.gameObject.layer, itemToCraftLayer))
        {
            SetTransform(raycastResult.gameObject.transform);
            ItemToCraft.itemIsCorrect?.Invoke(itemName); // Check if the item is correct
        }

        // Restore the original appearance
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    private void SetTransform(Transform targetTransform)
    {
        transform.position = targetTransform.position;
        transform.SetParent(targetTransform);
    }

    private bool IsLayer(int layer, LayerMask layerMask)
    {
        return (layerMask.value & (1 << layer)) != 0;
    }

    private Vector2 ConvertStringToVector2(string input)
    {
        input = input.Trim('(', ')');
        string[] values = input.Split(',');

        float x = float.Parse(values[0]);
        float y = float.Parse(values[1]);

        return new Vector2(x, y);
    }
}

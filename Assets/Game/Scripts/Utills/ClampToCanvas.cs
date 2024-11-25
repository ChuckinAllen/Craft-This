using UnityEngine;

public class ClampToCanvas : MonoBehaviour
{
    private RectTransform rectTransform;
    public RectTransform canvasRectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        //canvasRectTransform = FindFirstObjectByType<Canvas>().GetComponent<RectTransform>();
    }

    void LateUpdate()
    {
        ClampToCanvasBounds();
    }

    private void ClampToCanvasBounds()
    {
        if (canvasRectTransform == null || rectTransform == null)
            return;

        // Get the canvas bounds in world space
        Vector3[] canvasCorners = new Vector3[4];
        canvasRectTransform.GetWorldCorners(canvasCorners);
        Vector3 canvasMin = canvasCorners[0]; // Bottom-left corner
        Vector3 canvasMax = canvasCorners[2]; // Top-right corner

        // Get the size of the element in world space
        Vector3[] elementCorners = new Vector3[4];
        rectTransform.GetWorldCorners(elementCorners);
        Vector3 elementSize = elementCorners[2] - elementCorners[0];

        // Clamp the position in world space
        Vector3 clampedPosition = rectTransform.position;

        clampedPosition.x = Mathf.Clamp(
            clampedPosition.x,
            canvasMin.x + elementSize.x / 2f,
            canvasMax.x - elementSize.x / 2f
        );
        clampedPosition.y = Mathf.Clamp(
            clampedPosition.y,
            canvasMin.y + elementSize.y / 2f,
            canvasMax.y - elementSize.y / 2f
        );

        // Apply the clamped position
        rectTransform.position = clampedPosition;
    }
}

using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class DrawArrow : MonoBehaviour
{
    public Transform startTransform;    // Start position of the arrow
    public Transform endTransform;      // End position of the arrow
    public Color arrowColor = Color.red;    // Color of the arrow
    public float arrowHeadLength = 0.5f;    // Length of the arrowhead
    public float arrowHeadAngle = 20f;      // Angle of the arrowhead

    private LineRenderer lineRenderer;

    private void Start()
    {
        // Get the LineRenderer component and set up its properties
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = 10f;
        lineRenderer.endWidth = 5f;
        lineRenderer.positionCount = 3;  // We’ll use 3 points (line + arrowhead)
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));  // Simple shader for solid color
        lineRenderer.startColor = arrowColor;
        lineRenderer.endColor = arrowColor;
        lineRenderer.sortingLayerName = "Default";  // Make sure this is a layer visible to your camera
        lineRenderer.sortingOrder = 10;  // Higher numbers render on top
        lineRenderer.useWorldSpace = true;
    }

    private void Update()
    {
        if (startTransform != null && endTransform != null)
        {
            // Draw main line from start to end
            Vector3 startPosition = startTransform.position;
            Vector3 endPosition = endTransform.position;
            lineRenderer.SetPosition(0, startPosition);
            lineRenderer.SetPosition(1, endPosition);

            // Draw arrowhead
            DrawArrowHead(startPosition, endPosition);
        }
    }

    private void DrawArrowHead(Vector3 start, Vector3 end)
    {
        Vector3 direction = (end - start).normalized;

        // Calculate the positions for the arrowhead
        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * Vector3.forward;
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * Vector3.forward;
        Vector3 arrowTipRight = end + right * arrowHeadLength;
        Vector3 arrowTipLeft = end + left * arrowHeadLength;

        // Set the third point as the arrow tip
        lineRenderer.positionCount = 4;
        lineRenderer.SetPosition(2, arrowTipRight);
        lineRenderer.SetPosition(3, arrowTipLeft);
    }
}

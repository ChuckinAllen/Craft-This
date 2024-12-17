using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RaycastCheck : MonoBehaviour, IPointerMoveHandler
{
    [SerializeField] private Camera mainCamera; // Assign the main camera in the inspector
    [SerializeField] private LayerMask layerMask; // Optional: filter which objects to include (set to "Everything" by default)

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main; // Automatically set the main camera if not assigned
        }

        if (layerMask == 0)
        {
            layerMask = ~0; // Set to "Everything" if no layer mask is specified
        }
    }
    public void OnPointerMove(PointerEventData eventData)
    {
        Vector3 mousePosition = Input.mousePosition;

        // Convert mouse position to a world position
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);

        Debug.Log($"Mouse Screen Position: {mousePosition}");

        // Find the first object hit by the ray (if a collider exists)
        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, layerMask);

        if (hits.Length > 0)
        {
            foreach (RaycastHit hit in hits)
            {
                Debug.Log($"Hit object: {hit.collider.name}");
            }
        }
        else
        {
            Debug.Log("No physical objects hit.");
        }

        // Optionally: Find objects under the cursor by position or other criteria
        GameObject hitUI = FindUIElementUnderCursor(eventData);
        if (hitUI != null)
        {
            Debug.Log($"Hit UI object: {hitUI.name}");
        }
    }

    private GameObject FindUIElementUnderCursor(PointerEventData eventData)
    {
        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        if (results.Count > 0)
        {
            return results[0].gameObject; // Return the first UI object hit
        }

        return null;
    }


}

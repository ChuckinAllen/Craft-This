using Sirenix.OdinInspector;
using UnityEngine;

public class AlwaysRenderOnTop : MonoBehaviour
{
    public bool isUI = false;
    public int sortingOrder = 1000; // Higher number = rendered on top

    [Button]
    void Start()
    {
        if (isUI)
        {
            // UI Element
            transform.SetAsLastSibling();
        }
        else
        {
            // 3D or Sprite Renderer
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.renderQueue = 3000;
                renderer.sortingOrder = sortingOrder;
            }
        }
    }
}
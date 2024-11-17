using UnityEngine;
using UnityEngine.UI;

public class ArrowUI : MonoBehaviour
{
    public RectTransform StartTransform;  // Start point of the arrow
    public RectTransform EndTransform;    // End point of the arrow
    public CanvasGroup arrowCanvasGroup;

    public float ArrowWidth = 10f;        // Width of the arrow shaft
    public float ArrowHeadSize = 20f;     // Size of the arrowhead
    public float ScaleFactor = 1f;        // Scale factor to adjust arrow length
    public float FadeDuration = 1f;       // Time to fade in and out

    private RectTransform arrowRectTransform;
    
    private float fadeTimer;
    private bool isFadingIn = true;

    void Start()
    {
        arrowRectTransform = GetComponent<RectTransform>();

        // Ensure the image is anchored to the middle center for correct scaling
        arrowRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        arrowRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
    }

    void Update()
    {
        if (StartTransform == null || EndTransform == null)
            return;

        // Set position to midpoint between start and end points
        Vector3 startPosition = StartTransform.position;
        Vector3 endPosition = EndTransform.position;
        Vector3 direction = endPosition - startPosition;
        Vector3 midpoint = startPosition + direction / 2;

        arrowRectTransform.position = midpoint;
        arrowRectTransform.localScale = Vector3.one;

        // Set rotation to point from start to end
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        arrowRectTransform.rotation = Quaternion.Euler(0, 0, angle);

        // Set width and length of the arrow image
        arrowRectTransform.sizeDelta = new Vector2(direction.magnitude * ScaleFactor, ArrowWidth);

        UpdateFadeEffect();
    }

    void UpdateFadeEffect()
    {
        // Update fade timer and reverse direction when reaching the limit
        fadeTimer += (isFadingIn ? Time.deltaTime : -Time.deltaTime);
        if (fadeTimer >= FadeDuration)
        {
            fadeTimer = FadeDuration;
            isFadingIn = false;
        }
        else if (fadeTimer <= 0f)
        {
            fadeTimer = 0f;
            isFadingIn = true;
        }

        // Set opacity based on fade timer
        float alpha = fadeTimer / FadeDuration;
        arrowCanvasGroup.alpha = alpha;
    }
}

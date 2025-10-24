using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ArrowAnimation : MonoBehaviour
{
    [Header("Scroll Settings")]
    public float scrollSpeedX = 0.5f;
    public float scrollSpeedY = 0f;

    [Tooltip("Choose which map to scroll: _BaseMap (color) or _EmissionMap (glow)")]
    public string textureProperty = "_BaseMap";

    private Renderer rend;
    private Material matInstance;
    private Vector2 offset;

    void Start()
    {
        rend = GetComponent<Renderer>();
        if (!rend)
        {
            Debug.LogError($"{name}: Renderer not found!");
            enabled = false;
            return;
        }

        // Create a unique runtime instance of the material
        matInstance = rend.material;

        // Get current offset (if any)
        offset = matInstance.GetTextureOffset(textureProperty);
    }

    void Update()
    {
        if (matInstance == null) return;

        offset.x += scrollSpeedX * Time.deltaTime;
        offset.y += scrollSpeedY * Time.deltaTime;

        matInstance.SetTextureOffset(textureProperty, offset);
    }

    /// <summary>
    /// Dynamically set scroll speed (e.g., for roads or neon lights).
    /// </summary>
    public void SetScrollSpeed(float x, float y)
    {
        scrollSpeedX = x;
        scrollSpeedY = y;
    }

    /// <summary>
    /// Reset scroll offset back to zero.
    /// </summary>
    public void ResetOffset()
    {
        offset = Vector2.zero;
        if (matInstance)
            matInstance.SetTextureOffset(textureProperty, offset);
    }
}

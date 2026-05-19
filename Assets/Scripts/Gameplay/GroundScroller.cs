using UnityEngine;
using UnityEngine.UI;

public class GroundScroller : MonoBehaviour
{
    [SerializeField] private float baseSpeed = 5f;

    private float _halfWidth;

    // Shared flag so only the first instance creates the canvas visual
    private static bool _floorVisualCreated = false;

    void Start()
    {
        // Hide sprite — visual is handled by a single seamless Canvas element
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = false;

        if (!_floorVisualCreated)
        {
            _floorVisualCreated = true;
            CreateCanvasFloor();
        }

        // Size collider piece to cover the full camera width + buffer
        if (Camera.main != null)
        {
            float camWidth = Camera.main.orthographicSize * Camera.main.aspect * 2f;
            Vector3 s = transform.localScale;
            s.x = Mathf.Max(s.x, camWidth + 4f);
            transform.localScale = s;
        }

        float w = transform.localScale.x;
        float snapped = Mathf.Round(transform.position.x / w) * w;
        transform.position = new Vector3(snapped, transform.position.y, transform.position.z);
        _halfWidth = w * 0.5f;
    }

    void OnDestroy()
    {
        // Reset so the visual is recreated on scene reload
        _floorVisualCreated = false;
    }

    void CreateCanvasFloor()
    {
        if (Camera.main == null) return;

        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null) return;

        // How much of the screen (bottom fraction) the floor + below-ground area occupies
        float camHeight   = Camera.main.orthographicSize * 2f;
        float camBottomY  = -Camera.main.orthographicSize;
        float groundTopY  = transform.position.y + transform.localScale.y * 0.5f;
        float heightFrac  = (groundTopY - camBottomY) / camHeight;

        GameObject go = new GameObject("GroundVisual");
        go.transform.SetParent(canvas.transform, false);
        go.transform.SetAsFirstSibling(); // render behind all other UI

        var img = go.AddComponent<Image>();
        img.color = new Color(0.038f, 0.038f, 0.038f, 1f);
        img.raycastTarget = false;

        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin  = new Vector2(0f, 0f);
        rt.anchorMax  = new Vector2(1f, heightFrac);
        rt.offsetMin  = Vector2.zero;
        rt.offsetMax  = Vector2.zero;
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver())
            return;

        float speed = DifficultyManager.Instance != null
            ? DifficultyManager.Instance.CurrentSpeed
            : baseSpeed;

        transform.Translate(Vector3.left * speed * Time.deltaTime);

        if (transform.position.x + _halfWidth < -_halfWidth)
        {
            transform.position = new Vector3(
                transform.position.x + transform.localScale.x * 2f,
                transform.position.y,
                transform.position.z);
        }
    }
}

using UnityEngine;
using UnityEngine.Pool;
using TMPro;
using System.Collections;

public class Coin : MonoBehaviour
{
    [SerializeField] private float spinSpeed = 2.5f;
    [SerializeField] private float destroyX  = -14f;

    public IObjectPool<GameObject> Pool { get; set; }

    private bool _collected;
    private SpriteRenderer _sr;

    void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        _collected = false;
        if (_sr != null) _sr.enabled = true;
        transform.localScale = Vector3.one;
        StartCoroutine(SpinRoutine());
    }

    void OnDisable()
    {
        StopAllCoroutines();
        transform.localScale = Vector3.one;
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver()) return;
        if (_collected) return;

        float speed = DifficultyManager.Instance != null
            ? DifficultyManager.Instance.CurrentSpeed
            : 5f;
        transform.Translate(Vector2.left * speed * Time.deltaTime);

        if (transform.position.x <= destroyX)
            ReturnToPool();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (_collected) return;
        if (other.GetComponent<PlayerController>() == null) return;

        _collected = true;
        Collect();
    }

    public void Collect()
    {
        if (_sr != null) _sr.enabled = false;

        if (ScoreManager.Instance != null)
            ScoreManager.Instance.AddPickupCoin();

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayCoin();

        StartCoroutine(FloatingTextRoutine());
    }

    IEnumerator SpinRoutine()
    {
        while (true)
        {
            // PingPong 0→1→0 gives the "coin edge-on and back" look
            float t = Mathf.PingPong(Time.time * spinSpeed, 1f);
            float sx = Mathf.Lerp(0.08f, 1f, t);
            transform.localScale = new Vector3(sx, 1f, 1f);
            yield return null;
        }
    }

    IEnumerator FloatingTextRoutine()
    {
        GameObject go = new GameObject("CoinPopup");
        go.transform.position = transform.position + Vector3.up * 0.4f;

        var tmp = go.AddComponent<TextMeshPro>();
        tmp.text = "+1";
        tmp.fontSize = 2f;
        tmp.color = new Color(1f, 0.85f, 0.1f, 1f);
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = FontStyles.Bold;

        float duration = 0.75f;
        float elapsed  = 0f;
        Vector3 start  = go.transform.position;

        while (elapsed < duration)
        {
            float pct = elapsed / duration;
            go.transform.position = start + Vector3.up * pct * 1.2f;
            tmp.color = new Color(1f, 0.85f, 0.1f, 1f - pct);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(go);
        ReturnToPool();
    }

    void ReturnToPool()
    {
        if (Pool != null)
            Pool.Release(gameObject);
        else
            gameObject.SetActive(false);
    }
}

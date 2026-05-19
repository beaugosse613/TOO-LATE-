using UnityEngine;

public class ParallaxScroller : MonoBehaviour
{
    [SerializeField] private float baseSpeed = 2f;
    [SerializeField] private float speedScaleFactor = 0.4f;
    [SerializeField] private float resetX = -20f;
    [SerializeField] private float startX = 20f;

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver())
            return;

        float score = ScoreManager.Instance != null ? ScoreManager.Instance.GetCurrentScore() : 0f;
        float speed = baseSpeed + (score / 20f) * speedScaleFactor;

        transform.Translate(Vector3.left * speed * Time.deltaTime);

        if (transform.position.x <= resetX)
            transform.position = new Vector3(startX, transform.position.y, transform.position.z);
    }
}

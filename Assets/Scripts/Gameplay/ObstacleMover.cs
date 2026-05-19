using UnityEngine;
using UnityEngine.Pool;

public class ObstacleMover : MonoBehaviour
{
    [SerializeField] private float baseSpeed = 5f;
    [SerializeField] private float destroyX = -12f;

    public IObjectPool<GameObject> Pool { get; set; }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver())
            return;

        float currentSpeed = DifficultyManager.Instance != null
            ? DifficultyManager.Instance.CurrentSpeed
            : baseSpeed;
        transform.Translate(Vector2.left * currentSpeed * Time.deltaTime);

        if (transform.position.x <= destroyX)
        {
            if (Pool != null)
                Pool.Release(gameObject);
            else
                Destroy(gameObject);
        }
    }
}

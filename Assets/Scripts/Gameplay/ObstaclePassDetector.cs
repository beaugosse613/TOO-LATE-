using UnityEngine;

public class ObstaclePassDetector : MonoBehaviour
{
    [SerializeField] private float playerX = -5f;
    private bool counted = false;

    void OnEnable()
    {
        counted = false;
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver())
            return;

        if (!counted && transform.position.x < playerX)
        {
            counted = true;

            if (ScoreManager.Instance != null)
                ScoreManager.Instance.AddCombo();
        }
    }
}

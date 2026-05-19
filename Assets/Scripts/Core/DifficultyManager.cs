using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance { get; private set; }

    [Header("Score Thresholds")]
    [Tooltip("Score before the difficulty ramp begins.")]
    [SerializeField] private float gracePeriodScore    = 20f;
    [Tooltip("Score at which difficulty is fully ramped.")]
    [SerializeField] private float fullDifficultyScore = 250f;

    [Header("Speed")]
    [Tooltip("Movement speed at the start of the game.")]
    [SerializeField] private float minSpeed = 5f;
    [Tooltip("Maximum movement speed — never exceeded.")]
    [SerializeField] private float maxSpeed = 11f;

    // 0-1 difficulty value, smoothstepped for a fair ramp
    public float DifficultyT   { get; private set; }
    public float CurrentSpeed  { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        int score = ScoreManager.Instance != null ? ScoreManager.Instance.GetCurrentScore() : 0;

        float adjusted = Mathf.Max(0f, score - gracePeriodScore);
        float range    = Mathf.Max(1f, fullDifficultyScore - gracePeriodScore);
        float raw      = Mathf.Clamp01(adjusted / range);

        // Smoothstep eases in slowly, accelerates, then plateaus — feels fair
        DifficultyT  = raw * raw * (3f - 2f * raw);
        CurrentSpeed = Mathf.Lerp(minSpeed, maxSpeed, DifficultyT);
    }
}

using UnityEngine;
using UnityEngine.Pool;
using System.Collections;

public class ObstacleSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] obstaclePrefabs;

    [SerializeField] private float spawnX = 11f;
    [SerializeField] private float spawnY = -2.07f;

    [SerializeField] private float startMinDelay = 1.2f;
    [SerializeField] private float startMaxDelay = 2.3f;

    [SerializeField] private float hardestMinDelay = 0.55f;
    [SerializeField] private float hardestMaxDelay = 1.0f;

    [SerializeField] private float absoluteMinDelay = 0.45f;

    public static ObstacleSpawner Instance;

    // Shared with GroundScroller and ParallaxScroller for speed sync
    public static float DifficultyPercent { get; private set; }

    private ObjectPool<GameObject>[] _pools;
    private Coroutine _spawnCoroutine;

    void Awake()
    {
        Instance = this;
        InitPools();
    }

    void InitPools()
    {
        if (obstaclePrefabs == null || obstaclePrefabs.Length == 0) return;

        _pools = new ObjectPool<GameObject>[obstaclePrefabs.Length];

        for (int i = 0; i < obstaclePrefabs.Length; i++)
        {
            int index = i;
            _pools[i] = new ObjectPool<GameObject>(
                createFunc:      () => Instantiate(obstaclePrefabs[index]),
                actionOnGet:     go => go.SetActive(true),
                actionOnRelease: go => go.SetActive(false),
                actionOnDestroy: go => Destroy(go),
                collectionCheck: true,
                defaultCapacity: 5,
                maxSize: 20
            );
        }
    }

    void Start()
    {
        _spawnCoroutine = StartCoroutine(SpawnLoop());
    }

    public void StopSpawning()
    {
        if (_spawnCoroutine != null)
            StopCoroutine(_spawnCoroutine);
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            UpdateDifficulty();

            float minDelay = Mathf.Max(Mathf.Lerp(startMinDelay, hardestMinDelay, DifficultyPercent), absoluteMinDelay);
            float maxDelay = Mathf.Max(Mathf.Lerp(startMaxDelay, hardestMaxDelay, DifficultyPercent), absoluteMinDelay + 0.1f);

            yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));

            SpawnRandomObstacle();
        }
    }

    void UpdateDifficulty()
    {
        int score = ScoreManager.Instance != null ? ScoreManager.Instance.GetCurrentScore() : 0;
        DifficultyPercent = Mathf.Clamp01(score / 300f);
    }

    void SpawnRandomObstacle()
    {
        if (_pools == null || _pools.Length == 0) return;

        int index = Random.Range(0, _pools.Length);
        GameObject go = _pools[index].Get();

        go.transform.position = new Vector3(spawnX, spawnY, 0f);
        go.transform.rotation = Quaternion.identity;

        ObstacleMover mover = go.GetComponent<ObstacleMover>();
        if (mover != null)
            mover.Pool = _pools[index];
    }
}

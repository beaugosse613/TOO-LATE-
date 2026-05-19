using UnityEngine;
using UnityEngine.Pool;
using System.Collections;

public class CoinSpawner : MonoBehaviour
{
    public static CoinSpawner Instance;

    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private float spawnX    = 12f;
    [SerializeField] private float minY      = -1.5f;
    [SerializeField] private float maxY      =  0.5f;
    [SerializeField] private float minDelay  =  1.5f;
    [SerializeField] private float maxDelay  =  4.0f;

    private ObjectPool<GameObject> _pool;
    private Coroutine _spawnCoroutine;

    void Awake()
    {
        Instance = this;

        _pool = new ObjectPool<GameObject>(
            createFunc:      () => Instantiate(coinPrefab),
            actionOnGet:     go => go.SetActive(true),
            actionOnRelease: go => go.SetActive(false),
            actionOnDestroy: go => Destroy(go),
            collectionCheck: true,
            defaultCapacity: 10,
            maxSize:         30
        );
    }

    void Start()
    {
        if (coinPrefab != null)
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
            yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));
            SpawnCoin();
        }
    }

    void SpawnCoin()
    {
        GameObject go = _pool.Get();
        go.transform.position = new Vector3(spawnX, Random.Range(minY, maxY), 0f);
        go.transform.rotation = Quaternion.identity;

        Coin coin = go.GetComponent<Coin>();
        if (coin != null)
            coin.Pool = _pool;
    }
}

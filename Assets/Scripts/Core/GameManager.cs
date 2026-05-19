using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private GameObject gameOverPanel;

    private bool isGameOver = false;

    void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;
        Application.targetFrameRate = 60;
    }

    void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    void Update()
    {
        if (Application.platform == RuntimePlatform.Android && Input.GetKeyDown(KeyCode.Escape))
        {
            if (isGameOver) SceneManager.LoadScene("MainMenu");
            else            Application.Quit();
            return;
        }

        if (isGameOver && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)))
            RestartGame();
    }

    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;

        if (Camera.main != null)
        {
            CameraShake shake = Camera.main.GetComponent<CameraShake>();
            if (shake != null)
                shake.Shake(0.35f, 0.5f);
        }

        if (ScoreManager.Instance != null)
            ScoreManager.Instance.OnGameOver();

        if (ObstacleSpawner.Instance != null)
            ObstacleSpawner.Instance.StopSpawning();

        StartCoroutine(GameOverSequence());
    }

    IEnumerator GameOverSequence()
    {
        float elapsed = 0f;
        float duration = 0.45f;

        while (elapsed < duration)
        {
            Time.timeScale = Mathf.Lerp(1f, 0f, elapsed / duration);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        Time.timeScale = 0f;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public bool IsGameOver() => isGameOver;
}

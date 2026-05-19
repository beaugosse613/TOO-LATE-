using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [Header("Title")]
    [SerializeField] private RectTransform titleTransform;
    [SerializeField] private float         floatAmplitude = 10f;
    [SerializeField] private float         floatSpeed     = 1.1f;

    [Header("Stats")]
    [SerializeField] private TextMeshProUGUI bestScoreText;
    [SerializeField] private TextMeshProUGUI totalCoinsText;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI versionText;
    [SerializeField] private TextMeshProUGUI soundToggleLabel;

    [Header("Fade")]
    [SerializeField] private CanvasGroup fadeOverlay;
    [SerializeField] private float       fadeDuration = 0.4f;

    private Vector2 _titleOrigin;

    void Start()
    {
        if (titleTransform != null)
            _titleOrigin = titleTransform.anchoredPosition;

        // Stats
        int best  = PlayerPrefs.GetInt("BestScore", 0);
        int coins = PlayerPrefs.GetInt("Coins", 0);
        if (bestScoreText  != null) bestScoreText.text  = "BEST    " + best;
        if (totalCoinsText != null) totalCoinsText.text = "COINS   " + coins;

        // Version
        if (versionText != null)
            versionText.text = "v" + Application.version;

        RefreshSoundLabel();

        // Fade in
        if (fadeOverlay != null)
            StartCoroutine(FadeRoutine(1f, 0f, () => fadeOverlay.gameObject.SetActive(false)));
    }

    void Update()
    {
        if (titleTransform == null) return;
        float y = _titleOrigin.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        titleTransform.anchoredPosition = new Vector2(_titleOrigin.x, y);
    }

    // ── Button callbacks ─────────────────────────────────────────────────────

    public void PlayGame()  => StartCoroutine(LoadWithFade("GamePlayScene"));
    public void OpenShop()  => StartCoroutine(LoadWithFade("Shopscene"));
    public void QuitGame()  => Application.Quit();

    public void ToggleSound()
    {
        bool muted = PlayerPrefs.GetInt("SoundMuted", 0) == 1;
        muted = !muted;
        PlayerPrefs.SetInt("SoundMuted", muted ? 1 : 0);
        PlayerPrefs.Save();
        AudioListener.volume = muted ? 0f : 1f;
        RefreshSoundLabel();
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    void RefreshSoundLabel()
    {
        if (soundToggleLabel == null) return;
        bool muted = PlayerPrefs.GetInt("SoundMuted", 0) == 1;
        soundToggleLabel.text = muted ? "SFX  OFF" : "SFX  ON";
    }

    IEnumerator LoadWithFade(string sceneName)
    {
        if (fadeOverlay != null)
        {
            fadeOverlay.gameObject.SetActive(true);
            yield return StartCoroutine(FadeRoutine(0f, 1f, null));
        }
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator FadeRoutine(float from, float to, System.Action onComplete)
    {
        if (fadeOverlay == null) yield break;

        fadeOverlay.gameObject.SetActive(true);
        fadeOverlay.blocksRaycasts = true;

        float t = 0f;
        while (t < fadeDuration)
        {
            fadeOverlay.alpha = Mathf.Lerp(from, to, t / fadeDuration);
            t += Time.deltaTime;
            yield return null;
        }

        fadeOverlay.alpha = to;
        fadeOverlay.blocksRaycasts = (to > 0.5f);
        onComplete?.Invoke();
    }
}

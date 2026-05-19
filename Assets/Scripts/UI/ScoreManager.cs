using UnityEngine;
using TMPro;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI bestText;
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI milestoneText;
    [SerializeField] private TextMeshProUGUI comboText;

    [Header("Settings")]
    [SerializeField] private float scoreRate = 8f;
    [SerializeField] private int coinsPerScoreBlock = 50;

    private float score;
    private int currentScore;
    private int bestScore;
    private int totalCoins;
    private int coinsThisRun;
    private int combo;

    private int[] milestones = { 50, 100, 200, 350, 500, 750, 1000 };
    private int milestoneIndex = 0;

    private int _pickupCoins;
    private int _lastCoinsThisRun;
    private Coroutine _coinPulseCoroutine;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Safety check: prevents ScoreManager from crashing in MainMenu or Shop scenes
        if (scoreText == null)
        {
            Debug.LogWarning("ScoreManager disabled because this scene has no gameplay UI.");
            enabled = false;
            return;
        }

        bestScore = SaveManager.Instance != null
            ? SaveManager.Instance.GetBestScore()
            : PlayerPrefs.GetInt("BestScore", 0);

        totalCoins = SaveManager.Instance != null
            ? SaveManager.Instance.GetCoins()
            : PlayerPrefs.GetInt("Coins", 0);

        score = 0f;
        currentScore = 0;
        coinsThisRun = 0;
        _pickupCoins = 0;
        _lastCoinsThisRun = 0;
        combo = 0;
        milestoneIndex = 0;

        if (finalScoreText != null) finalScoreText.gameObject.SetActive(false);
        if (milestoneText != null)  milestoneText.gameObject.SetActive(false);
        if (comboText != null)      comboText.gameObject.SetActive(false);

        UpdateUI();
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver())
            return;

        score += Time.deltaTime * scoreRate;
        currentScore = Mathf.FloorToInt(score);

        int newCoins = currentScore / coinsPerScoreBlock + _pickupCoins;
        if (newCoins > _lastCoinsThisRun)
        {
            coinsThisRun = newCoins;
            _lastCoinsThisRun = newCoins;
            PulseCoinText();
        }
        else
        {
            coinsThisRun = newCoins;
        }

        CheckMilestones();
        UpdateUI();
    }

    void CheckMilestones()
    {
        if (milestoneIndex >= milestones.Length) return;

        if (currentScore >= milestones[milestoneIndex])
        {
            ShowMilestone(milestones[milestoneIndex]);
            milestoneIndex++;
        }
    }

    void ShowMilestone(int value)
    {
        if (milestoneText == null) return;

        switch (value)
        {
            case 50:   milestoneText.text = "FAST!";      break;
            case 100:  milestoneText.text = "INSANE!";    break;
            case 200:  milestoneText.text = "UNREAL!";    break;
            case 350:  milestoneText.text = "LEGEND!";    break;
            case 500:  milestoneText.text = "GODLIKE!";   break;
            case 750:  milestoneText.text = "MONSTER!";   break;
            case 1000: milestoneText.text = "IMMORTAL!";  break;
        }

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMilestone();

        if (Camera.main != null)
        {
            CameraShake shake = Camera.main.GetComponent<CameraShake>();
            if (shake != null)
                shake.Shake(0.12f, 0.2f);
        }

        StartCoroutine(ShowTemporary(milestoneText));
    }

    public void AddPickupCoin()
    {
        _pickupCoins++;
        PulseCoinText();
        UpdateUI();
    }

    public void AddCombo()
    {
        combo++;

        if      (combo == 5)  ShowCombo($"NICE! x{combo}");
        else if (combo == 10) ShowCombo($"HOT STREAK! x{combo}");
        else if (combo == 20) ShowCombo($"ON FIRE! x{combo}");
        else if (combo == 35) ShowCombo($"UNSTOPPABLE! x{combo}");
        else if (combo == 50) ShowCombo($"TOO LATE LEGEND! x{combo}");
    }

    void ShowCombo(string message)
    {
        if (comboText == null) return;

        comboText.text = message;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMilestone();

        StartCoroutine(ShowTemporary(comboText));
    }

    IEnumerator ShowTemporary(TextMeshProUGUI txt)
    {
        txt.gameObject.SetActive(true);
        txt.transform.localScale = Vector3.one * 1.35f;

        float timer = 0f;
        while (timer < 0.25f)
        {
            txt.transform.localScale = Vector3.Lerp(Vector3.one * 1.35f, Vector3.one, timer / 0.25f);
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        txt.transform.localScale = Vector3.one;
        yield return new WaitForSeconds(0.6f);
        txt.gameObject.SetActive(false);
    }

    void PulseCoinText()
    {
        if (_coinPulseCoroutine != null)
            StopCoroutine(_coinPulseCoroutine);
        _coinPulseCoroutine = StartCoroutine(CoinPulseRoutine());
    }

    IEnumerator CoinPulseRoutine()
    {
        if (coinText == null) yield break;

        Vector3 original = Vector3.one;
        Vector3 punched  = Vector3.one * 1.25f;
        float   t        = 0f;

        while (t < 0.08f)
        {
            coinText.transform.localScale = Vector3.Lerp(original, punched, t / 0.08f);
            t += Time.unscaledDeltaTime;
            yield return null;
        }

        t = 0f;
        while (t < 0.12f)
        {
            coinText.transform.localScale = Vector3.Lerp(punched, original, t / 0.12f);
            t += Time.unscaledDeltaTime;
            yield return null;
        }

        coinText.transform.localScale = original;
    }

    public void ResetCombo()
    {
        combo = 0;
    }

    public int GetCurrentScore()
    {
        return currentScore;
    }

    public void OnGameOver()
    {
        ResetCombo();

        if (currentScore > bestScore)
        {
            bestScore = currentScore;
            if (SaveManager.Instance != null)
                SaveManager.Instance.SetBestScore(bestScore);
            else
                PlayerPrefs.SetInt("BestScore", bestScore);
        }

        totalCoins += coinsThisRun;
        if (SaveManager.Instance != null)
            SaveManager.Instance.SetCoins(totalCoins);
        else
        {
            PlayerPrefs.SetInt("Coins", totalCoins);
            PlayerPrefs.Save();
        }

        if (finalScoreText != null)
        {
            finalScoreText.gameObject.SetActive(true);
            finalScoreText.text =
                "Score: " + currentScore +
                "\nCoins Earned: +" + coinsThisRun +
                "\nTotal Coins: " + totalCoins;
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "<size=20><color=#AAAAAA>SCORE</color></size>\n<size=62><b>" + currentScore + "</b></size>";

        if (bestText != null)
            bestText.text = "<size=20><color=#AAAAAA>BEST</color></size>\n<b>" + bestScore + "</b>";

        if (coinText != null)
        {
            string coinsStr = "<b>COINS  " + totalCoins + "</b>";
            if (coinsThisRun > 0)
                coinsStr += "  <size=18><color=#AAAAAA>+" + coinsThisRun + "</color></size>";
            coinText.text = coinsStr;
        }
    }
}

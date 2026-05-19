using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class ShopManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private TextMeshProUGUI feedbackText;

    [Header("Button Labels")]
    [SerializeField] private TextMeshProUGUI greenButtonLabel;
    [SerializeField] private TextMeshProUGUI redButtonLabel;
    [SerializeField] private TextMeshProUGUI goldButtonLabel;
    [SerializeField] private TextMeshProUGUI purpleButtonLabel;

    private int coins;
    private Coroutine _feedbackCoroutine;

    void Start()
    {
        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);

        RefreshCoins();
        RefreshButtonLabels();
    }

    void RefreshCoins()
    {
        coins = SaveManager.Instance.GetCoins();

        if (coinsText != null)
            coinsText.text = "Coins: " + coins;
    }

    void RefreshButtonLabels()
    {
        RefreshButtonLabel("Green",  25,  greenButtonLabel);
        RefreshButtonLabel("Red",    50,  redButtonLabel);
        RefreshButtonLabel("Gold",   100, goldButtonLabel);
        RefreshButtonLabel("Purple", 250, purpleButtonLabel);
    }

    void RefreshButtonLabel(string skinName, int cost, TextMeshProUGUI label)
    {
        if (label == null) return;
        bool owned = SaveManager.Instance.GetSkinOwned(skinName);
        label.text = owned ? "Equip" : $"Buy ({cost})";
    }

    public void BuyGreen()  { BuySkin("Green",  25,  greenButtonLabel);  }
    public void BuyRed()    { BuySkin("Red",    50,  redButtonLabel);    }
    public void BuyGold()   { BuySkin("Gold",   100, goldButtonLabel);   }
    public void BuyPurple() { BuySkin("Purple", 250, purpleButtonLabel); }

    void BuySkin(string skinName, int cost, TextMeshProUGUI buttonLabel)
    {
        coins = SaveManager.Instance.GetCoins();

        bool alreadyOwned = SaveManager.Instance.GetSkinOwned(skinName);

        if (alreadyOwned)
        {
            EquipSkin(skinName);
            Debug.Log($"[Shop] Equipped '{skinName}' (already owned).");
            ShowFeedback("Equipped!", Color.cyan);
            return;
        }

        if (coins >= cost)
        {
            coins -= cost;

            SaveManager.Instance.SetCoins(coins);
            SaveManager.Instance.SetSkinOwned(skinName);
            EquipSkin(skinName);

            if (buttonLabel != null)
                buttonLabel.text = "Equip";

            Debug.Log($"[Shop] Purchased '{skinName}' for {cost} coins. Coins remaining: {coins}.");
            ShowFeedback("Bought & Equipped!", Color.green);
            RefreshCoins();
        }
        else
        {
            int shortfall = cost - coins;
            Debug.Log($"[Shop] Cannot buy '{skinName}' — need {shortfall} more coins (have {coins}, cost {cost}).");
            ShowFeedback($"Need {shortfall} more coins!", Color.red);
        }
    }

    void EquipSkin(string skinName)
    {
        SaveManager.Instance.SetSelectedSkin(skinName);
        Debug.Log($"[Shop] '{skinName}' set as active skin.");
    }

    void ShowFeedback(string message, Color color)
    {
        if (feedbackText == null) return;

        if (_feedbackCoroutine != null)
            StopCoroutine(_feedbackCoroutine);

        _feedbackCoroutine = StartCoroutine(FeedbackRoutine(message, color));
    }

    IEnumerator FeedbackRoutine(string message, Color color)
    {
        feedbackText.text = message;
        feedbackText.color = color;
        feedbackText.gameObject.SetActive(true);

        feedbackText.transform.localScale = Vector3.one * 1.3f;
        float timer = 0f;
        while (timer < 0.2f)
        {
            feedbackText.transform.localScale = Vector3.Lerp(Vector3.one * 1.3f, Vector3.one, timer / 0.2f);
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        feedbackText.transform.localScale = Vector3.one;

        yield return new WaitForSecondsRealtime(1.5f);

        timer = 0f;
        while (timer < 0.3f)
        {
            float a = Mathf.Lerp(1f, 0f, timer / 0.3f);
            feedbackText.color = new Color(color.r, color.g, color.b, a);
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        feedbackText.gameObject.SetActive(false);
    }

    public void BackToMenu()
    {
        Debug.Log("[Shop] Returning to Main Menu.");
        SceneManager.LoadScene("MainMenu");
    }
}

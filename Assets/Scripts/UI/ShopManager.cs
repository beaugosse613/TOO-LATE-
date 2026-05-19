using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [Serializable]
    public class SkinEntry
    {
        public string          skinName;
        public int             cost;
        public Button          button;
        public TextMeshProUGUI label;
    }

    [Header("Skins")]
    [SerializeField] private SkinEntry[] skins;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private Image           skinPreview;

    private string    _equippedSkin;
    private Coroutine _feedbackCoroutine;

    void Start()
    {
        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);

        _equippedSkin = SaveManager.Instance.GetSelectedSkin();
        RefreshAll();
    }

    void RefreshAll()
    {
        RefreshCoins();
        RefreshButtons();
        RefreshPreview();
    }

    void RefreshCoins()
    {
        if (coinsText != null)
            coinsText.text = "Coins: " + SaveManager.Instance.GetCoins();
    }

    void RefreshButtons()
    {
        if (skins == null) return;
        foreach (SkinEntry entry in skins)
        {
            if (entry.button == null || entry.label == null) continue;

            bool owned    = SaveManager.Instance.GetSkinOwned(entry.skinName);
            bool equipped = entry.skinName == _equippedSkin;

            if (equipped)
            {
                entry.label.text          = "Equipped";
                entry.button.interactable = false;
            }
            else if (owned)
            {
                entry.label.text          = "Equip";
                entry.button.interactable = true;
            }
            else
            {
                entry.label.text          = $"Buy ({entry.cost})";
                entry.button.interactable = true;
            }
        }
    }

    void RefreshPreview()
    {
        if (skinPreview != null)
            skinPreview.color = SkinColors.GetColor(_equippedSkin);
    }

    // Called by each Button's OnClick — pass the index matching the skins array order
    public void OnSkinButtonPressed(int index)
    {
        if (index < 0 || index >= skins.Length) return;

        SkinEntry entry = skins[index];
        bool      owned = SaveManager.Instance.GetSkinOwned(entry.skinName);

        if (owned)
        {
            Equip(entry.skinName);
            ShowFeedback("Equipped!", Color.cyan);
            return;
        }

        int coins = SaveManager.Instance.GetCoins();

        if (coins >= entry.cost)
        {
            SaveManager.Instance.SetCoins(coins - entry.cost);
            SaveManager.Instance.SetSkinOwned(entry.skinName);
            Equip(entry.skinName);
            RefreshCoins();
            ShowFeedback("Bought & Equipped!", Color.green);
        }
        else
        {
            ShowFeedback($"Need {entry.cost - coins} more coins!", Color.red);
        }
    }

    void Equip(string skinName)
    {
        _equippedSkin = skinName;
        SaveManager.Instance.SetSelectedSkin(skinName);
        RefreshButtons();
        RefreshPreview();
    }

    // Named wrappers so existing Button.OnClick assignments stay valid
    public void OnGreenButton()  => OnSkinButtonPressed(0);
    public void OnRedButton()    => OnSkinButtonPressed(1);
    public void OnGoldButton()   => OnSkinButtonPressed(2);
    public void OnPurpleButton() => OnSkinButtonPressed(3);

    public void BackToMenu() => SceneManager.LoadScene("MainMenu");

    void ShowFeedback(string message, Color color)
    {
        if (feedbackText == null) return;
        if (_feedbackCoroutine != null) StopCoroutine(_feedbackCoroutine);
        _feedbackCoroutine = StartCoroutine(FeedbackRoutine(message, color));
    }

    IEnumerator FeedbackRoutine(string message, Color color)
    {
        feedbackText.text  = message;
        feedbackText.color = color;
        feedbackText.gameObject.SetActive(true);

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
            feedbackText.color = new Color(color.r, color.g, color.b, Mathf.Lerp(1f, 0f, timer / 0.3f));
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        feedbackText.gameObject.SetActive(false);
    }
}

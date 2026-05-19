using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private static SaveManager _instance;

    public static SaveManager Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject("SaveManager");
                _instance = go.AddComponent<SaveManager>();
            }
            return _instance;
        }
    }

    private const string KEY_BEST_SCORE    = "BestScore";
    private const string KEY_COINS         = "Coins";
    private const string KEY_SELECTED_SKIN = "SelectedSkin";

    void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);
    }

    public int    GetBestScore()            => PlayerPrefs.GetInt(KEY_BEST_SCORE, 0);
    public int    GetCoins()                => PlayerPrefs.GetInt(KEY_COINS, 0);
    public string GetSelectedSkin()         => PlayerPrefs.GetString(KEY_SELECTED_SKIN, "Default");
    public bool   GetSkinOwned(string skin) => PlayerPrefs.GetInt(skin + "_Owned", 0) == 1;

    public void SetBestScore(int score)
    {
        PlayerPrefs.SetInt(KEY_BEST_SCORE, score);
        PlayerPrefs.Save();
    }

    public void SetCoins(int amount)
    {
        PlayerPrefs.SetInt(KEY_COINS, amount);
        PlayerPrefs.Save();
    }

    public void SetSelectedSkin(string skinName)
    {
        PlayerPrefs.SetString(KEY_SELECTED_SKIN, skinName);
        PlayerPrefs.Save();
    }

    public void SetSkinOwned(string skinName)
    {
        PlayerPrefs.SetInt(skinName + "_Owned", 1);
        PlayerPrefs.Save();
    }
}

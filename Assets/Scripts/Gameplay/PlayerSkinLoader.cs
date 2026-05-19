using UnityEngine;

public class PlayerSkinLoader : MonoBehaviour
{
    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();

        string selectedSkin = SaveManager.Instance != null
            ? SaveManager.Instance.GetSelectedSkin()
            : PlayerPrefs.GetString("SelectedSkin", "Default");

        if (selectedSkin == "Green")
        {
            sr.color = Color.green;
        }
        else if (selectedSkin == "Red")
        {
            sr.color = Color.red;
        }
        else if (selectedSkin == "Gold")
        {
            sr.color = new Color(1f, 0.75f, 0f);
        }
        else if (selectedSkin == "Purple")
        {
            sr.color = new Color(0.6f, 0f, 1f);
        }
        else
        {
            sr.color = Color.cyan;
        }
    }
}
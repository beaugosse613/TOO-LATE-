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

        sr.color = SkinColors.GetColor(selectedSkin);
    }
}
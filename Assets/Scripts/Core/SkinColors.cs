using UnityEngine;

public static class SkinColors
{
    public static Color GetColor(string skinName)
    {
        switch (skinName)
        {
            case "Green":  return Color.green;
            case "Red":    return Color.red;
            case "Gold":   return new Color(1f, 0.75f, 0f);
            case "Purple": return new Color(0.6f, 0f, 1f);
            default:       return Color.cyan;
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

public class MenuBackgroundScroller : MonoBehaviour
{
    [SerializeField] private RawImage backgroundImage;
    [SerializeField] private float    scrollSpeedX = 0.04f;
    [SerializeField] private float    scrollSpeedY = 0f;

    void Update()
    {
        if (backgroundImage == null) return;
        Rect r = backgroundImage.uvRect;
        r.x += scrollSpeedX * Time.deltaTime;
        r.y += scrollSpeedY * Time.deltaTime;
        backgroundImage.uvRect = r;
    }
}

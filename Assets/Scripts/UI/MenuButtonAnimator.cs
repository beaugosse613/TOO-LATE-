using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButtonAnimator : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler,  IPointerUpHandler
{
    [SerializeField] private float hoverScale = 1.08f;
    [SerializeField] private float pressScale = 0.93f;
    [SerializeField] private float animSpeed  = 14f;

    private Vector3 _target = Vector3.one;
    private bool    _hovering;

    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, _target, Time.unscaledDeltaTime * animSpeed);
    }

    public void OnPointerEnter(PointerEventData e) { _hovering = true;  _target = Vector3.one * hoverScale; }
    public void OnPointerExit (PointerEventData e) { _hovering = false; _target = Vector3.one; }
    public void OnPointerDown (PointerEventData e) { _target = Vector3.one * pressScale; }
    public void OnPointerUp   (PointerEventData e) { _target = _hovering ? Vector3.one * hoverScale : Vector3.one; }
}

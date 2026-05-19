using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Vector3 _originalPosition;
    private Coroutine _shakeCoroutine;

    void Start()
    {
        _originalPosition = transform.localPosition;
    }

    public void Shake(float magnitude = 0.15f, float duration = 0.15f)
    {
        if (_shakeCoroutine != null)
            StopCoroutine(_shakeCoroutine);

        _shakeCoroutine = StartCoroutine(ShakeRoutine(magnitude, duration));
    }

    private IEnumerator ShakeRoutine(float magnitude, float duration)
    {
        float elapsed = 0f;
        float seed = Random.value * 100f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float damper = 1f - t * t;

            float x = (Mathf.PerlinNoise(seed + elapsed * 40f, 0f) - 0.5f) * 2f * magnitude * damper;
            float y = (Mathf.PerlinNoise(0f, seed + elapsed * 40f) - 0.5f) * 2f * magnitude * damper;

            transform.localPosition = _originalPosition + new Vector3(x, y, 0f);

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        transform.localPosition = _originalPosition;
    }
}

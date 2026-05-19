using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioSource sfxSource;

    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip hitClip;
    [SerializeField] private AudioClip milestoneClip;
    [SerializeField] private AudioClip coinClip;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void PlayJump()
    {
        if (jumpClip != null)
            sfxSource.PlayOneShot(jumpClip);
    }

    public void PlayHit()
    {
        if (hitClip != null)
            sfxSource.PlayOneShot(hitClip);
    }

    public void PlayMilestone()
    {
        if (milestoneClip != null)
            sfxSource.PlayOneShot(milestoneClip);
    }

    public void PlayCoin()
    {
        if (coinClip != null)
            sfxSource.PlayOneShot(coinClip);
    }
}
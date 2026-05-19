using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float jumpForce = 12.5f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.1f;

    [Header("Feel")]
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float jumpBufferTime = 0.15f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private Color jumpColor = Color.white;
    private Color deathColor = Color.red;
    private Color savedSkinColor;

    private float _coyoteTimer;
    private float _jumpBufferTimer;
    private bool _wasGrounded;
    private Coroutine _squashCoroutine;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        savedSkinColor = sr.color;
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver())
            return;

        bool grounded = IsGrounded();

        // Coyote time: keep the window open for a moment after leaving ground
        if (grounded)
            _coyoteTimer = coyoteTime;
        else
            _coyoteTimer -= Time.deltaTime;

        // Landing squash
        if (grounded && !_wasGrounded)
            StartSquash(new Vector3(1.15f, 0.88f, 1f), new Vector3(1f, 1f, 1f), 0.05f, 0.1f);

        _wasGrounded = grounded;

        bool jumpInput = Input.GetKeyDown(KeyCode.Space)
                      || Input.GetMouseButtonDown(0)
                      || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);

        if (jumpInput)
            _jumpBufferTimer = jumpBufferTime;
        else
            _jumpBufferTimer -= Time.deltaTime;

        if (_jumpBufferTimer > 0f && _coyoteTimer > 0f)
        {
            _jumpBufferTimer = 0f;
            _coyoteTimer = 0f;
            DoJump();
        }
    }

    void DoJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayJump();

        if (VFXManager.Instance != null)
            VFXManager.Instance.PlayJumpPuff(transform.position);

        StartSquash(new Vector3(0.88f, 1.15f, 1f), new Vector3(1f, 1f, 1f), 0.05f, 0.12f);
        StartCoroutine(JumpFlash());
    }

    void StartSquash(Vector3 from, Vector3 to, float inTime, float outTime)
    {
        if (_squashCoroutine != null)
            StopCoroutine(_squashCoroutine);
        _squashCoroutine = StartCoroutine(SquashCoroutine(from, to, inTime, outTime));
    }

    IEnumerator SquashCoroutine(Vector3 target, Vector3 rest, float inTime, float outTime)
    {
        Vector3 startScale = transform.localScale;
        float t = 0f;
        while (t < inTime)
        {
            transform.localScale = Vector3.Lerp(startScale, target, t / inTime);
            t += Time.unscaledDeltaTime;
            yield return null;
        }

        transform.localScale = target;
        t = 0f;

        while (t < outTime)
        {
            transform.localScale = Vector3.Lerp(target, rest, t / outTime);
            t += Time.unscaledDeltaTime;
            yield return null;
        }

        transform.localScale = rest;
    }

    bool IsGrounded()
    {
        if (groundCheck != null)
            return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        return Mathf.Abs(rb.linearVelocity.y) < 0.05f;
    }

    IEnumerator JumpFlash()
    {
        sr.color = jumpColor;
        yield return new WaitForSeconds(0.08f);

        if (GameManager.Instance == null || !GameManager.Instance.IsGameOver())
            sr.color = savedSkinColor;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle"))
        {
            sr.color = deathColor;

            if (VFXManager.Instance != null)
                VFXManager.Instance.PlayDeathBurst(transform.position);

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayHit();

            GameManager.Instance.GameOver();
        }
    }
}

using System.Collections;
using UnityEngine;

public class ArrowController : ObjectPool
{
    private const float SPEED = 11.0f;
    private const float DESTROY_DURATION = 1.0f;
    private Rigidbody2D _rb2D;
    private SpriteRenderer _spriteRenderer;

    private float Gravity => Mathf.Abs(Physics2D.gravity.y * _rb2D.gravityScale);
    private float Angle => Mathf.Atan2(_rb2D.linearVelocityY, _rb2D.linearVelocityX) * Mathf.Rad2Deg;

    private bool _isActive;

    public void Init(Vector2 position, float x, float y)
    {
        Reset();

        this.transform.position = position;

        InitVelocity(x - this.transform.position.x);
    }

    private void Awake()
    {
        _rb2D = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        if (!_isActive) return;
        transform.rotation = Quaternion.Euler(0, 0, Angle);
    }

    private void InitVelocity(float length)
    {
        float sin2alpha = length * Gravity * 1 / Mathf.Pow(SPEED, 2);

        float angle = Mathf.PI / 2 - 0.5f * Mathf.Asin(Mathf.Clamp(sin2alpha, -1, 1));

        _rb2D.linearVelocity = new Vector2(SPEED * Mathf.Cos(angle), SPEED * Mathf.Sin(angle));
    }

    private void Reset()
    {
        Color color = _spriteRenderer.color;
        color.a = 1;
        _spriteRenderer.color = color;

        _rb2D.bodyType = RigidbodyType2D.Dynamic;
        _isActive = true;
    }

    private void StopMotion()
    {
        _rb2D.linearVelocity = Vector2.zero;
        _rb2D.bodyType = RigidbodyType2D.Static;
        _isActive = false;

        StartCoroutine(IE_FadeOut(DESTROY_DURATION));
    }

    private IEnumerator IE_FadeOut(float duration)
    {
        float startTime = Time.time;

        while (Time.time - startTime < duration)
        {
            Color color = _spriteRenderer.color;
            color.a = 1 - (Time.time - startTime) / duration;
            _spriteRenderer.color = color;
            yield return null;
        }

        ReturnToPool();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Ground")) StopMotion();
    }
}
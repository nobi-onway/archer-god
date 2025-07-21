using System.Collections;
using Netick;
using Netick.Unity;
using UnityEngine;

public class ArrowController : NetworkBehaviour
{
    private const float SPEED = 11.0f;
    private const float DESTROY_DURATION = 1.0f;
    private Rigidbody2D _rb2D;
    private SpriteRenderer _spriteRenderer;

    private float Gravity => Mathf.Abs(Physics2D.gravity.y * _rb2D.gravityScale);
    private float Angle => Mathf.Atan2(_rb2D.linearVelocityY, _rb2D.linearVelocityX) * Mathf.Rad2Deg;
    private float TickTime => Sandbox.TickToTime(Sandbox.Tick);
    
    private bool IsActive { get; set; }

    public void Init(float x, float y)
    {
        Reset();

        InitVelocity(x - this.transform.position.x);
    }

    public override void NetworkAwake()
    {
        _rb2D = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public override void NetworkFixedUpdate()
    {
        if (!IsActive) return;

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
        IsActive = true;
    }

    private void StopMotion()
    {
        _rb2D.linearVelocity = Vector2.zero;
        _rb2D.bodyType = RigidbodyType2D.Static;
        IsActive = false;

        StartCoroutine(IE_FadeOut(DESTROY_DURATION));
    }

    private IEnumerator IE_FadeOut(float duration)
    {
        float startTime = TickTime;

        while (TickTime - startTime < duration)
        {
            Color color = _spriteRenderer.color;
            color.a = 1 - (TickTime - startTime) / duration;
            _spriteRenderer.color = color;
            yield return null;
        }

        RPC_Destroy();
    }

    [Rpc(RpcPeers.Owner, RpcPeers.Everyone, isReliable: true, localInvoke: true)]
    private void RPC_Destroy()
    {
        if (!Sandbox.IsServer) return;

        Sandbox.Destroy(this.GetComponent<NetworkObject>());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground")) StopMotion();
    }
}
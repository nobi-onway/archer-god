using Netick;
using Netick.Unity;
using UnityEngine;

public class MainCharacter : NetworkBehaviour
{
    private const float SHOOT_COOL_DOWN = 1f;

    public ETeam Team { get; private set; }

    #region Networked
    [Networked] public float Velocity { get; set; }
    #endregion

    [SerializeField] private Transform _target;
    private Transform Target => _target = _target != null ? _target : GameManager.Instance.GetOpponent(Team);

    [SerializeField] private float _moveSpeed;

    private Collider2D _collider2D;

    private CharacterInput _characterInput;
    private CharacterInput CharacterInput => _characterInput ??= GetComponent<CharacterInput>();
    
    private Animator _animator;
    private Vector3 _nextPos;

    private float _lastShootTime;


    public void Init(ETeam team, Vector3 position)
    {
        Team = team;
        transform.position = position;
    }

    public override void NetworkAwake()
    {
        _collider2D = GetComponent<Collider2D>();
        _animator = GetComponent<Animator>();

        CharacterInput.OnHorizontalInput += DoMovement;
    }

    public override void NetworkFixedUpdate()
    {
        if (Velocity == 0) DoPostShoot();

        _nextPos = Vector2.zero;
    }

    public override void NetworkRender()
    {
        _animator.SetFloat("VELOCITY", Velocity);
    }

    private void DoMovement(int horizontal)
    {
        DoRotate(horizontal);

        Velocity = Mathf.Abs(_moveSpeed * horizontal);

        _nextPos = IsGroundForward() ? Velocity * Sandbox.FixedDeltaTime * transform.right : Vector3.zero;

        transform.position += _nextPos;
    }

    private void DoRotate(int horizontal)
    {
        if (horizontal == 0) return;

        transform.rotation = horizontal.Equals(-1) ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;
    }

    private void DoPostShoot()
    {
        if (!Target) return;

        if (Sandbox.TickToTime(Sandbox.Tick) - _lastShootTime < SHOOT_COOL_DOWN) return;

        DoRotate((int)Mathf.Sign(Target.position.x - transform.position.x));

        RPC_Shoot();

        _lastShootTime = Sandbox.TickToTime(Sandbox.Tick);
    }

    [Rpc(source: RpcPeers.Owner, isReliable: true, localInvoke: true)]
    private void RPC_Shoot()
    {
        _animator.SetTrigger("SHOOT");
    }

    [Rpc(source: RpcPeers.Owner, target: RpcPeers.Proxies, isReliable: true, localInvoke: true)]
    private void Shoot()
    {
        if (!Sandbox.IsServer) return;

        Sandbox.NetworkInstantiate(Sandbox.GetPrefab("Arrow"), this.transform.position + Vector3.up, Quaternion.identity).GetComponent<ArrowController>().Init(Target.position.x, Target.position.y);
    }

    private bool IsGroundForward()
    {
        Ray ray = new(transform.position + _collider2D.bounds.size.x * transform.right + Vector3.down, Vector2.down);
        RaycastHit2D raycastHit2D = Physics2D.Raycast(ray.origin, ray.direction, 1.0f);

        if (raycastHit2D.collider == null) return false;

        return raycastHit2D.collider.CompareTag("Ground");
    }
}
using Netick;
using UnityEngine;

public class MainCharacter : BaseNetworkBehaviour
{
    [SerializeField] private float _moveSpeed;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    private Transform _target;

    [Header("Shoot Settings")]
    [SerializeField] private ArrowController _arrowPrefab;
    [SerializeField] private Collider2D _collider2D;

    private CharacterInput _characterInput;
    private CharacterInput CharacterInput => _characterInput ??= GetComponent<CharacterInput>();

    private bool IsShooting;
    private Animator _animator;
    private bool IsMoving { get; set; }
    private Vector3 _nextPos;

    public void Init(Transform target)
    {
        _target = target;
    }

    private void OnEnable()
    {
        CharacterInput.OnHorizontalInputDown += DoMovement;
        CharacterInput.OnHorizontalInputUp += DoStopMove;
    }

    public override void NetworkRender()
    {
        _animator.SetBool("IS_MOVING", IsMoving);
    }

    private void Awake()
    {
        _collider2D = GetComponent<Collider2D>();
        _animator = GetComponent<Animator>();
    }

    protected override void Tick()
    {
        if (!IsMoving) DoPostShoot();

        _nextPos = Vector2.zero;
    }

    private void DoMovement(int horizontal)
    {
        IsMoving = true;

        DoRotate(horizontal);

        _nextPos = IsGroundForward() ? _moveSpeed * Sandbox.FixedDeltaTime * transform.right : Vector2.zero;

        transform.position += _nextPos;

        IsShooting = false;
    }

    private void DoStopMove()
    {
        IsMoving = false;
    }

    private void DoRotate(int horizontal)
    {
        transform.rotation = horizontal.Equals(-1) ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;
    }

    private void DoPostShoot()
    {
        if (!_target) return;
        if (IsShooting) return;

        IsShooting = true;
        _animator.SetTrigger("SHOOT");

        DoRotate((int)Mathf.Sign(_target.position.x - transform.position.x));
    }

    private void Shoot()
    {
        PoolManager.Instance.ArrowPool.GetObjectPool().GetComponent<ArrowController>().Init(this.transform.position, _target.position.x, _target.position.y);
        IsShooting = false;
    }

    private bool IsGroundForward()
    {
        Ray ray = new(transform.position + _collider2D.bounds.size.x * transform.right, Vector2.down);
        RaycastHit2D raycastHit2D = Physics2D.Raycast(ray.origin, ray.direction, 1.0f);

        if (raycastHit2D.collider == null) return false;

        return raycastHit2D.collider.CompareTag("Ground");
    }
}
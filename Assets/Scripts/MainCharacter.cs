using UnityEngine;

public class MainCharacter : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5.0f;
    [SerializeField] private Animator _animator;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Transform _target;

    [Header("Shoot Settings")]
    [SerializeField] private ArrowController _arrowPrefab;

    [SerializeField] private Collider2D _collider2D;

    private bool IsShooting;
    private bool IsMoving;

    private Vector3 _nextPos;

    private void OnEnable()
    {
        InputManager.Instance.OnHorizontalInputDown += DoMovement;
        InputManager.Instance.OnHorizontalInputUp += DoStopMove;
    }

    private void Awake()
    {
        _collider2D = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (!IsMoving) DoPostShoot();

        _animator.SetBool("IS_MOVING", IsMoving);
        _nextPos = Vector2.zero;
    }

    private void DoMovement(int horizontal)
    {
        IsMoving = true;
        DoRotate(horizontal);

        _nextPos = IsGroundForward() ? _moveSpeed * Time.deltaTime * transform.right : Vector2.zero;

        transform.position += _nextPos;

        IsShooting = false;
    }

    private void DoStopMove() => IsMoving = false;

    private void DoRotate(int horizontal)
    {
        transform.rotation = horizontal.Equals(-1) ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;
    }

    private void DoPostShoot()
    {
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
        Ray ray = new Ray(transform.position + _collider2D.bounds.size.x * transform.right, Vector2.down);
        RaycastHit2D raycastHit2D = Physics2D.Raycast(ray.origin, ray.direction, 1.0f);

        if (raycastHit2D.collider == null) return false;

        return raycastHit2D.collider.CompareTag("Ground");
    }
}
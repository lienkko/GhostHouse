using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    public float MoveSpeed = 4f;
    public bool IsGodMode = false;
    [HideInInspector] public bool CanWalk = true;


    public delegate void OnDeathDelegate();
    public event OnDeathDelegate OnDeath;

    public delegate void DamageDelegate(int damage, int hp);
    public event DamageDelegate OnDamage;

    public Vector2 MoveDir { get; private set; }
    public bool IsCrouching { get; private set; }
    public bool IsDead { get; private set; } = false;
    public int HealthPoints { get; private set; } = 100;
    public float LastHorizontalVector { get; private set; }
    public Vector3 DeltaMove { get; private set; } = Vector3.zero;


    private Rigidbody2D _playerRB;
    private Vector3 _lastPos;
    

    private void Awake()
    {
        Instance = this;
        _lastPos = transform.position;
        _playerRB = GetComponent<Rigidbody2D>();
    }






    private void Update()
    {
        DeltaMove = transform.position - _lastPos;
        _lastPos = transform.position;
        if (CanWalk)
            InputMovement();
        if (HealthPoints == 0)
            Die();
    }

    private void FixedUpdate()
    {
        
        if (CanWalk)
            Move();
    }

    private void InputMovement()
    {

        float moveH = Input.GetAxisRaw("Horizontal");
        float moveV = Input.GetAxisRaw("Vertical");
        IsCrouching = Input.GetKey(KeyCode.LeftShift);
        if (moveH != 0)
            LastHorizontalVector = moveH;
        MoveDir = new Vector2(moveH, moveV).normalized;
    }

    private void Move()
    {
        if (IsCrouching)
            MoveSpeed = 2f;
        else
            MoveSpeed = 4f;
        _playerRB.velocity = MoveDir * MoveSpeed;
    }

    public void InflictDamage(int dmg)
    {
        if (IsGodMode)
        {
            return;
        }
        if (dmg > 0)
        {
            HealthPoints -= dmg;
        }
        if (HealthPoints <= 0)
        {
            HealthPoints = 0;
        }
        OnDamage?.Invoke(dmg, HealthPoints);
    }

    public void ReloadPlayer()
    {
        gameObject.SetActive(false);
        gameObject.SetActive(true);
    }

    private void Die()
    {
        IsDead = true;
        OnDeath?.Invoke();
        gameObject.SetActive(false);
    }
}
